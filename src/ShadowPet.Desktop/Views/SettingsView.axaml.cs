using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ShadowPet.Desktop.ViewModels;
namespace ShadowPet.Desktop.Views
{
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            var topLevel = GetTopLevel(this);
            if (this.DataContext is SettingsViewModel vm)
            {
                vm.InitializeNotificationManager(topLevel);
            }
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
