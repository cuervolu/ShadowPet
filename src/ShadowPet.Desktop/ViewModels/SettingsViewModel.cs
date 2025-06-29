using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadowPet.Core.Models;
using ShadowPet.Core.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace ShadowPet.Desktop.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;

        [ObservableProperty]
        private bool _startWithWindows;

        [ObservableProperty]
        private bool _allowNotifications;

        [ObservableProperty]
        private double _annoyanceLevel;

        [ObservableProperty]
        private double _soundVolume;

        public ObservableCollection<string> AllowedPrograms { get; }

        public SettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;
            var settings = _settingsService.LoadSettings();

            _startWithWindows = settings.StartWithWindows;
            _allowNotifications = settings.AllowNotifications;
            _annoyanceLevel = settings.AnnoyanceLevel;
            _soundVolume = settings.SoundVolume;
            AllowedPrograms = new ObservableCollection<string>(settings.AllowedPrograms);
        }

        [RelayCommand]
        private void SaveSettings()
        {
            var settings = new AppSettings
            {
                HasRunBefore = _settingsService.LoadSettings().HasRunBefore,
                StartWithWindows = StartWithWindows,
                AllowNotifications = AllowNotifications,
                AnnoyanceLevel = AnnoyanceLevel,
                SoundVolume = SoundVolume,
                AllowedPrograms = AllowedPrograms.ToList()
            };
            _settingsService.SaveSettings(settings);
        }
        
        [RelayCommand]
        private void AddProgram()
        {
            AllowedPrograms.Add("C:\\Windows\\System32\\notepad.exe");
        }

        [RelayCommand]
        private void RemoveProgram(string? programPath)
        {
            if (programPath != null)
            {
                AllowedPrograms.Remove(programPath);
            }
        }

        [RelayCommand]
        private void ResetSettings()
        {
            var defaultSettings = new AppSettings();
            StartWithWindows = defaultSettings.StartWithWindows;
            AllowNotifications = defaultSettings.AllowNotifications;
            AnnoyanceLevel = defaultSettings.AnnoyanceLevel;
            SoundVolume = defaultSettings.SoundVolume;
            AllowedPrograms.Clear();
        }
    }
}