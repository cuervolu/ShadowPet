using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using ShadowPet.Core.Models;
using System;
using System.Threading.Tasks;
namespace ShadowPet.Desktop.Services
{
    public class PetBehaviorService
    {
        public event Func<string, Task>? OnAnimationChangeRequested;
        public event Func<string, Task>? OnDialogueRequested;
        public event Action<PixelPoint>? OnPositionChangeRequested;
        public event Action? OnMoveRequested;

        private readonly Random _random = new();
        private readonly DispatcherTimer _behaviorTimer;
        private PetState _currentState = PetState.Idle;
        private bool _isBusy = false;

        public PetBehaviorService()
        {
            _behaviorTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5),
                IsEnabled = true
            };
            _behaviorTimer.Tick += OnBehaviorTick;
        }

        public void Start() => _behaviorTimer.Start();
        public void Stop() => _behaviorTimer.Stop();

        public async Task HandleDragStart()
        {
            _currentState = PetState.Dragging;
            await OnAnimationChangeRequested?.Invoke("dragging");
        }

        public async Task HandleDragEnd()
        {
            _currentState = PetState.Idle;
            await OnAnimationChangeRequested?.Invoke("idle");
        }

        private async void OnBehaviorTick(object? sender, EventArgs e)
        {
            if (_currentState != PetState.Idle) return;

            var choice = _random.Next(100);

            if (choice < 25)
            {
                await DemandAttention();
            }
            else if (choice < 50)
            {
                await Speak();
            }
            else if (choice < 80)
            {
                await MoveRandomly();
            }

            _behaviorTimer.Interval = TimeSpan.FromSeconds(_random.Next(5, 15));
        }

        private async Task MoveRandomly()
        {
            _currentState = PetState.Moving;
            await OnAnimationChangeRequested?.Invoke("moving");

            OnMoveRequested?.Invoke();

            await Task.Delay(3000);

            _currentState = PetState.Idle;
            await OnAnimationChangeRequested?.Invoke("idle");
        }

        private async Task DemandAttention()
        {
            _isBusy = true;
            _currentState = PetState.DemandingAttention;
            await OnAnimationChangeRequested?.Invoke("attention");

            await Task.Delay(4000);

            _currentState = PetState.Idle;
            await OnAnimationChangeRequested?.Invoke("idle");
            _isBusy = false;
        }

        private async Task Speak()
        {
            _isBusy = true;
            _currentState = PetState.Speaking;

            await OnAnimationChangeRequested?.Invoke("interacting");
            await OnDialogueRequested?.Invoke("show");

            await Task.Delay(5000);

            await OnAnimationChangeRequested?.Invoke("idle");
            await OnDialogueRequested?.Invoke("hide");

            _currentState = PetState.Idle;
            _isBusy = false;
        }
    }
}

