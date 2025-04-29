namespace T20FichaComDB.MVVM.Views.Popup;
using CommunityToolkit.Mvvm;
using T20FichaComDB.MVVM.ViewModels;
using CommunityToolkit.Maui.Views;

public partial class SelecaoMagiasPopupView : CommunityToolkit.Maui.Views.Popup
{
    public SelecaoMagiasPopupView(SelecaoMagiasPopupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.Closed += SelecaoMagiasPopupView_Closed;

        if (viewModel != null)
        {
            viewModel.RequestClose += OnRequestClose;
        }
    }

    private void OnRequestClose()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            this.Close();
        });
    }

    private void SelecaoMagiasPopupView_Closed(object? sender, CommunityToolkit.Maui.Core.PopupClosedEventArgs e)
    {
        UnsubscribeEvents();
    }

    private void UnsubscribeEvents()
    {
        this.Closed -= SelecaoMagiasPopupView_Closed;
        if (BindingContext is SelecaoMagiasPopupViewModel vm)
        {
            vm.RequestClose -= OnRequestClose;
        }
    }
}