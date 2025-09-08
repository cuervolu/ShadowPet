using Avalonia.Threading;
using ShadowPet.Core.Models;
using ShadowPet.Core.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace ShadowPet.Desktop.Services
{
    public class PetBehaviorService
    {
        public event Func<string, string, Task<bool>>? OnConfirmationRequested;
        public event Func<string, Task>? OnAnimationChangeRequested;
        public event Func<string, Task>? OnDialogueRequested;
        public event Action? OnFollowMouseRequested;
        public event Action? OnMoveRequested;

        private readonly Random _random = new();
        private readonly DispatcherTimer _behaviorTimer;
        public PetState CurrentState { get; private set; } = PetState.Moving;
        private bool _isBusy = false;
        private CancellationTokenSource? _behaviorCts;

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
            _behaviorCts?.Cancel(); // Cancel current action when dragging starts
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

            _behaviorCts = new CancellationTokenSource();
            var token = _behaviorCts.Token;

            var settings = _settingsService.LoadSettings();
            var annoyance = settings.AnnoyanceLevel;

            double takeItemChance = settings.AllowProgramExecution ? 5 + (annoyance / 100.0) * 25.0 : 1 + (annoyance / 100.0) * 3.0;
            var openUrlChance = takeItemChance + (5 + (annoyance / 100.0) * 20.0);
            var remainingChance = 100.0 - openUrlChance;
            var demandAttentionChance = openUrlChance + (remainingChance * 0.15);
            var speakChance = demandAttentionChance + (remainingChance * 0.20);
            var moveRandomlyChance = speakChance + (remainingChance * 0.25);
            var followMouseChance = moveRandomlyChance + (remainingChance * 0.20);
            var choice = _random.NextDouble() * 100;

            try
            {
                if (choice < takeItemChance) await TakeItem(token);
                else if (choice < openUrlChance) await OpenAnnoyingUrlAsync(token);
                else if (choice < demandAttentionChance) await DemandAttention(token);
                else if (choice < speakChance) await Speak(token);
                else if (choice < moveRandomlyChance) await MoveRandomly();
                else if (choice < followMouseChance) await FollowMouse(token);
                else await MakeSillyDance(token);
            }
            catch (OperationCanceledException)
            {
                // Action was cancelled, which is fine. The cleanup is in the finally block of each method.
            }
            finally
            {
                _behaviorCts?.Dispose();
                _behaviorCts = null;
            }

            var minInterval = 3;
            var maxInterval = 12;
            var interval = maxInterval - (annoyance / 100.0) * (maxInterval - minInterval);
            _behaviorTimer.Interval = TimeSpan.FromSeconds(_random.Next((int)interval, (int)interval + 4));
        }

        public async Task TriggerPatPat()
        {
            if (CurrentState == PetState.PatPat) return;

            // Cancel any ongoing behavior from the timer
            _behaviorCts?.Cancel();
            _isBusy = true; // Take control

            CurrentState = PetState.PatPat;
            await OnAnimationChangeRequested?.Invoke("patpat");

            // The patting animation itself cannot be interrupted
            await Task.Delay(1500);

            CurrentState = PetState.Moving;
            await OnAnimationChangeRequested?.Invoke("moving");
            _isBusy = false;
        }

        private async Task MakeSillyDance(CancellationToken token)
        {
            _isBusy = true;

            var sillyAnimations = new[] { "victoria_face", "vaquita_jumping", "vaquita" };
            var chosenAnimation = sillyAnimations[_random.Next(sillyAnimations.Length)];

            CurrentState = chosenAnimation.Contains("vaquita") ? PetState.VaquitaDance : PetState.SillyDance;

            await OnAnimationChangeRequested?.Invoke(chosenAnimation);
            try
            {
                await Task.Delay(4000, token);
            }
            catch (OperationCanceledException)
            {
                /* Task was cancelled, exit gracefully */
            }
            finally
            {
                CurrentState = PetState.Moving;
                await OnAnimationChangeRequested?.Invoke("moving");
                _isBusy = false;
            }
        }

        private async Task TakeItem(CancellationToken token)
        {
            var settings = _settingsService.LoadSettings();
            var possibleActions = settings.PetActions.Where(a => !string.IsNullOrWhiteSpace(a.ProgramPath)).ToList();
            if (!possibleActions.Any()) return;

            _isBusy = true;
            CurrentState = PetState.TakingItem;
            try
            {
                if (!settings.AllowProgramExecution)
                {
                    await OnDialogueRequested?.Invoke("Quisiera abrir algo, pero no me dejas...");
                    await OnAnimationChangeRequested?.Invoke("attention");
                    await Task.Delay(3000, token);
                    return; // Exit after showing message
                }

                var action = possibleActions[_random.Next(possibleActions.Count)];
                var message = (action.ProgramType == SupportedProgram.Notepad)
                    ? _annoyingMessagesService.GetAllMessages().FirstOrDefault() ?? ""
                    : _programMessagesService.GetRandomMessageForProgram(action);

                await OnDialogueRequested?.Invoke("¡Je, je! ¿Qué tal un poco de diversión?");
                await OnAnimationChangeRequested?.Invoke("take_item_intro");
                await Task.Delay(1500, token);

                token.ThrowIfCancellationRequested();

                await OnAnimationChangeRequested?.Invoke("take_item_loop");
                await Task.Delay(2000, token);

                token.ThrowIfCancellationRequested();
                _processService.StartProgram(action, message);
                await Task.Delay(1000, token);
            }
            catch (OperationCanceledException)
            {
                /* Task was cancelled, exit gracefully */
            }
            finally
            {
                await OnDialogueRequested?.Invoke("hide");
                CurrentState = PetState.Moving;
                await OnAnimationChangeRequested?.Invoke("moving");
                _isBusy = false;
            }
        }

        private async Task OpenAnnoyingUrlAsync(CancellationToken token)
        {
            var settings = _settingsService.LoadSettings();
            if (settings.AnnoyingUrls == null || !settings.AnnoyingUrls.Any() || OnConfirmationRequested == null) return;

            _isBusy = true;
            CurrentState = PetState.DemandingAttention;
            await OnAnimationChangeRequested?.Invoke("attention");
            try
            {
                var manipulativeMessages = new[] { "¿Puedo...?", "¿Y si hacemos una travesura?", "¡Mira esto! ¿Te atreves?", "Confía en mí, te va a gustar..." };
                var message = manipulativeMessages[_random.Next(manipulativeMessages.Length)];

                var userAgreed = await OnConfirmationRequested.Invoke("Una preguntita...", message);

                token.ThrowIfCancellationRequested();

                if (userAgreed)
                {
                    var annoyingMessages = new[] { "Cagaste", "UN TROYANO CORRAN", "¡AQUÍ VIENE!", "¡AQUÍ VIENE EL CAOS!" };
                    await OnDialogueRequested?.Invoke(annoyingMessages[_random.Next(annoyingMessages.Length)]);
                    var url = settings.AnnoyingUrls[_random.Next(settings.AnnoyingUrls.Count)];
                    _processService.OpenUrl(url);
                    await Task.Delay(2000, token);
                }
                else
                {
                    var complaints = new[] { "¡Qué aburrido!", "Jo, con lo divertido que era...", "Otro día será, supongo.", "Más fome que clase de Duoc UC" };
                    await OnDialogueRequested?.Invoke(complaints[_random.Next(complaints.Length)]);
                    await Task.Delay(3000, token);
                }
            }
            catch (OperationCanceledException)
            {
                /* Task was cancelled, exit gracefully */
            }
            finally
            {
                await OnDialogueRequested?.Invoke("hide");
                CurrentState = PetState.Moving;
                await OnAnimationChangeRequested?.Invoke("moving");
                _isBusy = false;
            }
        }

        private async Task FollowMouse(CancellationToken token)
        {
            _isBusy = true;
            CurrentState = PetState.FollowingMouse;
            await OnAnimationChangeRequested?.Invoke("moving");
            try
            {
                var followDuration = 4000;
                var followStopwatch = Stopwatch.StartNew();
                while (followStopwatch.ElapsedMilliseconds < followDuration)
                {
                    token.ThrowIfCancellationRequested();
                    OnFollowMouseRequested?.Invoke();
                    await Task.Delay(30, token);
                }
            }
            catch (OperationCanceledException)
            {
                /* Task was cancelled, exit gracefully */
            }
            finally
            {
                CurrentState = PetState.Moving;
                _isBusy = false;
            }
        }

        public async Task TriggerSpecificAnimation(string animationName)
        {
            if (_isBusy) return;
            _isBusy = true;

            var previousState = CurrentState;
            CurrentState = PetState.SillyDance;
            await OnAnimationChangeRequested?.Invoke(animationName);
            await Task.Delay(3000); // This is a user-triggered action, so we let it finish
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

        private async Task DemandAttention(CancellationToken token)
        {
            _isBusy = true;
            CurrentState = PetState.DemandingAttention;
            await OnAnimationChangeRequested?.Invoke("attention");
            try
            {
                await Task.Delay(2500, token);
            }
            catch (OperationCanceledException)
            {
                /* Task was cancelled, exit gracefully */
            }
            finally
            {
                CurrentState = PetState.Moving;
                await OnAnimationChangeRequested?.Invoke("moving");
                _isBusy = false;
            }
        }

        private async Task Speak(CancellationToken token)
        {
            _isBusy = true;
            CurrentState = PetState.Speaking;
            await OnAnimationChangeRequested?.Invoke("interacting");
            await OnDialogueRequested?.Invoke("show");
            try
            {
                await Task.Delay(5000, token);
            }
            catch (OperationCanceledException)
            {
                /* Task was cancelled, exit gracefully */
            }
            finally
            {
                await OnAnimationChangeRequested?.Invoke("moving");
                await OnDialogueRequested?.Invoke("hide");
                CurrentState = PetState.Moving;
                _isBusy = false;
            }
        }
    }
}
