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
        public event Action? OnFollowMouseRequested;
        public event Action? OnMoveRequested;

        private readonly Random _random = new();
        private readonly DispatcherTimer _behaviorTimer;
        private PetState _currentState = PetState.Moving;
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
            _currentState = PetState.Moving;
            await OnAnimationChangeRequested?.Invoke("moving");
        }

        private async void OnBehaviorTick(object? sender, EventArgs e)
        {
            if (_isBusy || (_currentState != PetState.Idle && _currentState != PetState.Moving)) return;

            var choice = _random.Next(100);

            if (choice < 15)
            {
                await DemandAttention();
            }
            else if (choice < 30)
            {
                await Speak();
            }
            else if (choice < 50)
            {
                await MoveRandomly();
            }
            else if (choice < 65)
            {
                await FollowMouse();
            }
            else if (choice < 80)
            {
                await MakeSillyDance();
            }
            else if (choice < 95)
            {
                await TakeItem();
            }

            _behaviorTimer.Interval = TimeSpan.FromSeconds(_random.Next(5, 12));
        }

        private async Task MakeSillyDance()
        {
            _isBusy = true;
            _currentState = PetState.SillyDance;
            await OnAnimationChangeRequested?.Invoke("victoria_face");

            await Task.Delay(3000);

            _currentState = PetState.Moving;
            await OnAnimationChangeRequested?.Invoke("moving");
            _isBusy = false;
        }

        private async Task TakeItem()
        {
            _isBusy = true;
            _currentState = PetState.TakingItem;

            await OnAnimationChangeRequested?.Invoke("take_item_intro");
            await Task.Delay(1500);

            if (_currentState == PetState.TakingItem)
            {
                await OnAnimationChangeRequested?.Invoke("take_item_loop");
                await Task.Delay(3000);
            }

            _currentState = PetState.Moving;
            await OnAnimationChangeRequested?.Invoke("moving");
            _isBusy = false;
        }

        private async Task FollowMouse()
        {
            _isBusy = true;
            _currentState = PetState.FollowingMouse;
            await OnAnimationChangeRequested?.Invoke("moving");

            var followDuration = 4000;
            var followStopwatch = System.Diagnostics.Stopwatch.StartNew();

            while (followStopwatch.ElapsedMilliseconds < followDuration)
            {
                OnFollowMouseRequested?.Invoke();
                await Task.Delay(30);
            }

            _currentState = PetState.Moving;
            _isBusy = false;
        }

        public async Task TriggerSpecificAnimation(string animationName)
        {
            if (_isBusy) return;

            _isBusy = true;


            var previousState = _currentState;
            _currentState = PetState.SillyDance;

            await OnAnimationChangeRequested?.Invoke(animationName);

            await Task.Delay(3000);

            _currentState = previousState;
            await OnAnimationChangeRequested?.Invoke("moving");
            _isBusy = false;
        }


        private async Task MoveRandomly()
        {
            _currentState = PetState.Moving;
            await OnAnimationChangeRequested?.Invoke("moving");

            OnMoveRequested?.Invoke();
        }

        private async Task DemandAttention()
        {
            _isBusy = true;
            _currentState = PetState.DemandingAttention;
            await OnAnimationChangeRequested?.Invoke("attention");

            await Task.Delay(2500);

            _currentState = PetState.Moving;
            await OnAnimationChangeRequested?.Invoke("moving");
            _isBusy = false;
        }

        private async Task Speak()
        {
            _isBusy = true;
            _currentState = PetState.Speaking;

            await OnAnimationChangeRequested?.Invoke("interacting");
            await OnDialogueRequested?.Invoke("show");

            await Task.Delay(5000);

            await OnAnimationChangeRequested?.Invoke("moving");
            await OnDialogueRequested?.Invoke("hide");

            _currentState = PetState.Moving;
            _isBusy = false;
        }
    }
}

