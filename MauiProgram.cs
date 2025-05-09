using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using T20FichaComDB.MVVM.ViewModels;
using T20FichaComDB.MVVM.Views;
using T20FichaComDB.Services;
using T20FichaComDB.MVVM.Views.Popups;

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
            builder.Services.AddSingleton<PoderesViewModel>();

            builder.Services.AddTransient<MagiasViewModel>();
            builder.Services.AddTransient<SelecaoMagiasPopupViewModel>();
            builder.Services.AddTransient<DetalhesPoderesPopupViewModel>();

            // --- Registros de Views ---
            builder.Services.AddTransient<FichaPart1View>();
            builder.Services.AddTransient<FichaMagiasView>();
            builder.Services.AddTransient<FichaPoderesView>();

            // --- Registros de Popups (Views) ---
            builder.Services.AddTransient<SelecaoMagiasPopupView>();
            builder.Services.AddTransient<AtributosLivresPopupView>();
            builder.Services.AddTransient<DetalhesPoderesPopupView>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
