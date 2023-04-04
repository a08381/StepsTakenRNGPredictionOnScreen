using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;

namespace StepsTakenOnScreen
{
    public class ModEntry : Mod
    {

        private bool Enable { get; set; } = true;

        private ModConfig config;

        private double dailyLuck;
        private int islandWeatherForTomorrow;
        private int weatherForTomorrow;
        private int dishOfTheDay;
        private int dishOfTheDayAmount;
        private string mailPerson;

        private int lastStepsTakenCalculation = -1;
        private int daysPlayedCalculation = -1;
        private int targetStepsCalculation = -1;
        private int targetDay = -1;

        private string[] islandWeatherValues;
        private string[] weatherValues;
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
            config = Helper.ReadConfig<ModConfig>();
            islandWeatherValues = config.TargetWeather.Split(new[] { ',' });
            weatherValues = config.TargetWeather.Split(new[] { ',' });
            dishValues = config.TargetDish.Split(new[] { ',' });
            giftValues = config.TargetGifter.Split(new[] { ',' });
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
            switch (e.Button)
            {
                case SButton.F5:
                    config = Helper.ReadConfig<ModConfig>();
                    Monitor.Log("Config reloaded", LogLevel.Info);
                    islandWeatherValues = config.TargetWeather.Split(new[] { ',' });
                    weatherValues = config.TargetWeather.Split(new[] { ',' });
                    dishValues = config.TargetDish.Split(new[] { ',' });
                    giftValues = config.TargetGifter.Split(new[] { ',' });
                    targetStepsCalculation = 0;
                    break;
                case SButton.F3:
                    Enable = !Enable;
                    break;
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

            if ((config.DisplayDish || config.DisplayGift || config.DisplayLuck || config.DisplayWeather) && (furnitureChanged || lastStepsTakenCalculation != Game1.stats.StepsTaken || daysPlayedCalculation != Game1.stats.DaysPlayed))
            {
                lastStepsTakenCalculation = (int)Game1.stats.StepsTaken;
                daysPlayedCalculation = (int)Game1.stats.DaysPlayed;
                CalculatePredictions(lastStepsTakenCalculation, out dailyLuck, out islandWeatherForTomorrow, out weatherForTomorrow, out dishOfTheDay, out dishOfTheDayAmount, out mailPerson);
            }

            string str = "";
            if (config.DisplaySteps) str = InsertLine(str, GetStepsTaken());
            if (config.DisplayLuck) str = InsertLine(str, GetLuck());
            if (config.DisplayIslandWeather) str = InsertLine(str, GetIslandWeather(islandWeatherForTomorrow));
            if (config.DisplayWeather) str = InsertLine(str, GetWeather(weatherForTomorrow));
            if (config.DisplayDish) str = InsertLine(str, GetDishOfTheDay());
            if (config.DisplayGift) str = InsertLine(str, GetMailPerson());

            if (config.TargetLuck != -1.0 || config.TargetWeather != "" || config.TargetGifter != "" || config.TargetDish != "")
            {
                if (furnitureChanged || Game1.stats.StepsTaken > (ulong)targetStepsCalculation || Game1.stats.daysPlayed != (ulong)targetDay)
                {
                    targetFound = false;
                    targetDay = (int)Game1.stats.daysPlayed;
                    for (int steps = 0; steps < config.TargetStepsLimit; steps++)
                    {
                        targetStepsCalculation = steps + (int)Game1.stats.StepsTaken;
                        CalculatePredictions(targetStepsCalculation, out double luck, out int islandWeather, out int weather, out int dish, out int dishAmount, out string person);
                        if ((config.TargetLuck != -1.0 && !(luck >= config.TargetLuck)) ||
                            (config.TargetIslandWeather != "" &&
                             !islandWeatherValues.Contains(GetWeatherValue(islandWeather))) ||
                            (config.TargetWeather != "" && !weatherValues.Contains(GetWeatherValue(weather))) ||
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
                if (config.TargetIslandWeather != "") str = InsertLine(str, "Island Weather: " + config.TargetIslandWeather);
                if (config.TargetWeather != "") str = InsertLine(str, "Weather: " + config.TargetWeather);
                if (config.TargetDish != "") str = InsertLine(str, "Dish: " + config.TargetDish);
                if (config.TargetGifter != "") str = InsertLine(str, "Gifter: " + config.TargetGifter);
            }

            if (str == "") return;
            string text = str;
            Vector2 vector = new(config.HorizontalOffset, config.VerticalOffset);
            DrawHelper.DrawHoverBox(spriteBatch, text, vector, Game1.viewport.Width);
        }
        
        private string GetStepsTaken()
        {
            return Helper.Translation.Get("DisplaySteps") + Game1.stats.stepsTaken;
        }
        
        private string GetLuck()
        {
            return Helper.Translation.Get("DisplayLuck") + dailyLuck;
        }
        
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
        
        private string GetDishOfTheDay()
        {
            return string.Concat(Helper.Translation.Get("DisplayDish"), GetDishOfTheDayValue(dishOfTheDay), " (", dishOfTheDayAmount.ToString(), ")");
        }
        
        private string GetDishOfTheDayValue(int dish)
        {
            return Game1.objectInformation[dish].Split(new[] { '/' })[4];
        }
        
        private string GetMailPerson()
        {
            return Helper.Translation.Get("DisplayGift") + mailPerson;
        }

        private bool IsCorrectItem(Object item, GameLocation location, int minutesUntilMorning)
        {
            return item.heldObject.Value != null &&
                   !item.name.Contains("Table") &&
                   (!item.bigCraftable.Value || item.ParentSheetIndex != 165) &&
                   (!item.bigCraftable.Value || item.ParentSheetIndex != 231) &&
                   (!item.name.Equals("Bee House") || location.IsOutdoors) && !item.IsSprinkler() &&
                   (item.MinutesUntilReady - minutesUntilMorning > 0 || item.name.Contains("Incubator"));
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
        
        private void CalculatePredictions(int steps, out double dailyLuck, out int islandWeather, out int weather, out int dishOfTheDay, out int dishOfTheDayAmount, out string mailPerson)
        {
            CheckLocations();
            Random random = new((int)Game1.uniqueIDForThisGame / 100 + (int)(Game1.stats.DaysPlayed * 10U) + 1 + steps);
            for (int index = 0; index < Game1.dayOfMonth; index++)
            {
                random.Next();
            }
            dishOfTheDay = random.Next(194, 240);
            while (Utility.getForbiddenDishesOfTheDay().Contains(dishOfTheDay))
            {
                dishOfTheDay = random.Next(194, 240);
            }
            dishOfTheDayAmount = random.Next(1, 4 + ((random.NextDouble() < 0.08) ? 10 : 0));
            random.NextDouble();
            for (int index2 = 0; index2 < extraCalls; index2++)
            {
                random.NextDouble();
            }
            mailPerson = "";
            if (Game1.player.friendshipData.Any())
            {
                string key = Game1.player.friendshipData.Keys.ElementAt(random.Next(Game1.player.friendshipData.Keys.Count()));
                if (random.NextDouble() < (double)Game1.player.friendshipData[key].Points / 250 * 0.1 && (Game1.player.spouse == null || !Game1.player.spouse.Equals(key)) && Game1.content.Load<Dictionary<string, string>>("Data\\mail").ContainsKey(key))
                {
                    mailPerson = key;
                }
            }
            random.NextDouble();
            dailyLuck = random.Next(-100, 101) / 1000.0;
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
