using Microsoft.Extensions.Logging;
using Microsoft.Win32;
namespace ShadowPet.Core.Services
{
    public class ProgramFinderService(
        ILogger<ProgramFinderService> logger)
    {

        public async Task<string?> FindExecutablePathAsync(string programName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(programName)) return null;

            if (!programName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                programName += ".exe";
            }

            logger.LogInformation("Iniciando búsqueda para '{ProgramName}'", programName);

            cancellationToken.ThrowIfCancellationRequested();
            logger.LogDebug("Buscando en el registro...");
            string? path = FindInRegistry(programName);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                logger.LogInformation("Programa encontrado en el registro: '{Path}'", path);
                return path;
            }

            cancellationToken.ThrowIfCancellationRequested();
            logger.LogDebug("Buscando en la variable de entorno PATH...");
            path = FindInPath(programName);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                logger.LogInformation("Programa encontrado en PATH: '{Path}'", path);
                return path;
            }

            cancellationToken.ThrowIfCancellationRequested();
            logger.LogDebug("Buscando en carpetas comunes (operación lenta)...");
            path = await FindInCommonFoldersAsync(programName, cancellationToken);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                logger.LogInformation("Programa encontrado en carpetas comunes: '{Path}'", path);
                return path;
            }

            logger.LogWarning("No se encontró el programa '{ProgramName}' después de todas las búsquedas", programName);
            return null;
        }

        private string? FindInRegistry(string programName)
        {
            try
            {
                string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\" + programName;
                using (var key = Registry.LocalMachine.OpenSubKey(registryKey))
                {
                    if (key != null)
                    {
                        object? defaultValue = key.GetValue(null);
                        if (defaultValue != null)
                        {
                            return defaultValue.ToString()?.Trim('"');
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al acceder al registro para buscar '{ProgramName}'", programName);
            }
            return null;
        }

        private string? FindInPath(string programName)
        {
            var pathVariable = Environment.GetEnvironmentVariable("PATH");
            if (pathVariable == null) return null;

            var paths = pathVariable.Split(';');
            foreach (var path in paths)
            {
                try
                {
                    string fullPath = Path.Combine(path, programName);
                    if (File.Exists(fullPath))
                    {
                        return fullPath;
                    }
                }
                catch (Exception)
                {
                    /* Ignorar rutas inválidas en PATH */
                }
            }
            return null;
        }

        private async Task<string?> FindInCommonFoldersAsync(string programName, CancellationToken cancellationToken)
        {
            var commonFolders = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            };

            foreach (var folder in commonFolders.Where(Directory.Exists))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var result = await Task.Run(() => SearchDirectoryRecursive(folder, programName, cancellationToken), cancellationToken);
                if (!string.IsNullOrEmpty(result)) return result;
            }
            return null;
        }

        private string? SearchDirectoryRecursive(string directory, string programName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var files = Directory.GetFiles(directory, programName);
                if (files.Any()) return files.FirstOrDefault();

                foreach (var subDirectory in Directory.GetDirectories(directory))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var result = SearchDirectoryRecursive(subDirectory, programName, cancellationToken);
                    if (!string.IsNullOrEmpty(result)) return result;
                }
            }
            catch (OperationCanceledException) { throw; }
            catch (UnauthorizedAccessException) { logger.LogWarning("Acceso denegado a la carpeta: '{Directory}'", directory); }
            catch (Exception ex) { logger.LogError(ex, "Error inesperado buscando en la carpeta: '{Directory}'", directory); }

            return null;
        }
    }
}
