using T20FichaComDB.MVVM.ViewModels;
using T20FichaComDB.MVVM.Views;

namespace T20FichaComDB
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(FichaPart1View), typeof(FichaPart1View));

        }
    }
}
