using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using ShadowPet.Core.Models;
using ShadowPet.Core.Services;
using ShadowPet.Core.Utils;
using System;
using System.IO;
using System.Linq;
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
        public PetState CurrentState { get; private set; } = PetState.Moving;
        private bool _isBusy = false;

        private readonly SettingsService _settingsService;
        private readonly ProcessService _processService;
        private readonly AnnoyingMessagesService _annoyingMessagesService;
        private readonly ProgramMessagesService _programMessagesService;

        public PetBehaviorService(SettingsService settingsService, ProcessService processService, AnnoyingMessagesService annoyingMessagesService, ProgramMessagesService programMessagesService)
        {
            _settingsService = settingsService;
            _processService = processService;
            _annoyingMessagesService = annoyingMessagesService;
            _programMessagesService = programMessagesService;
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
            CurrentState = PetState.Dragging;
            await OnAnimationChangeRequested?.Invoke("dragging");
        }

        public async Task HandleDragEnd()
        {
            CurrentState = PetState.Moving;
            await OnAnimationChangeRequested?.Invoke("moving");
        }

        private async void OnBehaviorTick(object? sender, EventArgs e)
        {
            if (_isBusy || (CurrentState != PetState.Idle && CurrentState != PetState.Moving)) return;

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
            CurrentState = PetState.SillyDance;
            await OnAnimationChangeRequested?.Invoke("victoria_face");

            await Task.Delay(3000);

            CurrentState = PetState.Moving;
            await OnAnimationChangeRequested?.Invoke("moving");
            _isBusy = false;
        }

   private async Task TakeItem()
        {
            var settings = _settingsService.LoadSettings();
            var possibleActions = settings.PetActions
                .Where(a => !string.IsNullOrWhiteSpace(a.ProgramPath))
                .ToList();

            if (!possibleActions.Any())
            {
                await OnDialogueRequested?.Invoke("Uhm... creo que no tengo ideas.");
                await Task.Delay(2000);
                await OnDialogueRequested?.Invoke("hide");
                return;
            }

            _isBusy = true;
            CurrentState = PetState.TakingItem;

            if (!settings.AllowProgramExecution)
            {
                await OnDialogueRequested?.Invoke("Quisiera abrir algo, pero no me dejas...");
                await OnAnimationChangeRequested?.Invoke("attention");
                await Task.Delay(3000);
                await OnDialogueRequested?.Invoke("hide");
                CurrentState = PetState.Moving;
                await OnAnimationChangeRequested?.Invoke("moving");
                _isBusy = false;
                return;
            }


            var action = possibleActions[_random.Next(possibleActions.Count)];

            if (action.ProgramType == SupportedProgram.Unknown)
            {
                action.ProgramType = ProgramDetector.DetectProgramType(action.ProgramPath);
            }

            string message;
            if (action.ProgramType == SupportedProgram.Notepad)
            {
                var allMessages = _annoyingMessagesService.GetAllMessages();
                message = allMessages[_random.Next(allMessages.Count)];
            }
            else
            {
                message = _programMessagesService.GetRandomMessageForProgram(action);
            }

            await OnDialogueRequested?.Invoke("¡Je, je! ¿Qué tal un poco de diversión?");
            await OnAnimationChangeRequested?.Invoke("take_item_intro");
            await Task.Delay(1500);

            if (CurrentState == PetState.TakingItem)
            {
                await OnAnimationChangeRequested?.Invoke("take_item_loop");
                await Task.Delay(2000);

                _processService.StartProgram(action, message);

                await Task.Delay(1000);
            }

            await OnDialogueRequested?.Invoke("hide");
            CurrentState = PetState.Moving;
            await OnAnimationChangeRequested?.Invoke("moving");
            _isBusy = false;
        }


        private async Task FollowMouse()
        {
            _isBusy = true;
            CurrentState = PetState.FollowingMouse;
            await OnAnimationChangeRequested?.Invoke("moving");

            var followDuration = 4000;
            var followStopwatch = System.Diagnostics.Stopwatch.StartNew();

            while (followStopwatch.ElapsedMilliseconds < followDuration)
            {
                OnFollowMouseRequested?.Invoke();
                await Task.Delay(30);
            }

            CurrentState = PetState.Moving;
            _isBusy = false;
        }

        public async Task TriggerSpecificAnimation(string animationName)
        {
            if (_isBusy) return;

            _isBusy = true;


            var previousState = CurrentState;
            CurrentState = PetState.SillyDance;

            await OnAnimationChangeRequested?.Invoke(animationName);

            await Task.Delay(3000);

            CurrentState = previousState;
            await OnAnimationChangeRequested?.Invoke("moving");
            _isBusy = false;
        }


        private async Task MoveRandomly()
        {
            CurrentState = PetState.Moving;
            await OnAnimationChangeRequested?.Invoke("moving");

            OnMoveRequested?.Invoke();
        }

        private async Task DemandAttention()
        {
            _isBusy = true;
            CurrentState = PetState.DemandingAttention;
            await OnAnimationChangeRequested?.Invoke("attention");

            await Task.Delay(2500);

            CurrentState = PetState.Moving;
            await OnAnimationChangeRequested?.Invoke("moving");
            _isBusy = false;
        }

        private async Task Speak()
        {
            _isBusy = true;
            CurrentState = PetState.Speaking;

            await OnAnimationChangeRequested?.Invoke("interacting");
            await OnDialogueRequested?.Invoke("show");

            await Task.Delay(5000);

            await OnAnimationChangeRequested?.Invoke("moving");
            await OnDialogueRequested?.Invoke("hide");

            CurrentState = PetState.Moving;
            _isBusy = false;
        }
    }
}

