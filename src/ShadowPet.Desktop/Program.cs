using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ShadowPet.Core.Services;
using ShadowPet.Desktop.Services;
using ShadowPet.Desktop.ViewModels;
using System;
using System.IO;
using Velopack;

namespace ShadowPet.Desktop;

sealed class Program
{
    public static IServiceProvider? ServiceProvider { get; private set; }
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        VelopackApp.Build().Run();
        // Configure services
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
        Log.Information("Iniciando Shadow Pet...");
        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Fatal(e, "La aplicación murió inesperadamente");
        }
        finally
        {
            Log.CloseAndFlush();
            (ServiceProvider as IDisposable)?.Dispose();
        }
    }


    private static void ConfigureServices(IServiceCollection services)
    {
        var paths = new AppPaths();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .WriteTo.File(
                path: Path.Combine(paths.LogFilePath),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();

        services.AddLogging(builder => builder.AddSerilog());
        services.AddSingleton<AnnoyingMessagesService>();
        services.AddSingleton<ProcessService>();
        services.AddSingleton<ProgramMessagesService>();
        services.AddSingleton<UpdateService>();
        services.AddSingleton<AppPaths>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<WindowsStartupService>();
        services.AddSingleton<AnimationService>();
        services.AddSingleton<DialogueService>();
        services.AddSingleton<AudioService>();
        services.AddSingleton<PetBehaviorService>();

        // ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<UpdateViewModel>();
    }



    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
