using ShadowPet.Core.Models;
namespace ShadowPet.Core.Services;

public class AnimationService
{
    private readonly Dictionary<string, PetAnimation> _animations;

    public AnimationService()
    {
        _animations = new List<PetAnimation>
        {
            new("idle", "/Assets/shadow.png", 1, 1000, true),
            new("moving", "/Assets/Sprite_Movimiento.png", 11, 1000, true),
            new("attention", "/Assets/Atención.png", 11, 1200, true),
            new("victoria_face", "/Assets/cara_victoria.png", 29, 2000, true),
            new("interacting", "/Assets/Interacción.png", 23, 1500, true),
            new("dragging", "/Assets/Ratón.png", 20, 1800, true),
            new("take_item_intro", "/Assets/Tomar_Cosas.png", 18, 1500, false),
            new("take_item_loop", "/Assets/TomarCosas_Movimiento.png", 11, 1000, true),
            new("patpat", "/Assets/Acariciar_Shadow.png", 8, 1500, false),
            new("vaquita_jumping", "/Assets/Shadow_vaquita_saltando.png", 4, 800, true),
            new("vaquita", "/Assets/Shadow_vaquita.png", 16, 1500, true)

        }.ToDictionary(anim => anim.Name);
    }

    public PetAnimation? GetAnimation(string name)
    {
        _animations.TryGetValue(name, out var animation);
        return animation;
    }
}
