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

            if (settings.AnnoyingUrls == null || settings.AnnoyingUrls.Count == 0)
            {
                InitializeDefaultUrls(settings);
            }

            if (settings.CustomDialogues == null || settings.CustomDialogues.Count == 0)
            {
                InitializeDefaultDialogues(settings);
            }

            return settings;
        }

        private void InitializeDefaultUrls(AppSettings settings)
        {
            settings.AnnoyingUrls = new List<string>
            {
                "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                "https://youtu.be/pzjeSdsXVm4?si=N8RnohE0xUzvxqNK", // >:D
                "https://youtu.be/BQQJ1ikGpYY?si=PqGjgRpo4gxjuW1v",
                "https://pointerpointer.com/",
                "https://www.youtube.com/watch?v=4df2CMrpqqo",
                "https://youtu.be/MxbBgi6qorA?si=lXuGznxGIJZbHKNF",
                "https://youtu.be/03br1abjjJw?si=U92V5L-5yLAouu5j",
                "https://www.youtube.com/watch?v=cLCFTQDygS8",
                "https://www.youtube.com/watch?v=TjpQsYeTnPw",
                "https://www.youtube.com/watch?v=vOLwTy7J5jY",
                "https://www.youtube.com/watch?v=Q9sK-g82KuE",
                "https://youtube.com/shorts/ZAVbtqWAAds?si=5xldlFMy-sZ1RKKy",
                "https://www.youtube.com/shorts/dPBWvx2YASc",
                "https://www.youtube.com/shorts/LbrJowrCFEw",
                "https://www.youtube.com/watch?v=Ruu3q4EGY-k",
                "https://www.youtube.com/shorts/_C3BXBM3pQM",
                "https://www.youtube.com/watch?v=ar3sgABf1eA",
                "https://preview.redd.it/shadow-milk-cookie-when-he-sees-pure-vanilla-v0-dtr3jyfc1dme1.jpeg?width=1080&crop=smart&auto=webp&s=142144602fea1b8af4652d50d3b78b65d7cc16d9",
                "https://64.media.tumblr.com/0e4b377b68ebc9370e8146a2da9df54b/744bcb252f91c376-ee/s1280x1920/dbda3871a3d3339b02906b9c24b70094eaf2f791.png",
                "https://i.pinimg.com/236x/ca/e9/63/cae96395a951b0c7212359ab66b2f39e.jpg",
                "https://tr.rbxcdn.com/180DAY-91b0d03fed98d377b00dc6660c3f2128/420/420/Hat/Webp/noFilter",
                "https://i.pinimg.com/236x/34/c7/55/34c755dd000074cc14fa7780404618d6.jpg",
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRVFSH5M1uH9_LKKFySdxS3X90qiusrjJr7VQ&s",
                "https://i.imgflip.com/9u1u3k.jpg"
            };
        }

        private void InitializeDefaultDialogues(AppSettings settings)
        {
            settings.CustomDialogues = new List<string>
            {
                "Heh.",
                "¿Listos para unas risitas?",
                "Lamento decírtelo, ¡pero todo esto es un montaje!",
                "¡TA-DÁ! ¡La estrella del show ha llegado!",
                "¡Oh, y yo que pensaba que el payaso era yo!",
                "¡QUÉ ABURRIDO!",
                "¿Qué hiciste? Hmm, como que me gusta.",
                "¡AHHH! ¡Qué aire tan FRESCO!",
                "¡¡¡Que las mentiras se esparzan como la pólvora!!!",
                "¡Miren allá! ¡¡¡MENTIRAS Y CAOS!!!",
                "¡Vaya, vaya! ¿Listos para unas cuantas travesuras?",
                "Si te dijera que soy un mentiroso, ¿sería eso una mentira...?",
                "¿Por qué esa cara taaan larga?",
                "¡Es hora de un verdadero DRAMA!",
                "Todos vivimos por el chismecito, ¿o no?",
                "¡¿DONDE CRESTA ESTA VAINILLA?!",
                "¡¿Dónde está mi VAINILLA?!",
                "Ya llegó por quién lloraban! ¡Aplaudan!",
                "¿Tienes alguna idea para quitar el aburrimiento?",
                "Uuuuuh, tremendo chisme estoy viendo.",
                "Hey niña, ¿quieres comprar lechuga?",
                "Acabo de tener un sueño donde yo era Shadow Milk Cookie"
            };
        }


        public void SaveSettings(AppSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings, _serializerSettings);
            File.WriteAllText(_settingsPath, json);
        }
    }
}
