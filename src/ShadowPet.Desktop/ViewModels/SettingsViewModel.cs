using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ShadowPet.Core.Models;
using ShadowPet.Core.Services;
using ShadowPet.Core.Utils;
using ShadowPet.Desktop.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace ShadowPet.Desktop.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;
        private readonly ILogger<SettingsViewModel> _logger;
        private readonly ProgramFinderService _programFinderService;
        private readonly NotificationService _notificationService;
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

        [ObservableProperty]
        private string? _programNameToSearch;

        [ObservableProperty]
        private string? _searchStatus;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(FindAndAddProgramCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelFindProgramCommand))]
        private bool _isSearching;

        public bool IsDialogueSelected => !string.IsNullOrEmpty(SelectedDialogue);

        public ObservableCollection<PetAction> PetActions { get; }
        public ObservableCollection<string> CustomDialogues { get; }
        private CancellationTokenSource? _findProgramCts;

        public SettingsViewModel(SettingsService settingsService, WindowsStartupService windowsStartupService, ThemeService themeService, ProgramFinderService programFinderService, NotificationService notificationService, ILogger<SettingsViewModel> logger)
        {
            _settingsService = settingsService;
            _windowsStartupService = windowsStartupService;
            _themeService = themeService;
            _programFinderService = programFinderService;
            _notificationService = notificationService;
            _logger = logger;
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

        public void InitializeNotificationManager(TopLevel topLevel)
        {
            _notificationService.SetHostWindow(topLevel);
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


        [RelayCommand(CanExecute = nameof(CanFindProgram))]
        private async Task FindAndAddProgramAsync()
        {
            if (string.IsNullOrWhiteSpace(ProgramNameToSearch))
            {
                _notificationService.Show("Atención", "Por favor, escribe un nombre de programa.", NotificationType.Warning);
                return;
            }

            _findProgramCts = new CancellationTokenSource();
            IsSearching = true;

            try
            {
                var programPath = await _programFinderService.FindExecutablePathAsync(ProgramNameToSearch, _findProgramCts.Token);

                if (!string.IsNullOrEmpty(programPath))
                {
                    var newAction = new PetAction
                    {
                        /*...*/
                    };
                    PetActions.Add(newAction);
                    _notificationService.Show("Éxito", $"'{newAction.Name}' añadido correctamente.", NotificationType.Success);
                    ProgramNameToSearch = string.Empty;
                }
                else
                {
                    _notificationService.Show("No encontrado", $"No se pudo encontrar '{ProgramNameToSearch}'.", NotificationType.Error);
                }
            }
            catch (OperationCanceledException)
            {
                _notificationService.Show("Cancelado", "La búsqueda ha sido cancelada.");
            }
            catch (Exception ex)
            {
                _notificationService.Show("Error Inesperado", "Ocurrió un error durante la búsqueda.", NotificationType.Error);
                _logger.LogError(ex, "Error al buscar el programa '{ProgramName}'", ProgramNameToSearch);
            }
            finally
            {
                IsSearching = false;
                _findProgramCts?.Dispose();
                _findProgramCts = null;
                FindAndAddProgramCommand.NotifyCanExecuteChanged();
                CancelFindProgramCommand.NotifyCanExecuteChanged();
            }
        }

        private async Task ShowStatusAsync(string message, int durationMs)
        {
            SearchStatus = message;
            await Task.Delay(durationMs);
            if (SearchStatus == message)
            {
                SearchStatus = string.Empty;
            }
        }



        private bool CanFindProgram() => !IsSearching;

        [RelayCommand(CanExecute = nameof(IsSearching))]
        private void CancelFindProgram()
        {
            _findProgramCts?.Cancel();
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
