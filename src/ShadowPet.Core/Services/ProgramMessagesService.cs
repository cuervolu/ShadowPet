using ShadowPet.Core.Models;
namespace ShadowPet.Core.Services
{
    public class ProgramMessagesService
    {
        private readonly Random _random = new();

        private readonly Dictionary<SupportedProgram, List<string>> _programMessages = new()
        {
            [SupportedProgram.Notepad] = new()
            {
                "Te vigilo...",
                "¿Sabías que los pingüinos tienen rodillas?",
                "Hasta la próxima~  Nah, es mentira. De mi no te libras :3",
                "¡He secuestrado tu Bloc de Notas! Para liberarlo, dame una galleta.",
                "Esta nota se autodestruirá en... bueno, en realidad no.",
                "Abra kadabra, patas de cabra....! .......  ¿Qué? ¿Esperabas que hiciera aparecer un cerdo o algo? ¡Ja! Ojalá, sería interesante.\n",
                "¿Estás seguro de que no deberías estar tomando un tecito?",
                "¡Sorpresa! ¿Me echabas de menos?",
                "Sigue trabajando, yo solo pasaba a saludar... y a juzgar tu historial de navegación.",
                "Oye, sé que soy divino y mantengo tu pc hermoso, pero limpia tu cochinada de vez en cuando :"
            },

            [SupportedProgram.VsCode] = new()
            {
                "¡Hora de que trabajes, esclava! 💻",
                "A codear se ha dicho~ ¡Que empiece el caos!",
                "¿Más bugs? ¡Perfecto! Me encantan los desastres 🐛",
                "Espero que tengas café, esto va para largo...",
                "¡Vamos! ¡Que esos commits no se van a hacer solos!",
                "¿Stack Overflow otra vez? ¡Qué predecible! 😏",
                "Console.log('Hola, soy Shadow y estoy aquí para molestarte');",
                "// TODO: Dejar de procrastinar y ponerse a trabajar",
                "Error 404: Motivación not found. ¿Necesitas ayuda? ¡Nah!",
                "¡Tiempo de debugging! Mi parte favorita: ver cómo sufres 😈"
            }
        };

        public string GetRandomMessageForProgram(SupportedProgram programType)
        {
            if (_programMessages.TryGetValue(programType, out var messages) && messages.Any())
            {
                return messages[_random.Next(messages.Count)];
            }

            return "¡Que disfrutes tu programa! ~Shadow";
        }

        public string GetRandomMessageForProgram(PetAction action)
        {
            return GetRandomMessageForProgram(action.ProgramType);
        }
    }
}