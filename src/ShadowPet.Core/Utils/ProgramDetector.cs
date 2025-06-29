using ShadowPet.Core.Models;
namespace ShadowPet.Core.Utils
{
    public static class ProgramDetector
    {
        public static SupportedProgram DetectProgramType(string programPath)
        {
            if (string.IsNullOrWhiteSpace(programPath))
                return SupportedProgram.Unknown;

            var fileName = Path.GetFileName(programPath).ToLowerInvariant();
            var directoryName = Path.GetDirectoryName(programPath)?.ToLowerInvariant() ?? "";

            return fileName switch
            {
                "notepad.exe" => SupportedProgram.Notepad,
                "code.exe" when directoryName.Contains("microsoft vs code") ||
                                directoryName.Contains("visual studio code") ||
                                directoryName.Contains("vscode") => SupportedProgram.VsCode,
                _ => SupportedProgram.Unknown
            };
        }
    }
}
