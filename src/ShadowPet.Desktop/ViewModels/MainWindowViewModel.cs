using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ShadowPet.Core.Models;
using ShadowPet.Core.Services;
using ShadowPet.Desktop.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowPet.Desktop.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly AnimationService _animationService;
        private readonly PetBehaviorService _behaviorService;
        private readonly DialogueService _dialogueService;
        private readonly AudioService _audioService;

        private readonly DispatcherTimer _animationTimer;
        private PetAnimation? _currentAnimation;
        private Bitmap? _spriteSheet;
        private int _currentFrame;
        private readonly Stopwatch _stopwatch = new();
        public event Action<PixelPoint>? PositionChanged;
        private readonly Random _random = new();
        private IReadOnlyList<Screen> _screens;

        [ObservableProperty]
        private CroppedBitmap? _petSprite;

        [ObservableProperty]
        private bool _isDialogueVisible;

        [ObservableProperty]
        private string? _dialogueText;

        public MainWindowViewModel(AnimationService animationService, PetBehaviorService behaviorService, DialogueService dialogueService, AudioService audioService)
        {
            _animationService = animationService;
            _behaviorService = behaviorService;
            _dialogueService = dialogueService;
            _audioService = audioService;

            _animationTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(16), DispatcherPriority.Normal, OnAnimationTick);

            _behaviorService.OnAnimationChangeRequested += SetAnimation;
            _behaviorService.OnDialogueRequested += HandleDialogueRequest;
            _behaviorService.OnMoveRequested += HandleMoveRequest;
            // ...
            SetAnimation("idle");
            _behaviorService.Start();
        }

        private async Task HandleDialogueRequest(string command)
        {
            if (command == "show")
            {
                DialogueText = _dialogueService.GetRandomDialogue();
                IsDialogueVisible = true;
                _audioService.PlayRandomSpeakSound();
            }
            else
            {
                IsDialogueVisible = false;
                DialogueText = null;
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

            var assets = AssetLoader.Open(new Uri($"avares://ShadowPet.Desktop{_currentAnimation.SpriteSheetPath}"));
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
                SetAnimation("idle");
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

            var targetX = _random.Next(0, workingArea.Width);
            var targetY = _random.Next(0, workingArea.Height);

            PositionChanged?.Invoke(new PixelPoint(targetX, targetY));
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
