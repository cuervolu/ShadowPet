using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Microsoft.Extensions.DependencyInjection;
using ShadowPet.Core.Services;
using ShadowPet.Desktop.Services;
using ShadowPet.Desktop.ViewModels;
using ShadowPet.Desktop.Views;
using System;
using System.Linq;
namespace ShadowPet.Desktop;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var settingsService = Program.ServiceProvider!.GetRequiredService<SettingsService>();
        var themeService = Program.ServiceProvider!.GetRequiredService<ThemeService>();

        var settings = settingsService.LoadSettings();

        themeService.SetTheme(settings.AppTheme);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            if (settings.HasRunBefore)
            {
                var mainWindowViewModel = Program.ServiceProvider!.GetRequiredService<MainWindowViewModel>();
                var mainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel
                };
                ConfigureTrayIcon(mainWindowViewModel);
                desktop.MainWindow = mainWindow;
                mainWindow.Show();
            }
            else
            {
                var homeViewModel = Program.ServiceProvider!.GetRequiredService<HomeViewModel>();
                var homeView = new HomeView
                {
                    DataContext = homeViewModel
                };
                desktop.MainWindow = homeView;

                homeView.Show();

                homeViewModel.StartPetRequested += () =>
                {
                    var currentSettings = settingsService.LoadSettings();
                    currentSettings.HasRunBefore = true;
                    settingsService.SaveSettings(currentSettings);

                    var mainWindowViewModel = Program.ServiceProvider!.GetRequiredService<MainWindowViewModel>();
                    var mainWindow = new MainWindow
                    {
                        DataContext = mainWindowViewModel
                    };

                    ConfigureTrayIcon(mainWindowViewModel);
                    mainWindow.Show();
                    homeView.Close();
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureTrayIcon(MainWindowViewModel petViewModel)
    {
        var icons = new TrayIcons
        {
            new TrayIcon
            {
                Icon = new(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowPet/Assets/icon.ico")))),
                ToolTipText = "Shadow Pet",
                Menu = new NativeMenu
                {
                    new NativeMenuItem("Hacer una gracia") { Command = petViewModel.DoTrickCommand },
                    new NativeMenuItemSeparator(),
                    new NativeMenuItem("Configuracion") { Command = petViewModel.OpenSettingsCommand },
                    new NativeMenuItem("Salir") { Command = petViewModel.QuitCommand }
                }
            }
        };
        TrayIcon.SetIcons(this, icons);
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
