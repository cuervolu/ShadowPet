namespace ShadowPet.Core.Services;

public class DialogueService
{
    private readonly Random _random = new();

    private readonly List<string> _dialogues = new()
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

    public string GetRandomDialogue()
    {
        var index = _random.Next(_dialogues.Count);
        return _dialogues[index];
    }
}
