using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using T20FichaComDB.Data.Entities;
using T20FichaComDB.MVVM.Models;
using T20FichaComDB.MVVM.ViewModels.Popups;
using T20FichaComDB.MVVM.Views.Popups;
using T20FichaComDB.Services;

namespace T20FichaComDB.MVVM.ViewModels
{
    public partial class PoderesViewModel : ObservableObject
    {
        private readonly PersonagemViewModel _personagemViewModel;
        private readonly DataService _dataService;
        private PersonagemModel? _personagem;

        public PersonagemModel? Personagem => _personagem;

        // COLEÇÕES LIGADAS AO PERSONAGMEMODEL
        public ObservableCollection<PoderesData> PoderesRaca => _personagem?.PoderesRaca ?? new ObservableCollection<PoderesData>();
        public ObservableCollection<PoderesData> PoderesClasse => _personagem?.PoderesClasse ?? new ObservableCollection<PoderesData>();
        public ObservableCollection<PoderesData> PoderesConcedidos => _personagem?.PoderesConcedidos ?? new ObservableCollection<PoderesData>();
        public ObservableCollection<PoderesData> PoderesGerais => _personagem?.PoderesGerais ?? new ObservableCollection<PoderesData>();
        public ObservableCollection<PoderesData> PoderesOrigem => _personagem?.PoderesOrigem ?? new ObservableCollection<PoderesData>();

        private bool _processandoEscolhasPoder = false;


        public PoderesViewModel(PersonagemViewModel personagemViewModel, DataService dataService)
        {
            _personagemViewModel = personagemViewModel;
            _dataService = dataService;

            // Recebe mudanças no PersonagemModel do PersonagemViewModel
            _personagemViewModel.PropertyChanged += PersonagemViewModel_PropertyChanged;
            AtualizarVinculoRaca(_personagemViewModel.Personagem);
        }

