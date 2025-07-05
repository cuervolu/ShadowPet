using Avalonia.Controls;
using Avalonia.Interactivity;
namespace ShadowPet.Desktop.Views
{
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
