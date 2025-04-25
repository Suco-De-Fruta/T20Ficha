namespace T20FichaComDB.MVVM.Views;
using T20FichaComDB.MVVM.ViewModels;

public partial class FichaPart1View : ContentPage
{
    public FichaPart1View(PersonagemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Define o BindingContext
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PersonagemViewModel vm)
        {
            if (!vm.RacasDisponiveis.Any())
            {
                await vm.InitializeAsync();
            }
            // await vm.CarregarPersonagemAsync(idDoPersonagem);
        }
    }
}