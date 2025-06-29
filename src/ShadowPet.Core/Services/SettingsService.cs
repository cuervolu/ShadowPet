using System.IO;
using Newtonsoft.Json;
using ShadowPet.Core.Models;
using ShadowPet.Core.Utils;

namespace ShadowPet.Core.Services
{
    public class SettingsService
    {
        private readonly string _settingsPath;
        private readonly JsonSerializerSettings _serializerSettings;

        public SettingsService(AppPaths appPaths)
        {
            _settingsPath = Path.Combine(appPaths.AppDataFolder, "settings.json");
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        public AppSettings LoadSettings()
        {
            if (!File.Exists(_settingsPath))
            {
                return new AppSettings();
            }

            var json = File.ReadAllText(_settingsPath);
            var settings = JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();

            foreach (var action in settings.PetActions.Where(a => a.ProgramType == SupportedProgram.Unknown))
            {
                action.ProgramType = ProgramDetector.DetectProgramType(action.ProgramPath);
            }

            return settings;
        }

        public void SaveSettings(AppSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings, _serializerSettings);
            File.WriteAllText(_settingsPath, json);
        }
    }
}
