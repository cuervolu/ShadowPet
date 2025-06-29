using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ShadowPet.Core.Services
{
    public class ProcessService(ILogger<ProcessService> logger)
    {

        public void StartProcessWithText(string programPath, string text)
        {
            if (string.IsNullOrWhiteSpace(programPath) || !File.Exists(programPath))
            {
                logger.LogWarning("Se intentó iniciar un proceso con una ruta inválida: {path}", programPath);
                return;
            }

            try
            {
                string tempFilePath = Path.Combine(Path.GetTempPath(), $"shadow_pet_note_{Path.GetRandomFileName()}.txt");
                File.WriteAllText(tempFilePath, text);

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = programPath,
                    Arguments = $"\"{tempFilePath}\"",
                    UseShellExecute = true
                };

                Process.Start(processStartInfo);
                logger.LogInformation("Proceso {Program} iniciado con el archivo temporal {File}", Path.GetFileName(programPath), tempFilePath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falló al intentar iniciar el proceso {Program}", programPath);
            }
        }
    }
}
