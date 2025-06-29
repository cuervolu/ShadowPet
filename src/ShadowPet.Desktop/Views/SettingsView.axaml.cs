using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;

namespace ShadowPet.Desktop.Views
{
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            Dispatcher.UIThread.Post(() =>
            {
                if (Screens.Primary != null)
                {
                    var workArea = Screens.Primary.WorkingArea;
                    var scaling = this.Screens.Primary.Scaling;

                    var finalHeight = this.Bounds.Height;
                    var finalWidth = this.Bounds.Width;

                    var y = workArea.Bottom - (finalHeight * scaling) - (10 * scaling);
                    var x = workArea.Right - (finalWidth * scaling) - (20 * scaling);

                    this.Position = new PixelPoint((int)x, (int)y);
                }
            }, DispatcherPriority.Background);
        }

        // private void Window_LostFocus(object? sender, RoutedEventArgs e)
        // {
        //     Close();
        // }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
