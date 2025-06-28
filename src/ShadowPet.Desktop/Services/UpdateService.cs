using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Velopack;
using Velopack.Sources;

namespace ShadowPet.Desktop.Services
{
    public class UpdateService
    {
        public event Action<UpdateInfo> UpdateAvailable;
        public event Action NoUpdateAvailable;
        public event Action<int> UpdateDownloadProgress;
        public event Action UpdateReadyToInstall;

        private readonly ILogger<UpdateService> _logger;
        private readonly UpdateManager _updateManager;

        public UpdateService(ILogger<UpdateService> logger)
        {
            _logger = logger;
            var source = new GithubSource("https://github.com/cuervolu/ShadowPet", null, false);
            _updateManager = new UpdateManager(source);
        }

        public bool IsUpdatePendingRestart => _updateManager.IsUpdatePendingRestart;

        public async Task CheckForUpdates()
        {
            try
            {
                var updateInfo = await _updateManager.CheckForUpdatesAsync();
                if (updateInfo != null)
                {
                    _logger.LogInformation("Actualizacion disponible: {Version}", updateInfo.TargetFullRelease.Version);
                    UpdateAvailable?.Invoke(updateInfo);
                }
                else
                {
                    _logger.LogInformation("No hay actualizaciones disponibles");
                    NoUpdateAvailable?.Invoke();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo al buscar actualizaciones");
                NoUpdateAvailable?.Invoke();
            }
        }

        public async Task DownloadAndApplyUpdates(UpdateInfo updateInfo)
        {
            try
            {
                await _updateManager.DownloadUpdatesAsync(updateInfo, p => UpdateDownloadProgress?.Invoke(p));

                _updateManager.ApplyUpdatesAndRestart(updateInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo al descargar o aplicar actualizaciones");
            }
        }
    }
}
