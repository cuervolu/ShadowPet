namespace ShadowPet.Core.Services
{
    public class DialogueService
    {
        private readonly Random _random = new();
        private readonly SettingsService _settingsService;

        public DialogueService(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public string GetRandomDialogue()
        {
            var settings = _settingsService.LoadSettings();
            var dialogues = settings.CustomDialogues;

            if (dialogues.Count == 0)
            {
                return "No se qué decir...";
            }

            var index = _random.Next(dialogues.Count);
            return dialogues[index];
        }
    }
}
