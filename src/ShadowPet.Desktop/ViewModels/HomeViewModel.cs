using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadowPet.Core.Services;
using ShadowPet.Desktop.Services;
using System;

namespace ShadowPet.Desktop.ViewModels
{
    public partial class HomeViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;
        private readonly WindowsStartupService _windowsStartupService;

        public event Action? StartPetRequested;

        [ObservableProperty]
        private bool _startWithSystem;

        public HomeViewModel(SettingsService settingsService, WindowsStartupService windowsStartupService)
        {
            _settingsService = settingsService;
            _windowsStartupService = windowsStartupService;

            var settings = _settingsService.LoadSettings();
            _startWithSystem = settings.StartWithWindows;
        }

        [RelayCommand]
        private void StartPet()
        {
            var settings = _settingsService.LoadSettings();
            settings.StartWithWindows = StartWithSystem;
            _settingsService.SaveSettings(settings);

            _windowsStartupService.SetStartup(StartWithSystem);

            StartPetRequested?.Invoke();
        }

        [RelayCommand]
        private void ToggleStartWithSystem(bool isChecked)
        {
            StartWithSystem = isChecked;

            // Opcional: Aplicar inmediatamente o solo cuando se guarde
            // _windowsStartupService.SetStartup(isChecked);
        }
    }
}
