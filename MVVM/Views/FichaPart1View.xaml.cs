using T20FichaComDB.MVVM.ViewModels;
using T20FichaComDB.Services;

namespace T20FichaComDB.MVVM.Views;


public partial class FichaPart1View : ContentPage
{
    private PersonagemViewModel _viewModel;

    public FichaPart1View()
    {
        InitializeComponent();

        var databaseService = new DataService(); // ou usar Singleton/DI se tiver
        _viewModel = new PersonagemViewModel(databaseService);
        this.BindingContext = _viewModel;
    }
    

    //public FichaPart1View(PersonagemViewModel viewModel)
    //{
    //    InitializeComponent();
    //    BindingContext = viewModel;

    //}

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