using T20FichaComDB.MVVM.ViewModels;

namespace T20FichaComDB.MVVM.Views;

public partial class FichaMagiasView : ContentPage
{
	private readonly MagiasViewModel _viewModel;
	public FichaMagiasView(MagiasViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

	}
}
