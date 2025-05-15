using CommunityToolkit.Maui.Views;
using T20FichaComDB.MVVM.ViewModels.Popups;

namespace T20FichaComDB.MVVM.Views.Popups
{
    public partial class DetalhesPoderesPopupView : CommunityToolkit.Maui.Views.Popup
    {
        public DetalhesPoderesPopupView(DetalhesPoderesPopupViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            if (viewModel != null)
            {
                viewModel.RequestClose += OnRequestClose;
            }
            this.Closed += (s, e) =>
            {
                if (viewModel != null)
                {
                    viewModel.RequestClose -= OnRequestClose;
                }
            };
        }
        private void OnRequestClose()
        {
            MainThread.BeginInvokeOnMainThread(() => Close());
        }
    }
}