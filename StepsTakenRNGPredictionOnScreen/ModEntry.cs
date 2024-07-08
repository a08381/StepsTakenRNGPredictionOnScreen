using System;
using System.Linq;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;
using Object = StardewValley.Object;

namespace StepsTakenOnScreen
{
    public class ModEntry : Mod
    {

        private bool Enable { get; set; } = true;

        private ModConfig config;

        private float dailyLuck;
        // private int islandWeatherForTomorrow;
        // private int weatherForTomorrow;
        private string dishOfTheDay;
        private int dishOfTheDayAmount;
        private string mailPerson;

        private int lastStepsTakenCalculation = -1;
        private int daysPlayedCalculation = -1;
        private int targetStepsCalculation = -1;
        private int targetDay = -1;

        // private string[] islandWeatherValues;
        // private string[] weatherValues;
        private string[] dishValues;
        private string[] giftValues;

        private bool locationsChecked;

        private int extraCalls;

        private bool targetFound;


        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Input.ButtonReleased += OnButtonReleased;
            helper.Events.Display.RenderedHud += OnRenderedHud;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            config = helper.ReadConfig<ModConfig>();
            // islandWeatherValues = config.TargetWeather.Split(new[] { ',' });
            // weatherValues = config.TargetWeather.Split(new[] { ',' });
            dishValues = config.TargetDish.Split(new[] { ',' });
            giftValues = config.TargetGifter.Split(new[] { ',' });
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu == null) return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => config = new ModConfig(),
                save: Save
            );

            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Toggle Gui",
                tooltip: () => "the key to toggle the gui on or off.",
                getValue: () => config.ToggleGui,
                setValue: value => config.ToggleGui = value);

            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Reload Config",
                tooltip: () => "the key to reload the config file.",
                getValue: () => config.ReloadConfig,
                setValue: value => config.ReloadConfig = value);

            // add some config options
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Display Step",
                tooltip: () => "show your steps that will used for seed.",
                getValue: () => config.DisplaySteps,
                setValue: value => config.DisplaySteps = value
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Display Luck",
                tooltip: () => "show your luck that will happen on tomorrow.",
                getValue: () => config.DisplayLuck,
                setValue: value => config.DisplayLuck = value
            );

            /*

            // add some config options
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Display Weather",
                tooltip: () => "show the weather that will happen on aftermorrow.",
                getValue: () => config.DisplayWeather,
                setValue: value => config.DisplayWeather = value
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Display Island Weather",
                tooltip: () => "show the island weather that will happen on aftermorrow.",
                getValue: () => config.DisplayIslandWeather,
                setValue: value => config.DisplayIslandWeather = value
            );

            */

            // add some config options
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Display Gift",
                tooltip: () => "show the island weather that will happen on aftermorrow.",
                getValue: () => config.DisplayGift,
                setValue: value => config.DisplayGift = value
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Display Dish",
                tooltip: () => "show the island weather that will happen on aftermorrow.",
                getValue: () => config.DisplayDish,
                setValue: value => config.DisplayDish = value
            );

            // add some config options
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Horizontal Offset",
                tooltip: () => "show the island weather that will happen on aftermorrow.",
                getValue: () => config.HorizontalOffset,
                setValue: value => config.HorizontalOffset = value,
                min: 0,
                max: Game1.graphics.PreferredBackBufferWidth,
                interval: 1
            );

            // add some config options
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Vertical Offset",
                tooltip: () => "show the island weather that will happen on aftermorrow.",
                getValue: () => config.VerticalOffset,
                setValue: value => config.VerticalOffset = value,
                min: 0,
                max: Game1.graphics.PreferredBackBufferHeight,
                interval: 1
            );

            // add some config options
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Target Luck",
                tooltip: () => "show the island weather that will happen on aftermorrow.",
                getValue: () => config.TargetLuck,
                setValue: value => config.TargetLuck = value,
                min: -1.0f,
                max: 1.0f
            );

            /*

            // add some config options
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Target Weather",
                tooltip: () => "show the island weather that will happen on aftermorrow.",
                getValue: () => config.TargetWeather,
                setValue: value => config.TargetWeather = value
            );

            // add some config options
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Target Island Weather",
                tooltip: () => "show the island weather that will happen on aftermorrow.",
                getValue: () => config.TargetIslandWeather,
                setValue: value => config.TargetIslandWeather = value
            );

            */

            // add some config options
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Target Gifter",
                tooltip: () => "show the island weather that will happen on aftermorrow.",
                getValue: () => config.TargetGifter,
                setValue: value => config.TargetGifter = value
            );

            // add some config options
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Target Dish",
                tooltip: () => "show the island weather that will happen on aftermorrow.",
                getValue: () => config.TargetDish,
                setValue: value => config.TargetDish = value
            );

            // add some config options
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Target Dish Amount",
                tooltip: () => "show the island weather that will happen on aftermorrow.",
                getValue: () => config.TargetDishAmount,
                setValue: value => config.TargetDishAmount = value
            );

            // add some config options
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Target Steps Limit",
                tooltip: () => "show the island weather that will happen on aftermorrow.",
                getValue: () => config.TargetStepsLimit,
                setValue: value => config.TargetStepsLimit = value
            );
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            locationsChecked = false;
        }
        
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (e.Button == config.ReloadConfig)
            {
                Reload();
            }

            if (e.Button == config.ToggleGui)
            {
                Enable = !Enable;
            }
        }
        
        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (!e.Button.IsActionButton() && !e.Button.IsUseToolButton()) return;

            Vector2 tile = e.Cursor.GrabTile;
            Object obj = Game1.currentLocation.getObjectAtTile((int)tile.X, (int)tile.Y);
            if (obj is { MinutesUntilReady: > 0 })
            {
                locationsChecked = false;
            }
        }
        
        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (!Enable) return;

            var spriteBatch = Game1.spriteBatch;

            bool furnitureChanged = false;
            if (!locationsChecked)
            {
                int oldExtraCalls = extraCalls;
                CheckLocations();
                furnitureChanged = extraCalls != oldExtraCalls;
            }

            if ((config.DisplayDish || config.DisplayGift || config.DisplayLuck) && (furnitureChanged || lastStepsTakenCalculation != Game1.stats.StepsTaken || daysPlayedCalculation != Game1.stats.DaysPlayed))
            {
                lastStepsTakenCalculation = (int)Game1.stats.StepsTaken;
                daysPlayedCalculation = (int)Game1.stats.DaysPlayed;
                CalculatePredictions(lastStepsTakenCalculation, out dailyLuck, out dishOfTheDay, out dishOfTheDayAmount, out mailPerson);
            }

            string str = "";
            if (config.DisplaySteps) str = InsertLine(str, GetStepsTaken());
            if (config.DisplayLuck) str = InsertLine(str, GetLuck());
            // if (config.DisplayWeather) str = InsertLine(str, GetWeather(weatherForTomorrow));
            // if (config.DisplayIslandWeather) str = InsertLine(str, GetIslandWeather(islandWeatherForTomorrow));
            if (config.DisplayDish) str = InsertLine(str, GetDishOfTheDay());
            if (config.DisplayGift) str = InsertLine(str, GetMailPerson());

            if (config.TargetLuck != -1.0 || config.TargetGifter != "" || config.TargetDish != "")
            {
                if (furnitureChanged || Game1.stats.Get("stepsTaken") > (ulong)targetStepsCalculation || Game1.stats.Get("daysPlayed") != (ulong)targetDay)
                {
                    targetFound = false;
                    targetDay = (int)Game1.stats.Get("daysPlayed");
                    for (int steps = 0; steps < config.TargetStepsLimit; steps++)
                    {
                        targetStepsCalculation = steps + (int)Game1.stats.Get("stepsTaken");
                        CalculatePredictions(targetStepsCalculation, out float luck, out string dish, out int dishAmount, out string person);
                        if ((config.TargetLuck != -1.0 && !(luck >= config.TargetLuck)) ||
                            (config.TargetDish != "" && (!dishValues.Contains(GetDishOfTheDayValue(dish)) ||
                                                         dishAmount < config.TargetDishAmount)) ||
                            (config.TargetGifter != "" && !giftValues.Contains(person))) continue;
                        targetFound = true;
                        break;
                    }
                }

                str = InsertLine(str, "");
                str = targetFound ?
                    InsertLine(str, "Steps required to hit target: " + targetStepsCalculation) :
                    InsertLine(str, "Criteria not met after searching to step count: " + targetStepsCalculation);
                str = InsertLine(str, "Criteria:");
                if (config.TargetLuck != -1.0) str = InsertLine(str, "Luck: " + config.TargetLuck);
                // if (config.TargetIslandWeather != "") str = InsertLine(str, "Island Weather: " + config.TargetIslandWeather);
                // if (config.TargetWeather != "") str = InsertLine(str, "Weather: " + config.TargetWeather);
                if (config.TargetDish != "") str = InsertLine(str, "Dish: " + config.TargetDish);
                if (config.TargetGifter != "") str = InsertLine(str, "Gifter: " + config.TargetGifter);
            }

            if (str == "") return;
            string text = str;
            Vector2 vector = new(config.HorizontalOffset, config.VerticalOffset);
            DrawHelper.DrawHoverBox(spriteBatch, text, vector, Game1.viewport.Width);
        }

        private void Reload()
        {
            config = Helper.ReadConfig<ModConfig>();
            Monitor.Log("Config reloaded", LogLevel.Info);
            // islandWeatherValues = config.TargetWeather.Split(new[] { ',' });
            // weatherValues = config.TargetWeather.Split(new[] { ',' });
            dishValues = config.TargetDish.Split(new[] { ',' });
            giftValues = config.TargetGifter.Split(new[] { ',' });
            targetStepsCalculation = 0;
        }

        private void Save()
        {
            Helper.WriteConfig(config);
            Monitor.Log("Config saved", LogLevel.Info);
            // islandWeatherValues = config.TargetWeather.Split(new[] { ',' });
            // weatherValues = config.TargetWeather.Split(new[] { ',' });
            dishValues = config.TargetDish.Split(new[] { ',' });
            giftValues = config.TargetGifter.Split(new[] { ',' });
            targetStepsCalculation = 0;
        }

        private string GetStepsTaken()
        {
            return Helper.Translation.Get("DisplaySteps") + Game1.stats.Get("stepsTaken");
        }
        
        private string GetLuck()
        {
            return Helper.Translation.Get("DisplayLuck") + dailyLuck;
        }

        /*
        
        private string GetWeatherValue(int weatherValue)
        {
            string weather = weatherValue switch
            {
                0 => "Sunny",
                1 => "Rain",
                2 => "Debris",
                3 => "Lightning",
                4 => "Festival",
                5 => "Snow",
                6 => "Wedding",
                _ => ""
            };
            return weather;
        }
        
        private string GetIslandWeather(int weatherValue)
        {
            return Helper.Translation.Get("DisplayIslandWeather") + Helper.Translation.Get(GetWeatherValue(weatherValue));
        }
        
        private string GetWeather(int weatherValue)
        {
            return Helper.Translation.Get("DisplayWeather") + Helper.Translation.Get(GetWeatherValue(weatherValue));
        }

        */

        private string GetDishOfTheDay()
        {
            return string.Concat(Helper.Translation.Get("DisplayDish"), GetDishOfTheDayValue(dishOfTheDay), " (", dishOfTheDayAmount.ToString(), ")");
        }
        
        private string GetDishOfTheDayValue(string dish)
        {
            return TokenParser.ParseText(Game1.objectData[dish].DisplayName);
        }
        
        private string GetMailPerson()
        {
            string displayName = "";
            if (mailPerson != "")
            {
                displayName = TokenParser.ParseText(Game1.getCharacterFromName(mailPerson).GetTokenizedDisplayName());
            }
            return Helper.Translation.Get("DisplayGift") + displayName;
        }

        private bool IsCorrectItem(Object item, GameLocation location, int minutesUntilMorning)
        {
            if (item.heldObject.Value == null || item.name.Contains("Table") ||
                (item.bigCraftable.Value && item.ParentSheetIndex == 165)) return false;
            if (item.name.Equals("Bee House") && !location.IsOutdoors || item.IsSprinkler() || item.bigCraftable.Value && item.ParentSheetIndex == 231)
                return false;
            return (item.MinutesUntilReady <= minutesUntilMorning && !item.name.Contains("Incubator")) || item.readyForHarvest.Value;
        }
        
        private void CheckLocations()
        {
            if (locationsChecked) return;
            locationsChecked = true;

            int minutesUntilMorning = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);

            extraCalls = 0;

            extraCalls += (from location in Game1.locations
                from pair in location.Objects.Pairs
                let item = pair.Value
                where IsCorrectItem(item, location, minutesUntilMorning)
                select item).Count();

            extraCalls += (from b in Game1.getFarm().buildings
                where b.indoors.Value != null
                let location = b.indoors.Value
                from pair in location.objects.Pairs
                let item = pair.Value
                where IsCorrectItem(item, location, minutesUntilMorning)
                select item).Count();
        }

        // private static IEnumerator<int> _newDayAfterFade()
        private void CalculatePredictions(int steps, out float dailyLuck, out string dishOfTheDay, out int dishOfTheDayAmount, out string mailPerson)
        {
            CheckLocations();

            Monitor.Log(extraCalls.ToString());

            string currentSeason = Game1.currentSeason;
            int season = currentSeason switch
            {
                "spring" => 1,
                "summer" => 2,
                "fall" => 3,
                "winter" => 4,
                _ => 1
            };
            int dayOfMonth = Game1.dayOfMonth + 1;
            if (dayOfMonth == 29)
            {
                dayOfMonth = 1;
                season++;
                if (season == 5)
                {
                    season = 1;
                }
            }

            int daysPlayed = (int)(Game1.stats.DaysPlayed + 1U);
            int seed = (int)Game1.uniqueIDForThisGame / 100 + daysPlayed * 10 + 1 + steps;

            Random random = Utility.CreateRandom(seed);
            //Random random = new((int)Game1.uniqueIDForThisGame / 100 + (int)(Game1.stats.DaysPlayed * 10) + 1 + steps);

            for (int index = 0; index < Game1.dayOfMonth; ++index) random.Next();
            do
            {
                dishOfTheDay = random.Next(194, 240).ToString();
            } while (Utility.IsForbiddenDishOfTheDay(dishOfTheDay));
            dishOfTheDayAmount = random.Next(1, 4 + ((random.NextDouble() < 0.08) ? 10 : 0));
            random.NextDouble();
            for (int index2 = 0; index2 < extraCalls; index2++)
            {
                random.Next();
            }
            mailPerson = "";
            if (Utility
                    .TryGetRandom(Game1.player.friendshipData,
                        out string whichFriend, out Friendship friendship, random) &&
                random.NextBool((double)friendship.Points / 250 * 0.1) &&
                Game1.player.spouse != whichFriend && DataLoader.Mail(Game1.content).ContainsKey(whichFriend))
            {
                mailPerson = whichFriend;
            }
            /*
            if (Game1.player.friendshipData.Any())
            {
                string key = Game1.player.friendshipData.Keys.ElementAt(random.Next(Game1.player.friendshipData.Keys.Count()));
                if (random.NextDouble() < (double)Game1.player.friendshipData[key].Points / 250 * 0.1 && (Game1.player.spouse == null || !Game1.player.spouse.Equals(key)) && Game1.content.Load<Dictionary<string, string>>("Data\\mail").ContainsKey(key))
                {
                    mailPerson = key;
                }
            }
            */
            random.NextDouble();

            foreach (var value in Utility.getHomeOfFarmer(Game1.player).netObjects.Values)
            {
                Mannequin mannequin = value as Mannequin;
                if (mannequin != null)
                {
                    MannequinData data = null;
                    if (data == null && !DataLoader.Mannequins(Game1.content).TryGetValue(mannequin.ItemId, out data))
                    {
                        data = null;
                    }
                    if (data.Cursed && random.NextDouble() < 0.005)
                    {
                        bool value2 = mannequin.swappedWithFarmerTonight.Value;
                    }
                }
            }
            dailyLuck = random.Next(-100, 101) / 1000.0f;
            // dailyLuck = Math.Min(0.10000000149011612f, Game1.random.Next(-100, 101) / 1000.0f);

            /*
            islandWeather = (random.NextDouble() < 0.24) ? 1 : 0;
            if (Game1.weatherForTomorrow == 2)
            {
                int num = random.Next(16, 64);
                for (int index3 = 0; index3 < num; index3++)
                {
                    random.Next();
                    random.Next();
                    random.Next();
                    random.Next();
                    random.Next();
                    random.Next();
                }
            }
            double chanceToRainTomorrow = season switch
            {
                2 => 0.12 + dayOfMonth * 0.003,
                4 => 0.63,
                _ => 0.183
            };
            if (random.NextDouble() < chanceToRainTomorrow)
            {
                weather = 1;
                if ((season == 2 && random.NextDouble() < 0.85) || (season != 4 && random.NextDouble() < 0.25 && dayOfMonth > 2 && Game1.stats.DaysPlayed > 27U))
                {
                    weather = 3;
                }
                if (season == 4)
                {
                    weather = 5;
                }
            }
            else
            {
                weather = (Game1.stats.DaysPlayed <= 1U || ((season != 1 || random.NextDouble() >= 0.2) && (season != 3 || random.NextDouble() >= 0.6))) ? 0 : 2;
            }
            if (Utility.isFestivalDay(dayOfMonth + 1, Game1.currentSeason))
            {
                weather = 4;
            }
            weather = Game1.stats.DaysPlayed switch
            {
                1U => 1,
                2U => 0,
                _ => weather
            };
            if (season == 2 && dayOfMonth % 13 == 12)
            {
                weather = 3;
            }
            if (dayOfMonth == 28)
            {
                weather = 0;
            }
            */
        }
        
        private string InsertLine(string str, string newStr)
        {
            if (str == "")
            {
                return newStr;
            }
            return str + "\n" + newStr;
        }
    }
}
