namespace ShadowPet.Core.Services
{
    public class AnnoyingMessagesService
    {
        private readonly List<string> _messages = new()
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
        };

        public List<string> GetAllMessages()
        {
            return _messages;
        }
    }
}
