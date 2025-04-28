using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using T20FichaComDB.Data.Entities;
using T20FichaComDB.Services;
using T20FichaComDB.MVVM.ViewModels;
using T20FichaComDB.MVVM.Views;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;

namespace T20FichaComDB.MVVM.ViewModels
{
    public partial class SelecaoMagiasPopupViewModel : ObservableObject
    {
        private readonly DataService _dataService;
        private readonly ObservableCollection<MagiasData> _magiasConhecidasPersonagem;
        private readonly Action _fecharPopupAction;

        [ObservableProperty]
        private bool _isAddingMode;

        [ObservableProperty]
        private MagiasData _magiaParaExibirRemover;

        [ObservableProperty]
        private int _circuloParaAdicionar;

        [ObservableProperty]
        private ObservableCollection<MagiasData> _magiasDisponiveis = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConfirmarAdicaoCommand))]
        private MagiasData _magiaSelecionada;

        [ObservableProperty]
        private string _tituloPopup;

        // ---- CONSTRUTOR ADICIONAR
        public SelecaoMagiasPopupViewModel(
            DataService dataService,
            int circulo,
            ObservableCollection<MagiasData> magiasConhecidasOrigem,
            Action fecharPopupAction)

        {
            _dataService = dataService;
            _circuloParaAdicionar = circulo;
            _magiasConhecidasPersonagem = magiasConhecidasOrigem;
            _fecharPopupAction = fecharPopupAction;
            IsAddingMode = true;
            TituloPopup = $"{circulo}º Círculo - Selecionar Magia";

            LoadMagiasDisponiveisAsync();
        }

        public SelecaoMagiasPopupViewModel(
            MagiasData magia,
            ObservableCollection<MagiasData> magiasConhecidasOrigem,
            Action fecharPopupAction)
        {
            _magiasConhecidasPersonagem = magiasConhecidasOrigem;
            _fecharPopupAction = fecharPopupAction;
            IsAddingMode = false;
            TituloPopup = $"Detalhes: {magia?.Nome}";
        }

        private async Task LoadMagiasDisponiveisAsync()
        {
            if (!_isAddingMode || _dataService == null) return;

            try
            {
                var todasMagias = await _dataService.GetMagiasAsync();

                var magiasFiltradas = todasMagias
                    .Where(m => m.Circulo == _circuloParaAdicionar)
                    .Where(m => !_magiasConhecidasPersonagem.Any(mc => mc.Id == m.Id))
                    .OrderBy(m => m.Nome)
                    .ToList();

                MagiasDisponiveis.Clear();
                foreach (var magia in magiasFiltradas)
                {
                    MagiasDisponiveis.Add(magia);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar magias disponíveis: {ex.Message}");
            }
        }
        
        private bool CanConfirmarAdicao()
        {
            return IsAddingMode && MagiaSelecionada != null;
        }

        [RelayCommand(CanExecute = nameof(CanConfirmarAdicao))]
        private void ConfirmarAdicao()
        {
            if (!CanConfirmarAdicao()) return;
            _magiasConhecidasPersonagem?.Add(MagiaSelecionada);

            _fecharPopupAction.Invoke();
        }

        [RelayCommand]
        private void RemoverMagia()
        {
            if (IsAddingMode || MagiaParaExibirRemover == null) return;

            var magiaParaRemover = _magiasConhecidasPersonagem?.FirstOrDefault(m => m.Id == MagiaParaExibirRemover.Id);
            if (magiaParaRemover != null)
            {
                _magiasConhecidasPersonagem?.Remove(magiaParaRemover);
            }

            _fecharPopupAction?.Invoke();
        }

        [RelayCommand]
        private void FecharPopup()
        {
            _fecharPopupAction?.Invoke();
        }
    }
}
