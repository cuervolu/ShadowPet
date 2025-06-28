using Avalonia;
using Avalonia.Controls;

namespace ShadowPet.Desktop.Views
{
    public partial class DialogueBubble : UserControl
    {
        public static readonly StyledProperty<string?> TextProperty =
            AvaloniaProperty.Register<DialogueBubble, string?>(nameof(Text));

        public string? Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public DialogueBubble()
        {
            InitializeComponent();
        }
    }
}
