using T20FichaComDB.MVVM.ViewModels;

namespace T20FichaComDB.MVVM.Views;

public partial class FichaMagiasView : ContentPage
{
	private readonly MagiasViewModel _viewModel;
    private readonly PersonagemViewModel _personagemViewModel;

    public FichaMagiasView(MagiasViewModel viewModel, PersonagemViewModel personagemViewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		_personagemViewModel = personagemViewModel;
		BindingContext = _viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
        System.Diagnostics.Debug.WriteLine("--- FichaMagiasView OnAppearing Iniciado ---");

        var personagemAtual = _personagemViewModel?.Personagem;

        if (_personagemViewModel.Personagem != null)
		{
            System.Diagnostics.Debug.WriteLine($"OnAppearing: Personagem '{personagemAtual.Nome}' (ID:{personagemAtual.GetHashCode()}) encontrado.");

            System.Diagnostics.Debug.WriteLine("OnAppearing: Chamando _viewModel.Initialize...");

            _viewModel.Initialize(personagemAtual);

            System.Diagnostics.Debug.WriteLine("OnAppearing: _viewModel.Initialize chamado.");
        }
		else
		{
            System.Diagnostics.Debug.WriteLine("ERRO: PersonagemModel nulo ao tentar inicializar MagiasViewModel.");
            Shell.Current.DisplayAlert("Erro", "Não foi possível carregar os dados do personagem para as magias.", "OK");
        }

        System.Diagnostics.Debug.WriteLine("--- FichaMagiasView OnAppearing Finalizado ---");

    }

}
