using T20FichaComDB.MVVM.ViewModels;

namespace T20FichaComDB.MVVM.Views;

public partial class FichaPoderesView : ContentPage
{
	public FichaPoderesView(PoderesViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

	protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PoderesViewModel vm)
        {
            System.Diagnostics.Debug.WriteLine($"FichaPoderesView Aparecendo. Personagem no VM de Poderes: {vm.PoderesRaca != null}");
        }
    }
}