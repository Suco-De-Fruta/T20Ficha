using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using T20FichaComDB.Data.Entities;
using T20FichaComDB.Services;

namespace T20FichaComDB.MVVM.ViewModels.Popups
{
    public partial class SelecaoMagiasPopupViewModel : ObservableObject
    {
        private readonly DataService _dataService;
        private readonly ObservableCollection<MagiasData> _magiasConhecidasPersonagem;
        public event Action RequestClose;

        [ObservableProperty]
        private bool _isAddingMode;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PodeAdicionar))]
        [NotifyPropertyChangedFor(nameof(PodeRemover))]
        [NotifyPropertyChangedFor(nameof(MostrarDetalhes))]
        private MagiasData? _magiaEmDetalhe;

        [ObservableProperty]
        private MagiasData _magiaParaExibirRemover;

        [ObservableProperty]
        private int _circuloParaAdicionar;

        [ObservableProperty]
        private ObservableCollection<MagiasData> _magiasDisponiveis = new();

        [ObservableProperty]
        private MagiasData _magiaSelecionada;

        [ObservableProperty]
        private string _tituloPopup;

        [ObservableProperty]
        private MagiasData? _magiaSelecionadaNaLista;

        // ---- CONSTRUTOR ADICIONAR
        public SelecaoMagiasPopupViewModel(
            DataService dataService,
            int circulo,
            ObservableCollection<MagiasData> magiasConhecidasOrigem)

        {
            _dataService = dataService;
            _circuloParaAdicionar = circulo;
            _magiasConhecidasPersonagem = magiasConhecidasOrigem;
            IsAddingMode = true;
            TituloPopup = $"{circulo}º Círculo - Selecionar Magia";
            MagiaEmDetalhe = null;

            _ = LoadMagiasDisponiveisAsync();
        }

        public SelecaoMagiasPopupViewModel(MagiasData magia, ObservableCollection<MagiasData> magiasConhecidasOrigem)
        {
            _magiasConhecidasPersonagem = magiasConhecidasOrigem;
            MagiaEmDetalhe = magia;
            IsAddingMode = false;
            TituloPopup = $"Detalhes: {magia?.Nome}";
        }

        private async Task LoadMagiasDisponiveisAsync()
        {
            if (!_isAddingMode || _dataService == null) return;

            var todasMagiasDoBanco = await _dataService.GetMagiasAsync();

            try
            {
                var todasMagias = await _dataService.GetMagiasAsync();
                int magiasConhecidasCount = _magiasConhecidasPersonagem?.Count ?? 0;

                var magiasFiltradas = todasMagias
                    .Where(m => m.Circulo == _circuloParaAdicionar)
                    .Where(m => _magiasConhecidasPersonagem == null || !_magiasConhecidasPersonagem.Any(mc => mc.Id == m.Id))
                    .OrderBy(m => m.Nome)
                    .ToList();

                MagiasDisponiveis.Clear();
                foreach (var magia in magiasFiltradas)
                {
                    MagiasDisponiveis.Add(magia);
                }

                System.Diagnostics.Debug.WriteLine($"--- LoadMagiasDisponiveisAsync: Coleção 'MagiasDisponiveis' finalizada com {MagiasDisponiveis.Count} itens. ---");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"--- ERRO LoadMagiasDisponiveisAsync: Falha ao carregar magias: {ex.ToString()} ---");
            }
        }

        public bool PodeAdicionar => IsAddingMode && MagiaEmDetalhe != null;

        [RelayCommand(CanExecute = nameof(PodeAdicionar))]
        private void ConfirmarAdicao()
        {
            if (!PodeAdicionar || MagiaEmDetalhe == null) return;

            _magiasConhecidasPersonagem?.Add(MagiaEmDetalhe);

            RequestClose?.Invoke();
        }

        [RelayCommand]
        private void SelecionarMagiaParaDetalhe(MagiasData? magiaSelecionada)
        {
            MagiaEmDetalhe = magiaSelecionada;
        }

        public bool PodeRemover => !IsAddingMode && MagiaEmDetalhe != null;

        [RelayCommand (CanExecute = nameof(PodeRemover))]
        private void ConfirmarRemocao()
        {
            if (IsAddingMode || MagiaEmDetalhe == null) return;

            var magiaParaRemover = _magiasConhecidasPersonagem?.FirstOrDefault(m => m.Id == MagiaEmDetalhe.Id);
            if (magiaParaRemover != null)
            {
                _magiasConhecidasPersonagem?.Remove(magiaParaRemover);
            }

            RequestClose?.Invoke();
        }

        public bool MostrarDetalhes => MagiaEmDetalhe != null;

        [RelayCommand]
        private void FecharPopup()
        {
            RequestClose?.Invoke();
        }
    }
}
