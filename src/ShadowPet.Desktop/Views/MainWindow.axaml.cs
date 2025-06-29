using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Rendering;
using ShadowPet.Desktop.ViewModels;

namespace ShadowPet.Desktop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            PointerPressed += OnPointerPressed;

            PointerReleased += OnPointerReleased;
            PointerMoved += OnPointerMoved;
            DataContextChanged += (sender, args) =>
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.PositionChanged += position => Position = position;
                    vm.InitializeScreens(Screens.All);
                    vm.SetWindowSize(Width, Height);
                    vm.SetInitialPosition(Position);
                }
            };
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.UpdatePosition(this.Position);
                vm.StartDragging();
            }
            BeginMoveDrag(e);
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.StopDragging();
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.UpdateMousePosition(((IRenderRoot)this).PointToScreen(e.GetPosition(null)));
            }
        }
    }
}
