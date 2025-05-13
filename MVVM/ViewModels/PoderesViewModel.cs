using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using T20FichaComDB.Data.Entities;
using T20FichaComDB.MVVM.Models;
using T20FichaComDB.MVVM.Views.Popups;
using T20FichaComDB.Services;

namespace T20FichaComDB.MVVM.ViewModels
{
    public partial class PoderesViewModel : ObservableObject
    {
        private readonly PersonagemViewModel _personagemViewModel;
        private readonly DataService _dataService;
        private PersonagemModel? _personagem;

        // COLEÇÕES LIGADAS AO PERSONAGMEMODEL
        public ObservableCollection<PoderesData> PoderesRaca => _personagem?.PoderesRaca ?? new ObservableCollection<PoderesData>();
        public ObservableCollection<PoderesData> PoderesClasse => _personagem?.PoderesClasse ?? new ObservableCollection<PoderesData>();
        public ObservableCollection<PoderesData> PoderesConcedidos => _personagem?.PoderesConcedidos ?? new ObservableCollection<PoderesData>();
        public ObservableCollection<PoderesData> PoderesGerais => _personagem?.PoderesGerais ?? new ObservableCollection<PoderesData>();
        public ObservableCollection<PoderesData> PoderesOrigem => _personagem?.PoderesOrigem ?? new ObservableCollection<PoderesData>();

        public PoderesViewModel(PersonagemViewModel personagemViewModel, DataService dataService)
        {
            _personagemViewModel = personagemViewModel;
            _dataService = dataService;

            _personagemViewModel.PropertyChanged += PersonagemViewModel_PropertyChanged;
            AtualizarPoderesRaca(_personagemViewModel.Personagem);
        }

        private void PersonagemViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PersonagemViewModel.Personagem))
            {
                AtualizarPoderesRaca(_personagemViewModel.Personagem);
            }
        }

        private void AtualizarPoderesRaca(PersonagemModel? poderesRacaPersonagem)
        {
            Debug.WriteLine($"PoderesViewModel.AtualizarPoderesRaca: INÍCIO. poderesRacaPersonagemModel is {(poderesRacaPersonagem == null ? "NULL" : "NOT NULL")}. _personagem (antigo) is {(_personagem == null ? "NULL" : "NOT NULL")}.");

            if (_personagem != null)
            {
                if (_personagem.PoderesRaca != null)
                {
                    _personagem.PoderesRaca.CollectionChanged -= PoderesRaca_CollectionChanged;
                }
            }

            _personagem = poderesRacaPersonagem;

            if (_personagem != null)
            {

                if (_personagem.PoderesRaca == null)
                {
                    Debug.WriteLine("AVISO CRÍTICO: _personagem.PoderesRaca é NULL ao tentar INSCREVER em PoderesViewModel após atribuir novo personagem.");

                    _personagem.PoderesRaca = new ObservableCollection<PoderesData>();
                }
                _personagem.PoderesRaca.CollectionChanged += PoderesRaca_CollectionChanged;

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

                Debug.WriteLine("PoderesViewModel.AtualizarPoderesRaca: poderesRacaPersonagemModel é NULL. As coleções de poderes foram atualizadas para refletir isso.");
            }
            Debug.WriteLine($"PoderesViewModel.AtualizarPoderesRaca: FIM. _personagem (atual) is {(_personagem == null ? "NULL" : "NOT NULL")}.");
        }

        private void PoderesRaca_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PoderesRaca));
        }

        [RelayCommand]
        private async Task MostrarDetalhesPoder(PoderesData? poderes)
        {
            if (poderes == null) return;

            var popupViewModel = new DetalhesPoderesPopupViewModel(poderes);
            var popup = new DetalhesPoderesPopupView(popupViewModel);
            await Shell.Current.ShowPopupAsync(popup);
        }

    }
}
