using System.IO;
using Newtonsoft.Json;
using ShadowPet.Core.Models;

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
            return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
        }

        public void SaveSettings(AppSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings, _serializerSettings);
            File.WriteAllText(_settingsPath, json);
        }
    }
}
