using CommunityToolkit.Mvvm;
using T20FichaComDB.MVVM.ViewModels.Popups;
using CommunityToolkit.Maui.Views;

namespace T20FichaComDB.MVVM.Views.Popups;

public partial class SelecaoMagiasPopupView : Popup
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