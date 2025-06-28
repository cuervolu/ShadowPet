namespace ShadowPet.Core.Services
{
    public class AppPaths
    {
        public string AppDataFolder { get; }
        public string LogFilePath { get; }

        public AppPaths()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            AppDataFolder = Path.Combine(appDataPath, "dev.cuervolu.shadowpet");

            Directory.CreateDirectory(AppDataFolder);

            LogFilePath = Path.Combine(AppDataFolder, "log.log");
        }
    }
}