        private void PersonagemViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PersonagemViewModel.Personagem))
            {
                AtualizarVinculoRaca(_personagemViewModel.Personagem);
            }
        }

        private void AtualizarVinculoRaca(PersonagemModel? novoPoderesRaca)
        {
            Debug.WriteLine($"PoderesViewModel.AtualizarVinculoRaca: INÍCIO. novoPoderesRacaModel is {(novoPoderesRaca == null ? "NULL" : "NOT NULL")}. _personagem (antigo) is {(_personagem == null ? "NULL" : "NOT NULL")}.");

            if (_personagem != null)
            {
                if (_personagem.PoderesRaca != null)
                {
                    _personagem.PoderesRaca.CollectionChanged -= PoderesRaca_CollectionChanged;
                }
                _personagem.EscolhasDePoderesFeitasChanged -= EscolhasDePoderesFeitas_Changed;
                OnPropertyChanged(nameof(Personagem));
            }
            else
            {
                OnPropertyChanged(nameof(Personagem));
            }

                _personagem = novoPoderesRaca;

            if (_personagem != null)
            {

                if (_personagem.PoderesRaca == null)
                {
                    Debug.WriteLine("AVISO CRÍTICO: _personagem.PoderesRaca é NULL ao tentar INSCREVER em PoderesViewModel após atribuir novo personagem.");

                    _personagem.PoderesRaca = new ObservableCollection<PoderesData>();
                }
                _personagem.PoderesRaca.CollectionChanged += PoderesRaca_CollectionChanged;
                _personagem.EscolhasDePoderesFeitasChanged += EscolhasDePoderesFeitas_Changed;

                // Notifica a UI para atualizar todos os poderes
                OnPropertyChanged(nameof(PoderesRaca));
                OnPropertyChanged(nameof(PoderesClasse));
                OnPropertyChanged(nameof(PoderesConcedidos));
                OnPropertyChanged(nameof(PoderesGerais));
                OnPropertyChanged(nameof(PoderesOrigem));
            }
            else
            {
                OnPropertyChanged(nameof(PoderesRaca));
                OnPropertyChanged(nameof(PoderesClasse));
                OnPropertyChanged(nameof(PoderesConcedidos));
                OnPropertyChanged(nameof(PoderesGerais));
                OnPropertyChanged(nameof(PoderesOrigem));

                Debug.WriteLine("PoderesViewModel.AtualizarVinculoRaca: novoPoderesRacaModel é NULL. As coleções de poderes foram atualizadas para refletir isso.");
            }
            Debug.WriteLine($"PoderesViewModel.AtualizarVinculoRaca: FIM. _personagem (atual) is {(_personagem == null ? "NULL" : "NOT NULL")}.");
        }

        private void EscolhasDePoderesFeitas_Changed()
        {
            OnPropertyChanged(nameof(PoderesRaca));
        }

        private void PoderesRaca_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PoderesRaca));

            if (e.Action == NotifyCollectionChangedAction.Add     ||
                e.Action == NotifyCollectionChangedAction.Replace ||
                e.Action == NotifyCollectionChangedAction.Reset)
            {
                MainThread.BeginInvokeOnMainThread(async () => await VerificarEPromoverEscolhasDePoderesRacaAsync());
            }
        }
        public async Task VerificarEPromoverEscolhasDePoderesRacaAsync()
        {
            if (_personagem ==  null || !_personagem.PoderesRaca.Any() || _processandoEscolhasPoder)
            {
                return;
            }
            _processandoEscolhasPoder = true;

            try
            {
                var poderesDaRacaAtual = _personagem.PoderesRaca.ToList();

                foreach (var PoderParaEscolher in poderesDaRacaAtual)
                {
                    if (PoderParaEscolher.RequerEscolha &&
                        !_personagem.EscolhasDePoderesFeitas.ContainsKey(PoderParaEscolher.Nome))
                    {
                        Debug.WriteLine($"[PoderesViewModel] Poder '{PoderParaEscolher.Nome}' requer escolha e ainda não foi feita.");

                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            var popupViewModel = new SelecaoOpcaoPoderesPoppupViewModel(PoderParaEscolher, _personagem);
                            var popup = new SelecaoOpcaoPoderesPopupView(popupViewModel);

                            popupViewModel.EscolhaFeita += (escolha) =>
                            {
                                if (!string.IsNullOrEmpty(escolha))
                                {
                                    Debug.WriteLine($"[PoderesViewModel] Usuário escolheu '{escolha}' para o poder '{PoderParaEscolher.Nome}'.");
                                    OnPropertyChanged(nameof(PoderesRaca));
                                }
                                else
                                {
                                    Debug.WriteLine($"[PoderesViewModel] Escolha para '{PoderParaEscolher.Nome}' cancelada ou inválida.");
                                }
                            };
                            await Shell.Current.ShowPopupAsync(popup);
                        });
                    }
                }
            }
            finally
            {
                _processandoEscolhasPoder = false;
            }
        }

        [RelayCommand]
        private async Task MostrarDetalhesPoder(PoderesData? poderes)
        {
            if (poderes == null) return;

            if (poderes.RequerEscolha && _personagem != null && !_personagem.EscolhasDePoderesFeitas.ContainsKey(poderes.Nome))
            {
                Debug.WriteLine($"[PoderesViewModel] Detalhes solicitados para '{poderes.Nome}', mas requer escolha pendente. Solicitando escolha primeiro.");
                await VerificarEPromoverEscolhasDePoderesRacaAsync();
            }

            var popupViewModel = new DetalhesPoderesPopupViewModel(poderes);

            if (_personagem != null && _personagem.EscolhasDePoderesFeitas.TryGetValue(poderes.Nome, out var escolhaFeita))
            {
                Debug.WriteLine($"[PoderesViewModel] Mostrando detalhes para '{poderes.Nome}' com escolha já feita: '{escolhaFeita}'.");
            }
            var popup = new DetalhesPoderesPopupView(popupViewModel);
            await Shell.Current.ShowPopupAsync(popup);
        }
    }
}
