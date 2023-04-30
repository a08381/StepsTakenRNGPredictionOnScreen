using StardewModdingAPI;

namespace StepsTakenOnScreen
{
    internal class ModConfig
    {

        public SButton ToggleGui { get; set; } = SButton.F3;

        public SButton ReloadConfig { get; set; } = SButton.F5;

        public bool DisplaySteps { get; set; } = true;
        
        public bool DisplayLuck { get; set; } = true;

        public bool DisplayWeather { get; set; } = true;

        public bool DisplayIslandWeather { get; set; } = true;
        
        public bool DisplayGift { get; set; }
        
        public bool DisplayDish { get; set; }
        
        public int HorizontalOffset { get; set; }
        
        public int VerticalOffset { get; set; }
        
        public float TargetLuck { get; set; } = -1.0f;

        public string TargetWeather { get; set; } = "";

        public string TargetIslandWeather { get; set; } = "";
        
        public string TargetGifter { get; set; } = "";
        
        public string TargetDish { get; set; } = "";
        
        public int TargetDishAmount { get; set; }
        
        public int TargetStepsLimit { get; set; } = 1000;
    }
}
