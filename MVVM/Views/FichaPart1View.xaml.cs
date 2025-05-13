using System.Diagnostics;
using T20FichaComDB.MVVM.Models;
using T20FichaComDB.MVVM.ViewModels;

namespace T20FichaComDB.MVVM.Views;

public partial class FichaPart1View : ContentPage
{
    private bool _isInitialLoadDone = false;

    public FichaPart1View(PersonagemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("[FichaPart1View] OnAppearing iniciado.");

        if (BindingContext is not PersonagemViewModel vm)
        {
            Debug.WriteLine("[FichaPart1View] Erro: ViewModel não é do tipo PersonagemViewModel.");
            return;
        }

        // Chama o InitializeAsync do ViewModel, que agora deve coordenar
        if (!vm.ClassesDisponiveis.Any() || !vm.RacasViewModel.RacasDisponiveisNomes.Any()) // Verifica se os dados básicos precisam ser carregados
        {
            Debug.WriteLine("[FichaPart1View] Dados básicos (classes, raças, etc.) não carregados. Chamando vm.InitializeAsync().");
            await vm.InitializeAsync();
        }

        if (vm.Personagem != null && (vm.Personagem.Id > 0 || !string.IsNullOrEmpty(vm.Personagem.Nome)))
        {
            Debug.WriteLine($"[FichaPart1View] Personagem '{vm.Personagem.Nome}' (ID: {vm.Personagem.Id}) já está no ViewModel. Pulando carregamento inicial de personagem.");

            _isInitialLoadDone = true; // Marca que o setup inicial (pelo menos das listas) foi feito.
            return;
        }

        if (!_isInitialLoadDone) // Executa apenas na primeira vez ou se resetado
        {
            Debug.WriteLine("[FichaPart1View] Tentando carregar o último personagem salvo...");
            int? ultimoId = await vm.GetPersonagemMaisRecenteIdAsync();

            if (ultimoId.HasValue && ultimoId.Value > 0)
            {
                Debug.WriteLine($"[FichaPart1View] Último ID encontrado: {ultimoId.Value}. Carregando personagem...");
                await vm.CarregarPersonagemAsync(ultimoId.Value);
            }
            else
            {
                Debug.WriteLine("[FichaPart1View] Nenhum personagem salvo encontrado ou ID inválido. Preparando para um novo personagem.");

                if (vm.Personagem == null || vm.Personagem.Id != 0) // Se for nulo ou um personagem carregado anteriormente
                {
                    Debug.WriteLine("[FichaPart1View] Resetando para um novo PersonagemModel.");
                    vm.Personagem = new PersonagemModel();
                                                                                             
                }
                // Garante que o RacasViewModel reflita "nenhuma raça selecionada" para um novo personagem.
                if (vm.RacasViewModel.RacaSelecionadaDetalhes != null)
                {
                    Debug.WriteLine("[FichaPart1View] Limpando seleção de raça no RacasViewModel para novo personagem.");
                    await vm.RacasViewModel.SelecionarRacaAsync(null);
                }
            }
            _isInitialLoadDone = true;
        }
        Debug.WriteLine("[FichaPart1View] OnAppearing finalizado.");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Debug.WriteLine("[FichaPart1View] OnDisappearing.");
    }
}