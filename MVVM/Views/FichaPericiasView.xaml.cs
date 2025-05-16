using T20FichaComDB.MVVM.ViewModels;

namespace T20FichaComDB.MVVM.Views;

public partial class FichaPericiasView : ContentPage
{
	public FichaPericiasView(PericiasViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PericiasViewModel vm)
        {
            vm.OnViewAppearing();
        }
    }
}