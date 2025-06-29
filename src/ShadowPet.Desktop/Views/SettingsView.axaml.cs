using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
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

            if (Screens.Primary != null)
            {
                var workArea = Screens.Primary.WorkingArea;

                var scaling = Screens.Primary.Scaling;

                var physicalHeight = this.Bounds.Height * scaling;

                var y = workArea.Bottom - physicalHeight - (10 * scaling);
                var x = workArea.Right - (this.Bounds.Width * scaling) - (20 * scaling);

                this.Position = new PixelPoint((int)x, (int)y);
            }
        }

        private void Window_LostFocus(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
