using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadowPet.Core.Models;
using ShadowPet.Core.Services;
using ShadowPet.Core.Utils;
using ShadowPet.Desktop.Services;
using System.Collections.Generic;
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
        private readonly ThemeService _themeService;

        [ObservableProperty]
        private bool _isGeneralExpanded = true;

        [ObservableProperty]
        private bool _isBehaviorExpanded;

        [ObservableProperty]
        private bool _isPersonalizationExpanded;

        [ObservableProperty]
        private bool _startWithWindows;

        [ObservableProperty]
        private bool _allowNotifications;

        [ObservableProperty]
        private bool _allowProgramExecution;

        [ObservableProperty]
        private double _annoyanceLevel;

        [ObservableProperty]
        private double _soundVolume;

        [ObservableProperty]
        private PetAction? _selectedAction;

        [ObservableProperty]
        private string? _newDialogueText;

        [ObservableProperty]
        private string? _selectedDialogue;

        [ObservableProperty]
        private string _selectedTheme;
        public List<string> AvailableThemes { get; } = ["Dark", "Light"];

        public bool IsDialogueSelected => !string.IsNullOrEmpty(SelectedDialogue);

        public ObservableCollection<PetAction> PetActions { get; }
        public ObservableCollection<string> CustomDialogues { get; }

        public SettingsViewModel()
        {

        }

        public SettingsViewModel(SettingsService settingsService, WindowsStartupService windowsStartupService, ThemeService themeService)
        {
            _settingsService = settingsService;
            _windowsStartupService = windowsStartupService;
            _themeService = themeService;
            var settings = _settingsService.LoadSettings();

            _startWithWindows = settings.StartWithWindows;
            _allowNotifications = settings.AllowNotifications;
            _allowProgramExecution = settings.AllowProgramExecution;
            _annoyanceLevel = settings.AnnoyanceLevel;
            _soundVolume = settings.SoundVolume;
            _selectedTheme = settings.AppTheme;
            PetActions = new ObservableCollection<PetAction>(settings.PetActions);
            CustomDialogues = new ObservableCollection<string>(settings.CustomDialogues);
        }


        partial void OnSelectedThemeChanged(string value)
        {
            _themeService.SetTheme(value);
        }


        partial void OnIsGeneralExpandedChanged(bool value)
        {
            if (value)
            {
                IsBehaviorExpanded = false;
                IsPersonalizationExpanded = false;
            }
        }

        partial void OnIsBehaviorExpandedChanged(bool value)
        {
            if (value)
            {
                IsGeneralExpanded = false;
                IsPersonalizationExpanded = false;
            }
        }

        partial void OnIsPersonalizationExpandedChanged(bool value)
        {
            if (value)
            {
                IsGeneralExpanded = false;
                IsBehaviorExpanded = false;
            }
        }


        [RelayCommand]
        private void AddDialogue()
        {
            if (!string.IsNullOrWhiteSpace(NewDialogueText))
            {
                CustomDialogues.Add(NewDialogueText);
                NewDialogueText = string.Empty;
            }
        }

        [RelayCommand]
        private void RemoveDialogue()
        {
            if (!string.IsNullOrEmpty(SelectedDialogue))
            {
                CustomDialogues.Remove(SelectedDialogue);
            }
        }


        [RelayCommand]
        private void SaveSettings()
        {
            var settings = new AppSettings
            {
                HasRunBefore = _settingsService.LoadSettings().HasRunBefore,
                StartWithWindows = StartWithWindows,
                AllowNotifications = AllowNotifications,
                AllowProgramExecution = AllowProgramExecution,
                AnnoyanceLevel = AnnoyanceLevel,
                SoundVolume = SoundVolume,
                PetActions = PetActions.ToList(),
                AppTheme = SelectedTheme,
                CustomDialogues = CustomDialogues.ToList()
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
                Title = "Selecciona un programa",
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
                        ProgramType = ProgramDetector.DetectProgramType(filePath)
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
            _settingsService.LoadSettings();

            StartWithWindows = defaultSettings.StartWithWindows;
            AllowNotifications = defaultSettings.AllowNotifications;
            AllowProgramExecution = defaultSettings.AllowProgramExecution;
            AnnoyanceLevel = defaultSettings.AnnoyanceLevel;
            SoundVolume = defaultSettings.SoundVolume;
            SelectedTheme = defaultSettings.AppTheme;
            PetActions.Clear();
            CustomDialogues.Clear();
            foreach (var dialogue in defaultSettings.CustomDialogues)
            {
                CustomDialogues.Add(dialogue);
            }
        }
    }
}
