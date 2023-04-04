namespace StepsTakenOnScreen
{
    internal class ModConfig
    {
        public bool DisplaySteps { get; set; } = true;
        
        public bool DisplayLuck { get; set; } = true;
        
        public bool DisplayIslandWeather { get; set; } = true;
        
        public bool DisplayWeather { get; set; } = true;
        
        public bool DisplayGift { get; set; }
        
        public bool DisplayDish { get; set; }
        
        public int HorizontalOffset { get; set; }
        
        public int VerticalOffset { get; set; }
        
        public double TargetLuck { get; set; } = -1.0;
        
        public string TargetIslandWeather { get; set; } = "";
        
        public string TargetWeather { get; set; } = "";
        
        public string TargetGifter { get; set; } = "";
        
        public string TargetDish { get; set; } = "";
        
        public int TargetDishAmount { get; set; }
        
        public int TargetStepsLimit { get; set; } = 1000;
    }
}
