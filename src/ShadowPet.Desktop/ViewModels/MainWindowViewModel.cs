using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;
using ShadowPet.Core.Models;
using ShadowPet.Core.Services;
using ShadowPet.Desktop.Services;
using ShadowPet.Desktop.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Velopack;

namespace ShadowPet.Desktop.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly AnimationService _animationService;
        private readonly PetBehaviorService _behaviorService;
        private readonly DialogueService _dialogueService;
        private readonly AudioService _audioService;

        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly UpdateService _updateService;
        private readonly DispatcherTimer _updateCheckTimer;

        private readonly DispatcherTimer _animationTimer;
        private PetAnimation? _currentAnimation;
        private Bitmap? _spriteSheet;
        private int _currentFrame;
        private readonly Stopwatch _stopwatch = new();
        public event Action<PixelPoint>? PositionChanged;
        private readonly Random _random = new();
        private IReadOnlyList<Screen> _screens;
        private double _windowWidth;
        private double _windowHeight;
        private PixelPoint _mouseScreenPosition;
        private readonly DispatcherTimer _moveTimer;
        private PixelPoint _currentPosition;
        private PixelPoint _targetPosition;
        private const double MoveSpeed = 0.05;
        private SettingsView? _settingsView;
        [ObservableProperty]
        private CroppedBitmap? _petSprite;

        [ObservableProperty]
        private bool _isDialogueVisible;

        [ObservableProperty]
        private string? _dialogueText;

        public ICommand DoTrickCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand QuitCommand { get; }
        public ICommand CheckForUpdatesCommand { get; }

        public MainWindowViewModel(AnimationService animationService, PetBehaviorService behaviorService, DialogueService dialogueService, AudioService audioService, ILogger<MainWindowViewModel> logger, UpdateService updateService)
        {
            _animationService = animationService;
            _behaviorService = behaviorService;
            _dialogueService = dialogueService;
            _audioService = audioService;
            _logger = logger;
            _updateService = updateService;

            _moveTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(16), DispatcherPriority.Normal, OnMoveTick);
            _animationTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(16), DispatcherPriority.Normal, OnAnimationTick);

            _behaviorService.OnAnimationChangeRequested += SetAnimation;
            _behaviorService.OnDialogueRequested += HandleDialogueRequest;
            _behaviorService.OnMoveRequested += HandleMoveRequest;
            _behaviorService.OnFollowMouseRequested += HandleFollowMouseRequest;

            DoTrickCommand = new RelayCommand(() => _behaviorService.TriggerSpecificAnimation("victoria_face"));
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            QuitCommand = new RelayCommand(QuitApplication);
            CheckForUpdatesCommand = new RelayCommand(async () => await _updateService.CheckForUpdates());
            _updateService.UpdateAvailable += OnUpdateAvailable;

            SetAnimation("moving");
            _behaviorService.Start();
            _moveTimer.Start();
            _updateCheckTimer = new DispatcherTimer(TimeSpan.FromHours(2), DispatcherPriority.Background, async (s, e) => await _updateService.CheckForUpdates());
            _updateCheckTimer.Start();

            // ShowFakeUpdateModalForTesting();
        }

        // private async Task ShowFakeUpdateModalForTesting()
        // {
        //     await Task.Delay(500);
        //
        //     var fakeUpdateAsset = new VelopackAsset {
        //         PackageId = "dev.cuervolu.ShadowPet",
        //         Version = new SemanticVersion(1, 2, 3),
        //         NotesMarkdown = "### Novedades\n\n- Se corrigio un bug que hacia que Shadow se robara tus contrasenas.\n- Ahora es 20% mas molesto.\n- Se anadieron mas dialogos sin sentido para que no te sientas solo."
        //     };
        //     var fakeUpdateInfo = new UpdateInfo(fakeUpdateAsset,false);
        //
        //     OnUpdateAvailable(fakeUpdateInfo);
        // }


        private async void OnUpdateAvailable(UpdateInfo updateInfo)
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var updateView = new UpdateView();
                updateView.DataContext = new UpdateViewModel(updateView, updateInfo);

                var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

                var owner = lifetime?.Windows.OfType<MainWindow>().FirstOrDefault(w => w.IsActive);
                var result = await updateView.ShowDialog<bool?>(owner);

                if (result == true)
                {
                    _logger.LogInformation("Usuario acepto la actualizacion. Descargando...");
                    _ = _updateService.DownloadAndApplyUpdates(updateInfo);
                }
                else
                {
                    _logger.LogInformation("Usuario omitio la actualizacion v{Version}", updateInfo.TargetFullRelease.Version);
                }
            });
        }

        private void OpenSettings()
        {
            if (_settingsView != null)
            {
                _settingsView.Activate();
                return;
            }

            _settingsView = new SettingsView
            {
                DataContext = Program.ServiceProvider!.GetRequiredService<SettingsViewModel>()
            };

            _settingsView.Closed += (sender, args) => _settingsView = null;

            _settingsView.Show();
        }

        private void QuitApplication()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                lifetime.Shutdown();
            }
        }

        public void SetInitialPosition(PixelPoint position)
        {
            _currentPosition = position;
            _targetPosition = position;
        }

        public void UpdatePosition(PixelPoint newPosition)
        {
            _currentPosition = newPosition;
            _targetPosition = newPosition;
        }


        private void OnMoveTick(object? sender, EventArgs e)
        {
            if (_behaviorService.CurrentState == PetState.Dragging)
            {
                return;
            }

            var deltaX = _targetPosition.X - _currentPosition.X;
            var deltaY = _targetPosition.Y - _currentPosition.Y;

            if (Math.Abs(deltaX) < 1 && Math.Abs(deltaY) < 1)
            {
                return;
            }

            _currentPosition = new PixelPoint(
                (int)(_currentPosition.X + deltaX * MoveSpeed),
                (int)(_currentPosition.Y + deltaY * MoveSpeed)
            );

            PositionChanged?.Invoke(_currentPosition);
        }


        public void SetWindowSize(double width, double height)
        {
            _windowWidth = width;
            _windowHeight = height;
        }

        public void UpdateMousePosition(PixelPoint screenPosition)
        {
            _mouseScreenPosition = screenPosition;
        }

        private void HandleFollowMouseRequest() // Seguir al mouse
        {
            if (_windowWidth == 0 || _windowHeight == 0) return;

            _targetPosition = new PixelPoint(
                (int)(_mouseScreenPosition.X - (_windowWidth / 2)),
                (int)(_mouseScreenPosition.Y - (_windowHeight / 2))
            );
        }


        private async Task HandleDialogueRequest(string command)
        {
            if (command == "hide")
            {
                IsDialogueVisible = false;
                DialogueText = null;
            }
            else if (command == "show")
            {
                DialogueText = _dialogueService.GetRandomDialogue();
                IsDialogueVisible = true;
                _audioService.PlayRandomSpeakSound();
            }
            else
            {
                DialogueText = command;
                IsDialogueVisible = true;
                _audioService.PlayRandomSpeakSound();
            }
        }

        public void StartDragging() => _behaviorService.HandleDragStart();
        public void StopDragging() => _behaviorService.HandleDragEnd();

        public async Task SetAnimation(string name)
        {
            var newAnimation = _animationService.GetAnimation(name);
            if (newAnimation == null || newAnimation.Name == _currentAnimation?.Name)
            {
                return;
            }

            _currentAnimation = newAnimation;
            _currentFrame = 0;

            var assets = AssetLoader.Open(new Uri($"avares://ShadowPet{_currentAnimation.SpriteSheetPath}"));
            _spriteSheet = new Bitmap(assets);

            _animationTimer.Start();
            _stopwatch.Restart();
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            if (_currentAnimation == null || _spriteSheet == null) return;

            var elapsedMs = _stopwatch.ElapsedMilliseconds;
            var frameDuration = _currentAnimation.FrameCount > 0 ? (float)_currentAnimation.DurationMs / _currentAnimation.FrameCount : 0;
            var nextFrame = frameDuration > 0 ? (int)(elapsedMs / frameDuration) : 0;

            if (!_currentAnimation.IsLooping && nextFrame >= _currentAnimation.FrameCount)
            {
                _animationTimer.Stop();
                return;
            }

            _currentFrame = _currentAnimation.FrameCount > 0 ? nextFrame % _currentAnimation.FrameCount : 0;

            var frameWidth = _currentAnimation.FrameCount > 0 ? (int)_spriteSheet.Size.Width / _currentAnimation.FrameCount : (int)_spriteSheet.Size.Width;
            var frameHeight = (int)_spriteSheet.Size.Height;
            var frameRect = new PixelRect(_currentFrame * frameWidth, 0, frameWidth, frameHeight);

            PetSprite = new CroppedBitmap(_spriteSheet, frameRect);
        }

        public void InitializeScreens(IReadOnlyList<Screen> screens)
        {
            _screens = screens;
        }


        private void HandleMoveRequest()
        {
            if (_screens is null || _screens.Count == 0) return;

            var primaryScreen = _screens.FirstOrDefault(s => s.IsPrimary) ?? _screens[0];
            var workingArea = primaryScreen.WorkingArea;

            _targetPosition = new PixelPoint(
                _random.Next(workingArea.X, workingArea.Width - (int)_windowWidth),
                _random.Next(workingArea.Y, workingArea.Height - (int)_windowHeight)
            );
        }

        public void Dispose()
        {
            _animationTimer.Stop();
            _spriteSheet?.Dispose();
            PetSprite?.Dispose();
            _audioService.Dispose();
            _behaviorService.Stop();
            GC.SuppressFinalize(this);
        }
    }
}
