namespace ShadowPet.Core.Models
{
    public class PetAnimation(
        string name,
        string spriteSheetPath,
        int frameCount,
        int durationMs,
        bool isLooping = true)
    {
        // El nombre único para identificar esta animación, ej: "idle", "walking"
        public string Name { get; } = name;

        // Ruta al archivo de spritesheet en los Assets
        public string SpriteSheetPath { get; } = spriteSheetPath;

        // Cuántos frames tiene la animación en el spritesheet
        public int FrameCount { get; } = frameCount;

        // Duración total de un ciclo de animación en milisegundos
        public int DurationMs { get; } = durationMs;

        // Si la animación debe repetirse en bucle
        public bool IsLooping { get; } = isLooping;

    }
}
