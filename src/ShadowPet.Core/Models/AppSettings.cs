namespace ShadowPet.Core.Models
{
    public class AppSettings
    {
        public bool HasRunBefore { get; set; } = false;
        public bool StartWithWindows { get; set; } = false;
        public bool AllowNotifications { get; set; } = true;
        public double AnnoyanceLevel { get; set; } = 50; // 0-100
        public double SoundVolume { get; set; } = 80; // 0-100
        public bool AllowProgramExecution { get; set; } = true;
        public string AppTheme { get; set; } = "Dark";
        public List<PetAction> PetActions { get; set; } = new();
        public List<string> AnnoyingUrls { get; set; } = new();
        public List<string> CustomDialogues { get; set; } = new();
    }
}
