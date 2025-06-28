using Avalonia.Threading;
using System;
using System.Threading.Tasks;
namespace ShadowPet.Desktop.Services
{
    public class PetBehaviorService
    {
        public event Func<string, Task>? OnAnimationChangeRequested;
        public event Func<string, Task>? OnDialogueRequested;

        private readonly Random _random = new();
        private readonly DispatcherTimer _behaviorTimer;
        private bool _isBusy = false;

        public PetBehaviorService()
        {
            _behaviorTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10),
                IsEnabled = true
            };
            _behaviorTimer.Tick += OnBehaviorTick;
        }

        public void Start() => _behaviorTimer.Start();
        public void Stop() => _behaviorTimer.Stop();

        public async Task HandleDragStart()
        {
            _isBusy = true;
            await OnAnimationChangeRequested?.Invoke("dragging");
        }

        public async Task HandleDragEnd()
        {
            _isBusy = false;
            await OnAnimationChangeRequested?.Invoke("idle");
        }

        private async void OnBehaviorTick(object? sender, EventArgs e)
        {
            if (_isBusy) return;

            var choice = _random.Next(100);

            if (choice < 25)
            {
                await DemandAttention();
            }
            else if (choice < 50)
            {
                await Speak();
            }

            _behaviorTimer.Interval = TimeSpan.FromSeconds(_random.Next(8, 20));
        }

        private async Task DemandAttention()
        {
            _isBusy = true;
            await OnAnimationChangeRequested?.Invoke("attention");

            await Task.Delay(4000);

            if (_isBusy && OnAnimationChangeRequested?.Target is not null)
            {
                await OnAnimationChangeRequested.Invoke("idle");
            }

            _isBusy = false;
        }

        private async Task Speak()
        {
            _isBusy = true;

            await OnAnimationChangeRequested?.Invoke("interacting");
            await OnDialogueRequested?.Invoke("show");

            await Task.Delay(5000);

            if (_isBusy)
            {
                await OnAnimationChangeRequested?.Invoke("idle");
                await OnDialogueRequested?.Invoke("hide");
            }

            _isBusy = false;
        }
    }
}
