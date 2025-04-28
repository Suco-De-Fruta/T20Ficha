namespace T20FichaComDB.MVVM.Views.Popup;
using CommunityToolkit.Mvvm;
using T20FichaComDB.MVVM.ViewModels;

public partial class SelecaoMagiasPopupView : ContentPage
{
	public SelecaoMagiasPopupView(SelecaoMagiasPopupViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    public SelecaoMagiaPopup()
    {
        InitializeComponent();
    }
}