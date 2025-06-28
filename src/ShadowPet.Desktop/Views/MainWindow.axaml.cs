using Avalonia.Controls;
using Avalonia.Input;
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
            DataContextChanged += (sender, args) =>
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.PositionChanged += position => Position = position;
                    vm.InitializeScreens(Screens.All);
                }
            };
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
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
    }
}
