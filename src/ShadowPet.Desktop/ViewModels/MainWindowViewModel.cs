using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        public ICommand PatPatCommand { get; }

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
            _behaviorService.OnConfirmationRequested += ShowConfirmationDialogAsync;

            DoTrickCommand = new RelayCommand(() => _behaviorService.TriggerSpecificAnimation("victoria_face"));
            PatPatCommand = new RelayCommand(async () => await _behaviorService.TriggerPatPat());
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            QuitCommand = new RelayCommand(QuitApplication);
            CheckForUpdatesCommand = new RelayCommand(async () => await _updateService.CheckForUpdates());
            _updateService.UpdateAvailable += OnUpdateAvailable;

            SetAnimation("moving");
            _behaviorService.Start();
            _moveTimer.Start();
            _updateCheckTimer = new DispatcherTimer(TimeSpan.FromHours(2), DispatcherPriority.Background, (s, e) => _ = CheckForUpdatesAsync());
            _updateCheckTimer.Start();

            _ = CheckForUpdatesAsync();

            // ShowFakeUpdateModalForTesting();
        }


        private async Task CheckForUpdatesAsync()
        {
            try
            {
                _logger.LogInformation("Buscando actualizaciones...");
                await _updateService.CheckForUpdates();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico durante la comprobación de actualizaciones desde el ViewModel");
            }
        }


        private async Task<bool> ShowConfirmationDialogAsync(string title, string message)
        {
            var dialog = new MessageBoxWindow(title, message);

            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var owner = lifetime?.Windows.OfType<MainWindow>().FirstOrDefault();

            if (owner != null)
            {
                return await dialog.ShowDialog<bool>(owner);
            }

            return await dialog.ShowDialog<bool>(new Window());
        }

//         private async Task ShowFakeUpdateModalForTesting()
//         {
//             await Task.Delay(500);
//
//             // El contenido completo de tu changelog.md
//             var changelogContent = """
//                                    ## **Shadow Pet: "¡Ahora Hago Más Cosas!"**
//
//                                    He estado trabajando arduamente (y bajo la atenta y caótica mirada de Shadow Milk Cookie) para traer una oleada de mejoras a tu mascota de escritorio favorita. Esto es todo lo nuevo:
//
//                                    ### **Mejoras de Calidad de Vida (Para que no desinstales la app y me pegues)**
//
//                                    -   **Implementado el Botón de "Por favor, para":** Se ha añadido una configuración crucial en la nueva pestaña de "Comportamiento". Ahora puedes desmarcar la casilla "Permitir que abra programas y URLs". Si lo haces, Shadow ya no abrirá cosas al azar. En su lugar, se quejará de vez en quando con un aire de decepción adorable.
//                                    -   **Control de Volumen (Porque a veces grita):** En la pestaña "General", ahora hay un deslizador de "Volumen de Sonido". Úsalo para decidir si los susurros de Shadow son un murmullo de fondo o el evento principal de tu día.
//                                    -   **Zona de Confort (Ya no se sale de la pantalla xd):** He puesto límites. Shadow ya no podrá esconderse cobardemente debajo de la barra de tareas o en los rincones más oscuros de tu monitor. Ahora se mantendrá siempre a la vista, quiera o no el ql.
//
//                                    ### **Shadow Ahora es Más Inteligente (O al menos, más personalizable)**
//                                    -   **¿Qué tan caótico lo quieres?:** El deslizador "Nivel de Molestia" ahora _de verdad_ hace algo. Súbelo para que Shadow sea un torbellino de actividad, abriendo cosas y moviéndose sin parar. Bájalo para que entre en un estado de letargo casi productivo.
//                                    -   **Guionista Personal:** ¿Cansado de que siempre pregunte por Vainilla? ¡Ahora puedes escribirle sus propios diálogos! En la nueva pestaña de "Personalización", tienes control total sobre su repertorio. Añade, elimina y haz que diga lo que siempre quisiste.
//                                    -   **Modo Oscuro, porque somos programadores:** En la pestaña "General" puedes cambiar el tema de la aplicación entre Claro y Oscuro. Por defecto es oscuro, como el corazón de la galleta.
//
//                                    ### **La Gran Actualización de la Configuración**
//
//                                    -   **Adiós, ventanita esquinera:** La configuración ha madurado. Ya no es un pequeño panel que aparece en la esquina. Ahora es una ventana de verdad, con todas las de la ley, organizada en tres pestañas gloriosas: **General, Comportamiento y Personalización.**
//
//                                    -   **El Buscador de Programas Mágico:** Se acabó el tener que bucear en tus carpetas buscando un `.exe`. En la pestaña "Personalización", ahora solo tienes que escribir el nombre del programa (ej: `chrome.exe`), darle a "Buscar y Añadir", y nuestro nuevo y flamante `ProgramFinderService` hará el trabajo sucio.
//
//                                        -   **Búsqueda a tres niveles:** Primero mira en el registro (el lugar de los profesionales), luego en el PATH (el lugar de los valientes) y, si todo falla, se pone a escanear tus carpetas de `Program Files` como un becario desesperado.
//                                        -   **A prueba de errores (casi):** El buscador ahora es lo suficientemente listo como para no morir si se encuentra con una carpeta a la que no tiene permiso de acceso. Simplemente la ignora y sigue con su vida.
//                                        -   **Botón de Pánico (Cancelar):** Si la búsqueda se está demorando mucho, ahora hay un bonito botón rojo de "Cancelar" para que puedas detenerla. Y lo más importante: ahora sí funciona.
//
//                                    Espero que disfrutes de estas mejoras. He puesto mucho esfuerzo para que esta mascota sea el compañero de escritorio perfecto: adorable, un poco molesto y altamente personalizable.
//                                    """;
//
//             var fakeUpdateAsset = new VelopackAsset
//             {
//                 PackageId = "dev.cuervolu.ShadowPet",
//                 Version = new SemanticVersion(1, 2, 3),
//                 NotesMarkdown = changelogContent
//             };
//             var fakeUpdateInfo = new UpdateInfo(fakeUpdateAsset, false);
//
//             OnUpdateAvailable(fakeUpdateInfo);
//         }


        private async void OnUpdateAvailable(UpdateInfo updateInfo)
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var updateView = new UpdateView();
                updateView.DataContext = new UpdateViewModel(updateView, updateInfo);

                var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

                var owner = lifetime?.Windows.OfType<MainWindow>().FirstOrDefault() ?? lifetime?.MainWindow;
                if (owner is null)
                {
                    _logger.LogError("No se pudo encontrar una ventana dueña para mostrar el diálogo de actualización. Abortando");
                    return;
                }

                var result = await updateView.ShowDialog<bool?>(owner);

                if (result == true)
                {
                    _logger.LogInformation("Usuario aceptó la actualización. Descargando...");
                    _ = _updateService.DownloadAndApplyUpdates(updateInfo);
                }
                else
                {
                    _logger.LogInformation("Usuario omitió la actualización v{Version}", updateInfo.TargetFullRelease.Version);
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
            const int padding = 10;

            var windowWidth = _windowWidth > 0 ? (int)_windowWidth : 350;
            var windowHeight = _windowHeight > 0 ? (int)_windowHeight : 250;


            var maxX = workingArea.Right - windowWidth - padding;
            var maxY = workingArea.Bottom - windowHeight - padding;

            var minX = workingArea.X + padding;
            var minY = workingArea.Y + padding;

            _targetPosition = new PixelPoint(
                _random.Next(minX, maxX),
                _random.Next(minY, maxY)
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
