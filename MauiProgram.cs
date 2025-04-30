using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using T20FichaComDB.MVVM.ViewModels;
using T20FichaComDB.MVVM.Views;
using T20FichaComDB.Services;

namespace T20FichaComDB
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<DataService>(); 
            builder.Services.AddSingleton<PersonagemViewModel>(); 
            builder.Services.AddTransient<MagiasViewModel>(); 
            builder.Services.AddTransient<FichaMagiasView>();
            builder.Services.AddTransient<SelecaoMagiasPopupViewModel>();
            builder.Services.AddTransient<FichaPart1View>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
