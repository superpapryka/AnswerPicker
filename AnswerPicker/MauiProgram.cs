using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using AnswerPicker.Views;

namespace AnswerPicker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Rejestracja serwisów
        builder.Services.AddSingleton<Services.FileService>();
        // Rejestrujemy MainPage tak, by DI wstrzykiwało FileService
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}