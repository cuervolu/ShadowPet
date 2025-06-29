using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadowPet.Core.Models;
using ShadowPet.Core.Services;
using ShadowPet.Desktop.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowPet.Desktop.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;
        private readonly WindowsStartupService _windowsStartupService;
        [ObservableProperty]
        private bool _startWithWindows;

        [ObservableProperty]
        private bool _allowNotifications;

        [ObservableProperty]
        private double _annoyanceLevel;

        [ObservableProperty]
        private double _soundVolume;

        [ObservableProperty]
        private PetAction? _selectedAction;

        public ObservableCollection<PetAction> PetActions { get; }

        public SettingsViewModel(SettingsService settingsService, WindowsStartupService windowsStartupService)
        {
            _settingsService = settingsService;
            _windowsStartupService = windowsStartupService;
            var settings = _settingsService.LoadSettings();

            _startWithWindows = settings.StartWithWindows;
            _allowNotifications = settings.AllowNotifications;
            _annoyanceLevel = settings.AnnoyanceLevel;
            _soundVolume = settings.SoundVolume;
            PetActions = new ObservableCollection<PetAction>(settings.PetActions);
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
                PetActions = PetActions.ToList()
            };
            _settingsService.SaveSettings(settings);
            _windowsStartupService.SetStartup(settings.StartWithWindows);
        }
        
        [RelayCommand]
        private async Task AddAction(Window? owner)
        {
            if (owner is null) return;

            var files = await owner.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Selecciona una wea",
                AllowMultiple = false,
                FileTypeFilter = [FilePickerFileTypes.All]
            });

            if (files.Count >= 1)
            {
                var filePath = files[0].TryGetLocalPath();
                if (!string.IsNullOrEmpty(filePath))
                {
                    var newAction = new PetAction
                    {
                        Name = Path.GetFileNameWithoutExtension(filePath),
                        ProgramPath = filePath,
                    };
                    PetActions.Add(newAction);
                }
            }
        }

        [RelayCommand]
        private void RemoveAction(PetAction? action)
        {
            if (action != null)
            {
                PetActions.Remove(action);
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
            PetActions.Clear();
        }
    }
}