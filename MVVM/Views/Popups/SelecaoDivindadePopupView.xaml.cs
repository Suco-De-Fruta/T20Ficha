using CommunityToolkit.Maui.Views;
using T20FichaComDB.MVVM.ViewModels.Popups;

namespace T20FichaComDB.MVVM.Views.Popups;

public partial class SelecaoDivindadePopupView : Popup
{
	private SelecaoDivindadePopupViewModel _viewModel;

	public SelecaoDivindadePopupView(SelecaoDivindadePopupViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;

        if (_viewModel != null)
        {
            _viewModel.RequestClose += OnRequestClose;
        }

        this.Closed += (s, e) =>
        {
            if (_viewModel != null)
            {
                _viewModel.RequestClose -= OnRequestClose;
            }
        };
    }
    private void OnRequestClose()
    {
        MainThread.BeginInvokeOnMainThread(() => Close());
    }
}