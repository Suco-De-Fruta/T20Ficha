using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using T20FichaComDB.Data.Entities;
using T20FichaComDB.Services;
using T20FichaComDB.MVVM.Models;

namespace T20FichaComDB.MVVM.ViewModels
{
    public partial class RacasViewModel : ObservableObject
    {
        private readonly DataService _dataService;

        [ObservableProperty]
        private RacasData? _racaSelecionadaDetalhes;

        [ObservableProperty]
        private ObservableCollection<string> _racasDisponiveisNomes = new();

        [ObservableProperty]
        private ObservableCollection<PoderesData> _poderesDaRacaSelecionada = new();

        private List<RacasData> _todasRacasComDetalhes = new List<RacasData>();

        [ObservableProperty]
        private Dictionary<string, int> _modAtributosRaca = new();

        [ObservableProperty]
        private int _modLivresRaca;

        [ObservableProperty]
        private string _descricaoModLivresRaca = string.Empty;

        [ObservableProperty]
        private string _excecoesModLivresRaca = string.Empty;

        [ObservableProperty]
        string? _nomeRacaSelecionadaPicker;

        public RacasViewModel(DataService dataService)
        {
            _dataService = dataService;
        }

        public async Task InitializeAsync()
        {
            await LoadRacasDisponiveisAsync();
        }
        private async Task LoadRacasDisponiveisAsync()
        {
            if (RacasDisponiveisNomes.Any()) RacasDisponiveisNomes.Clear();
            _todasRacasComDetalhes.Clear();

            var racasDB = await _dataService.GetRacasAsync();
            if (racasDB != null)
            {
                _todasRacasComDetalhes.AddRange(racasDB);
                foreach (var raca in racasDB.OrderBy(r => r.Nome))
                {
                    RacasDisponiveisNomes.Add(raca.Nome);
                }
            }
        }

        [RelayCommand]
        public async Task SelecionarRacaAsync(string? nomeRaca)
        {
            if (RacaSelecionadaDetalhes?.Nome == nomeRaca && nomeRaca != null) // Se a mesma raça não nula já está selecionada
            {
                System.Diagnostics.Debug.WriteLine($"[RacaVM] SelecionarRacaAsync: Raça '{nomeRaca}' já estava selecionada. Nenhuma alteração no evento.");
                return;
            }

            if (string.IsNullOrWhiteSpace(nomeRaca) && RacaSelecionadaDetalhes == null)
            {
                return;
            }

            ModAtributosRaca.Clear();
            PoderesDaRacaSelecionada.Clear();
            ModLivresRaca = 0;
            DescricaoModLivresRaca = null;
            ExcecoesModLivresRaca = null;
            RacaSelecionadaDetalhes = null;

            if (string.IsNullOrWhiteSpace(nomeRaca))
            {
                OnRacaChanged();
                return;
            }

            RacaSelecionadaDetalhes = _todasRacasComDetalhes.FirstOrDefault(r => r.Nome.Equals(nomeRaca, StringComparison.OrdinalIgnoreCase));

            if (RacaSelecionadaDetalhes != null)
            {
                // Preencher os modificadores para o PersonagemViewModel consumir
                ModAtributosRaca["Forca"] = RacaSelecionadaDetalhes.ModForca;
                ModAtributosRaca["Destreza"] = RacaSelecionadaDetalhes.ModDestreza;
                ModAtributosRaca["Constituicao"] = RacaSelecionadaDetalhes.ModConstituicao;
                ModAtributosRaca["Inteligencia"] = RacaSelecionadaDetalhes.ModInteligencia;
                ModAtributosRaca["Sabedoria"] = RacaSelecionadaDetalhes.ModSabedoria;
                ModAtributosRaca["Carisma"] = RacaSelecionadaDetalhes.ModCarisma;

                ModLivresRaca = RacaSelecionadaDetalhes.ModLivres;
                DescricaoModLivresRaca = RacaSelecionadaDetalhes.DescricaoModLivres;
                ExcecoesModLivresRaca = RacaSelecionadaDetalhes.ExcecoesModLivres;

                if (RacaSelecionadaDetalhes.ListaPoderesRacaNomes != null && RacaSelecionadaDetalhes.ListaPoderesRacaNomes.Any())
                {
                    var poderesDb = await _dataService.GetPoderesPorNomeETipoAsync(
                        RacaSelecionadaDetalhes.ListaPoderesRacaNomes, TipoPoderEnum.Raca);
                    if (poderesDb != null)
                    {
                        foreach (var poder in poderesDb.OrderBy(p => p.Nome))
                        {
                            PoderesDaRacaSelecionada.Add(poder);
                        }
                    }
                }
                OnRacaChanged();
            }
        }

        partial void OnNomeRacaSelecionadaPickerChanged(string? oldValue, string? newValue)
        {
            if (oldValue != newValue)
            {
                _ = SelecionarRacaAsync(newValue);
            }
        }

        // Evento para notificar o PersonagemViewModel
        public event EventHandler? RacaChanged;
        protected virtual void OnRacaChanged()
        {
            RacaChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
