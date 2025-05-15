using T20FichaComDB.MVVM.Views;
using T20FichaComDB.MVVM.Views.Popups;

namespace T20FichaComDB
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(FichaPart1View), typeof(FichaPart1View));
            Routing.RegisterRoute(nameof(FichaMagiasView), typeof(FichaMagiasView));
            Routing.RegisterRoute(nameof(FichaPoderesView), typeof(FichaPoderesView));
            Routing.RegisterRoute(nameof(SelecaoMagiasPopupView), typeof(SelecaoMagiasPopupView));
            Routing.RegisterRoute(nameof(AtributosLivresPopupView), typeof(AtributosLivresPopupView));
            Routing.RegisterRoute(nameof(DetalhesPoderesPopupView), typeof(DetalhesPoderesPopupView));
            Routing.RegisterRoute(nameof(SelecaoOpcaoPoderesPopupView), typeof(SelecaoOpcaoPoderesPopupView));
        }
    }
}
