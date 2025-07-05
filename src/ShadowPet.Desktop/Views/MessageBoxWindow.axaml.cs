using Avalonia.Controls;
using Avalonia.Interactivity;
namespace ShadowPet.Desktop.Views
{
    public partial class MessageBoxWindow : Window
    {
        public MessageBoxWindow()
        {
            InitializeComponent();
        }

        public MessageBoxWindow(string title, string message)
        {
            InitializeComponent();
            this.FindControl<TextBlock>("TitleTextBlock")!.Text = title;
            this.FindControl<TextBlock>("MessageTextBlock")!.Text = message;
        }

        private void YesButton_Click(object? sender, RoutedEventArgs e)
        {
            Close(true);
        }

        private void NoButton_Click(object? sender, RoutedEventArgs e)
        {
            Close(false);
        }
    }
}
