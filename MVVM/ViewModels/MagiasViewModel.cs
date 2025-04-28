using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Maui.Views;
using T20FichaComDB.Data.Entities;
using CommunityToolkit.Maui.Extensions;
using T20FichaComDB.MVVM.Models;
using T20FichaComDB.MVVM.Views.Popup;
using T20FichaComDB.Services;

namespace T20FichaComDB.MVVM.ViewModels
{
    public partial class MagiasViewModel : ObservableObject
    {
        private readonly DataService _dataService;

        private readonly PersonagemModel _personagem;

        // ---- PROPRIEDADES DE CD ----
        public ObservableCollection<string> AtributosChave { get; } = new() { "Força", "Destreza", "Constituição", "Inteligência", "Sabedoria", "Carisma" };

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CDCalculado))]
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
        public MagiasViewModel (DataService dataService, PersonagemModel personagem)
        {
            _dataService = dataService;
            _personagem = personagem;

            if (_personagem == null)
            {
                _personagem._magiasConhecidas.CollectionChanged += MagiasConhecidas_CollectionChanged;
                FiltrarMagiasConhecidas();
                _personagem.PropertyChanged += Personagem_PropertyChanged;
            }
        }

        public void Initialize (PersonagemModel personagem)
        {
            if (_personagem == null)
            {
                _personagem.MagiasConhecidas.CollectionChanged -= MagiasConhecidas_CollectionChanged;
                _personagem.PropertyChanged -= Personagem_PropertyChanged;
            }

            if (_personagem == null)
            {
                _personagem.MagiasConhecidas.CollectionChanged += MagiasConhecidas_CollectionChanged;
                _personagem.PropertyChanged += Personagem_PropertyChanged;
                FiltrarMagiasConhecidas();
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

            return 10 + modAtributo + BonusEquip + BonusOutros;
        }

        private int CalculaMod(int atributoValor)
        {
            return (atributoValor - 10) / 2;
        }

        private void FiltrarMagiasConhecidas()
        {
            if (_personagem == null) return;

            var todasMagias = _personagem.MagiasConhecidas.ToList();

            MagiasCirculo1.Clear();
            MagiasCirculo2.Clear();
            MagiasCirculo3.Clear();
            MagiasCirculo4.Clear();
            MagiasCirculo5.Clear();

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
            if (_personagem == null) return;
            System.Diagnostics.Debug.WriteLine($"Abrir seleção para Círculo {circulo}");

            var popupViewModel = new SelecaoMagiasPopupViewModel(
                _dataService,
                circulo,
                _personagem.MagiasConhecidas,
                () => Shell.Current.CurrentPage.ClosePopup()
            );

            var popup = new SelecaoMagiasPopupView(popupViewModel);
            await Shell.Current.CurrentPage.ShowPopuoAsync(popup);
        }


        [RelayCommand]
        private async Task MostrarDetalhesMagia (MagiasData magia)
        {
            if (magia == null || _personagem == null) return;

            System.Diagnostics.Debug.WriteLine($"Mostrar detalhes/remover: {magia.Nome}");

            var popupViewModel = new SelecaoMagiasPopupViewModel(
                magia,
                _personagem.MagiasConhecidas,
                () => Shell.Current.CurrentPage.ClosePopup()
            );

            var popup = new SelecaoMagiasPopupView(popupViewModel);
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        }
    }
}
