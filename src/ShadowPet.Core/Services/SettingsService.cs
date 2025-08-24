using Newtonsoft.Json;
using ShadowPet.Core.Models;
using ShadowPet.Core.Utils;
namespace ShadowPet.Core.Services
{
    public class SettingsService(
        AppPaths appPaths)
    {
        private readonly string _settingsPath = Path.Combine(appPaths.AppDataFolder, "settings.json");
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public AppSettings LoadSettings()
        {
            AppSettings settings;
            if (!File.Exists(_settingsPath))
            {
                settings = new AppSettings();
            }
            else
            {
                var json = File.ReadAllText(_settingsPath);
                settings = JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
            }

            foreach (var action in settings.PetActions.Where(a => a.ProgramType == SupportedProgram.Unknown))
            {
                action.ProgramType = ProgramDetector.DetectProgramType(action.ProgramPath);
            }

            SyncDefaultLists(settings);

            return settings;
        }

        public void SaveSettings(AppSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings, _serializerSettings);
            File.WriteAllText(_settingsPath, json);
        }

        private void SyncDefaultLists(AppSettings settings)
        {
            var defaultUrls = GetDefaultUrls();
            if (settings.AnnoyingUrls == null)
            {
                settings.AnnoyingUrls = new List<string>();
            }
            var existingUrls = new HashSet<string>(settings.AnnoyingUrls, StringComparer.OrdinalIgnoreCase);
            foreach (var url in defaultUrls)
            {
                if (existingUrls.Add(url))
                {
                    settings.AnnoyingUrls.Add(url);
                }
            }


            var defaultDialogues = GetDefaultDialogues();
            if (settings.CustomDialogues == null)
            {
                settings.CustomDialogues = new List<string>();
            }
            var existingDialogues = new HashSet<string>(settings.CustomDialogues, StringComparer.OrdinalIgnoreCase);
            foreach (var dialogue in defaultDialogues)
            {
                if (existingDialogues.Add(dialogue))
                {
                    settings.CustomDialogues.Add(dialogue);
                }
            }
        }

        private List<string> GetDefaultUrls()
        {
            return new List<string>
            {
                "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                "https://youtu.be/pzjeSdsXVm4?si=N8RnohE0xUzvxqNK", // >:D
                "https://youtu.be/BQQJ1ikGpYY?si=PqGjgRpo4gxjuW1v",
                "https://pointerpointer.com/",
                "https://www.youtube.com/watch?v=4df2CMrpqqo",
                "https://youtu.be/MxbBgi6qorA?si=lXuGznxGIJZbHKNF",
                "https://youtu.be/03br1abjjJw?si=U92V5L-5yLAouu5j",
                "https://www.youtube.com/watch?v=cLCFTQDygS8",
                // "https://www.youtube.com/watch?v=TjpQsYeTnPw", Cerro su cuenta xd
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
                "https://i.imgflip.com/9u1u3k.jpg",
                "https://youtu.be/9hUzb2Fsz6k?si=0bgm7vPo9ubfeUfQ",
                "https://youtu.be/eNfbXJXrxvc?si=eI6P7sb9sKltqjOv",
                "https://youtu.be/AKmyTCLaXrw?si=3fwEcWY9YRBrdd51",
                "https://youtu.be/-wv46hX_gsE?si=QhgF6lYLV__SXrXU",
                "https://www.youtube.com/watch?v=iDLmYZ5HqgM",
                "https://www.youtube.com/watch?v=swnVdhCsYBk",
                "https://youtu.be/iL4EeSCQDFk?si=T_YGIKclhLa4EJax",
                "https://youtu.be/9hUzb2Fsz6k?si=W3iWYHjw-V5JWAvb",
                "https://youtube.com/shorts/Wnt0_QN1PFE?si=FFpx2OPiNJiFf0ma",
                "https://youtube.com/shorts/SoOoWJzE7HI?si=AsR4jhNM1Lbp5neR",
                "https://youtube.com/shorts/qtT1hfJFSCQ?si=vctcmNu80Fp8FaYW",
                "https://youtube.com/shorts/E7cRXyCYHSs?si=jYlWK3FnH4RXq8pf",
                "https://youtube.com/shorts/37tISM8ZG6I?si=MBdFo_toXS0VglEq",
                "https://youtube.com/shorts/HxSU5Ih1BHg?si=tEZjisxRUMTOkN5f",
                "https://youtube.com/shorts/-ED0uVygv70?si=9tnsz3cNN7wEjRLQ"
            };
        }

        private List<string> GetDefaultDialogues()
        {
            return new List<string>
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
    }
}
