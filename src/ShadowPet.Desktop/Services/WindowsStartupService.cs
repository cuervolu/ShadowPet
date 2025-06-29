using Microsoft.Win32;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace ShadowPet.Desktop.Services
{
    public class WindowsStartupService(
        ILogger<WindowsStartupService> logger)
    {
        private const string RegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "ShadowPet";

        public void SetStartup(bool startWithWindows)
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true);
                if (key is null)
                {
                    logger.LogWarning("No se pudo abrir la clave del registro para el auto-inicio");
                    return;
                }

                string? executablePath = Assembly.GetEntryAssembly()?.Location;
                if (string.IsNullOrEmpty(executablePath) || !executablePath.EndsWith(".exe"))
                {
                    executablePath = AppContext.BaseDirectory + "ShadowPet.exe";
                }


                if (startWithWindows)
                {
                    key.SetValue(AppName, $"\"{executablePath}\"");
                    logger.LogInformation("Se ha activado el inicio con Windows");
                }
                else
                {
                    key.DeleteValue(AppName, false);
                    logger.LogInformation("Se ha desactivado el inicio con Windows");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al configurar el inicio con Windows");
            }
        }
    }
}
