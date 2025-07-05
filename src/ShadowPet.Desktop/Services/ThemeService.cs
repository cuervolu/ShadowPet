using Avalonia;
using Avalonia.Styling;
namespace ShadowPet.Desktop.Services
{
    public class ThemeService
    {
        public void SetTheme(string? themeName)
        {
            if (Application.Current != null)
            {
                Application.Current.RequestedThemeVariant = themeName switch
                {
                    "Dark" => ThemeVariant.Dark,
                    "Light" => ThemeVariant.Light,
                    _ => ThemeVariant.Default
                };
            }
        }
    }
}
