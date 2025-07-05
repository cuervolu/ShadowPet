using Microsoft.Extensions.Logging;
using ShadowPet.Core.Models;
using ShadowPet.Core.Utils;
using System.Diagnostics;
namespace ShadowPet.Core.Services
{
    public class ProcessService(
        ILogger<ProcessService> logger)
    {
        private readonly Random _random = new Random();
        public void StartProgram(PetAction action, string message)
        {
            if (string.IsNullOrWhiteSpace(action.ProgramPath) || !File.Exists(action.ProgramPath))
            {
                logger.LogWarning("Se intentó iniciar un proceso con una ruta inválida: {path}", action.ProgramPath);
                return;
            }

            try
            {
                if (action.ProgramType == SupportedProgram.Unknown)
                {
                    action.ProgramType = ProgramDetector.DetectProgramType(action.ProgramPath);
                }

                switch (action.ProgramType)
                {
                    case SupportedProgram.Notepad:
                        StartNotepad(action.ProgramPath, message);
                        break;

                    case SupportedProgram.VsCode:
                        StartVsCode(action.ProgramPath, message);
                        break;

                    case SupportedProgram.Unknown:
                    default:
                        StartGenericProgram(action.ProgramPath);
                        break;
                }

                logger.LogInformation("Proceso {Program} iniciado como tipo {Type}",
                    Path.GetFileName(action.ProgramPath), action.ProgramType);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falló al intentar iniciar el proceso {Program}", action.ProgramPath);
            }
        }

        private void StartNotepad(string programPath, string message)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(),
                $"shadow_pet_note_{Path.GetRandomFileName()}.txt");
            File.WriteAllText(tempFilePath, message);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = programPath,
                Arguments = $"\"{tempFilePath}\"",
                UseShellExecute = true
            };

            Process.Start(processStartInfo);
            logger.LogInformation("Notepad iniciado con archivo temporal {File}", tempFilePath);
        }

        private void StartVsCode(string programPath, string message)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = programPath,
                UseShellExecute = true
            };

            var option = _random.Next(3);

            switch (option)
            {
                case 0: // Mensaje de texto
                    if (!string.IsNullOrEmpty(message))
                    {
                        string tempFilePath = Path.Combine(Path.GetTempPath(),
                            $"shadow_message_{Path.GetRandomFileName()}.txt");
                        File.WriteAllText(tempFilePath, message);
                        processStartInfo.Arguments = $"\"{tempFilePath}\"";
                        logger.LogInformation("VS Code iniciado con mensaje de texto");
                    }
                    else
                    {
                        goto case 2;
                    }
                    break;

                case 1:
                    var tempProgramFile = CodeTemplates.CreateTempProgramFile();
                    processStartInfo.Arguments = $"\"{tempProgramFile}\"";
                    logger.LogInformation("VS Code iniciado con programa de ejemplo");
                    break;

                case 2: // Ventana vacía
                default:
                    processStartInfo.Arguments = "--new-window";
                    logger.LogInformation("VS Code iniciado con ventana vacía");
                    break;
            }

            Process.Start(processStartInfo);
        }

        private void StartGenericProgram(string programPath)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = programPath,
                UseShellExecute = true
            };

            Process.Start(processStartInfo);
            logger.LogInformation("Programa genérico iniciado: {Program}", Path.GetFileName(programPath));
        }

        public void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                logger.LogInformation("URL abierta con éxito: {Url}", url);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falló al intentar abrir la URL {Url}", url);
            }
        }
    }
}
