using T20FichaComDB.MVVM.Models;
using T20FichaComDB.MVVM.ViewModels;

namespace T20FichaComDB.MVVM.Views;


public partial class FichaPart1View : ContentPage
{

    public FichaPart1View(PersonagemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
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

            if (vm.Personagem == null && vm.Personagem.Id > 0)
            {
                System.Diagnostics.Debug.WriteLine($"Personagem ID {vm.Personagem.Id} já está carregado no ViewModel. Pulando carregamento em OnAppearing.");
                return;
            }

            System.Diagnostics.Debug.WriteLine("Nenhum personagem carregado no ViewModel. Tentando carregar o último salvo...");

            int? ultimoId = await vm.GetPersonagemMaisRecenteIdAsync();

            if (ultimoId.HasValue && ultimoId.Value > 0)
            {
                System.Diagnostics.Debug.WriteLine($"Encontrado último ID: {ultimoId.Value}. Carregando....");
                await vm.CarregarPersonagemAsync(ultimoId.Value);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Nenhum personagem encontrado no banco de dados. Preparando para novo personagem.");
                if (vm.Personagem == null || vm.Personagem.Id != 0)
                {
                    vm.Personagem = new PersonagemModel();
                }
            }
        }
    }
}