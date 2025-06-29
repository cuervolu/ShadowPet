namespace ShadowPet.Core.Models
{
    public class PetAction
    {
        public string Name { get; set; } = "Acción sin nombre";
        public string ProgramPath { get; set; } = string.Empty;
        public SupportedProgram ProgramType { get; set; } = SupportedProgram.Unknown;
    }
}
