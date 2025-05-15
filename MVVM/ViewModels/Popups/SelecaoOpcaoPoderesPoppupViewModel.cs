using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using T20FichaComDB.Data.Entities;
using T20FichaComDB.MVVM.Models;

namespace T20FichaComDB.MVVM.ViewModels.Popups
{
    public partial class SelecaoOpcaoPoderesPoppupViewModel : ObservableObject
    {
        [ObservableProperty]
        private PoderesData _poderParaEscolha;

        [ObservableProperty]
        private string _promptEscolha;

        [ObservableProperty]
        private ObservableCollection<string> _opcoesDisponiveis;

        [ObservableProperty]
        private string _opcaoSelecionada;

        private PersonagemModel _personagemModel;

        public event Action<string?>? EscolhaFeita;
        public event Action? RequestClose;

        public SelecaoOpcaoPoderesPoppupViewModel(PoderesData poderEscolha, PersonagemModel personagem)
        {
            PoderParaEscolha = poderEscolha;
            _personagemModel = personagem;
            PromptEscolha = poderEscolha.DescricaoDaEscolha;
            OpcoesDisponiveis = new ObservableCollection<string>(poderEscolha.ListaOpcoesDisponiveis);

            // Tenta pré-selecionar se a escolha já foi feita
            if (_personagemModel.EscolhasDePoderesFeitas.TryGetValue(PoderParaEscolha.Nome, out var escolhaSalva))
            {
                OpcaoSelecionada = escolhaSalva;
            }
        }

        [RelayCommand]
        private void ConfirmarEscolha()
        {
            if (PoderParaEscolha != null && !string.IsNullOrWhiteSpace(OpcaoSelecionada))
            {
                _personagemModel.EscolhasDePoderesFeitas[PoderParaEscolha.Nome] = OpcaoSelecionada;
                EscolhaFeita?.Invoke(OpcaoSelecionada);
            }
            else
            {
                EscolhaFeita?.Invoke(null);
            }
            RequestClose?.Invoke();
        }

        [RelayCommand]
        private void CancelarEscolha()
        {
            EscolhaFeita?.Invoke(null);
            RequestClose?.Invoke();
        }
    }
}
