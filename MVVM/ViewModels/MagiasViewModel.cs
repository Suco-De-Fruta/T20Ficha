using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using T20FichaComDB.Data.Entities;
using T20FichaComDB.MVVM.Models;
using T20FichaComDB.MVVM.Views.Popup;
using T20FichaComDB.Services;

namespace T20FichaComDB.MVVM.ViewModels
{
    public partial class MagiasViewModel : ObservableObject
    {
        private readonly DataService _dataService;
        private PersonagemModel _personagem;

        // ---- PROPRIEDADES DE CD ----
        public ObservableCollection<string> AtributosChave { get; } = new() { "Força", "Destreza", "Constituição", "Inteligência", "Sabedoria", "Carisma" };

        [ObservableProperty]
        private string _atributoChaveSelecionado = "Inteligência";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CDCalculado))]
        private int _bonusEquip;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CDCalculado))]
        private int _bonusOutros;

        public int CDCalculado => CalcularCD();

        // ------ MAGIAS POR CICULO
        public ObservableCollection<MagiasData> MagiasCirculo1 { get; } = new();
        public ObservableCollection<MagiasData> MagiasCirculo2 { get; } = new();
        public ObservableCollection<MagiasData> MagiasCirculo3 { get; } = new();
        public ObservableCollection<MagiasData> MagiasCirculo4 { get; } = new();
        public ObservableCollection<MagiasData> MagiasCirculo5 { get; } = new();

        // ----- CONSTRUTOR
        public MagiasViewModel (DataService dataService)
        {
            _dataService = dataService;

        }

        public void Initialize (PersonagemModel personagem)
        {
            string idPersonagem = personagem != null ? personagem.GetHashCode().ToString() : "NULL";
            System.Diagnostics.Debug.WriteLine($"--- MagiasViewModel Initialize chamado com Personagem ID: {idPersonagem} ---");

            if (_personagem != null)
            {
                if (_personagem.MagiasConhecidas != null)
                {
                    _personagem.MagiasConhecidas.CollectionChanged -= MagiasConhecidas_CollectionChanged;
                }

                _personagem.PropertyChanged -= Personagem_PropertyChanged;

            }

            _personagem = personagem;

            if (_personagem != null)
            {
                _personagem.MagiasConhecidas.CollectionChanged += MagiasConhecidas_CollectionChanged;
                _personagem.PropertyChanged += Personagem_PropertyChanged;
                FiltrarMagiasConhecidas();
                OnPropertyChanged(nameof(CDCalculado));
            }
            else
            {
                MagiasCirculo1.Clear();
                MagiasCirculo2.Clear();
                MagiasCirculo3.Clear();
                MagiasCirculo4.Clear();
                MagiasCirculo5.Clear();
                OnPropertyChanged(nameof(CDCalculado));
            }
        }

        private void Personagem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (AtributosChave.Any(a => a.Equals(e.PropertyName, StringComparison.OrdinalIgnoreCase)))
            {
                OnPropertyChanged(nameof(CDCalculado));
            }
        }

        private void MagiasConhecidas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FiltrarMagiasConhecidas();
        }

        private int CalcularCD()
        {
            if (_personagem == null) return 10;

            int modAtributo = 0;
            switch (AtributoChaveSelecionado?.ToLower())
            {
                case "força": modAtributo = CalculaMod(_personagem.Forca ?? 0); break;
                case "destreza": modAtributo = CalculaMod(_personagem.Destreza ?? 0); break;
                case "constituição": modAtributo = CalculaMod(_personagem.Constituicao ?? 0); break;
                case "inteligência": modAtributo = CalculaMod(_personagem.Inteligencia ?? 0); break;
                case "sabedoria": modAtributo = CalculaMod(_personagem.Sabedoria ?? 0); break;
                case "carisma": modAtributo = CalculaMod(_personagem.Carisma ?? 0); break;
            }

            return 10 + modAtributo + (_personagem.NMetade) + BonusEquip + BonusOutros;
        }

        private int CalculaMod(int atributoValor)
        {
            return (int)Math.Floor((atributoValor - 10) / 2.0);
        }

        private void FiltrarMagiasConhecidas()
        {
            MagiasCirculo1.Clear();
            MagiasCirculo2.Clear();
            MagiasCirculo3.Clear();
            MagiasCirculo4.Clear();
            MagiasCirculo5.Clear();

            if (_personagem == null) return;

            var todasMagias = _personagem.MagiasConhecidas.ToList();

            foreach (var magia in todasMagias.OrderBy(m => m.Nome))
            {
                switch (magia.Circulo)
                {
                    case 1: MagiasCirculo1.Add(magia); break;
                    case 2: MagiasCirculo2.Add(magia); break;
                    case 3: MagiasCirculo3.Add(magia); break;
                    case 4: MagiasCirculo4.Add(magia); break;
                    case 5: MagiasCirculo5.Add(magia); break;
                }
            }
        }

        [RelayCommand]
        private async Task MostrarSelecaoMagia(int circulo)
        {
            if (_personagem == null)
            System.Diagnostics.Debug.WriteLine($"Abrir seleção para Círculo {circulo}");

            var popupViewModel = new SelecaoMagiasPopupViewModel(
                _dataService,
                circulo,
                _personagem.MagiasConhecidas
            );

            var popup = new SelecaoMagiasPopupView( popupViewModel );
            await Shell.Current.ShowPopupAsync( popup );
            System.Diagnostics.Debug.WriteLine($"Popup de seleção para Círculo {circulo} fechado.");
            return;
        }


        [RelayCommand]
        private async Task MostrarDetalhesMagia (MagiasData magia)
        {
            if (magia == null || _personagem == null) return;

            System.Diagnostics.Debug.WriteLine($"Mostrar detalhes/remover: {magia.Nome}");

            var popupViewModel = new SelecaoMagiasPopupViewModel(
                magia,
                _personagem.MagiasConhecidas
            );

            var popup = new SelecaoMagiasPopupView(popupViewModel);
            await Shell.Current.ShowPopupAsync(popup);
            System.Diagnostics.Debug.WriteLine($"Popup de detalhes para {magia.Nome} fechado.");
        }
    }
}
