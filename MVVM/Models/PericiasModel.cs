using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace T20FichaComDB.MVVM.Models
{
    public partial class PericiasModel : ObservableObject
    {
        [ObservableProperty]
        private string _nomePericia;

        [ObservableProperty]
        private string _atributoChavePadrao;

        private string _atributoChaveSelecionado;
        public string AtributoChaveSelecionado
        {
            get => _atributoChaveSelecionado;
            set
            {
                if (SetProperty(ref _atributoChaveSelecionado, value))
                {
                    OnPropertyChanged(nameof(ModAtributoCalculado));
                    RecalcularValorTotal?.Invoke(this);
                }
            }
        }

        [ObservableProperty]
        private bool _soTreinado;

        [ObservableProperty]
        private bool _aplicaPenalidade;

        private bool _treinada;
        public bool Treinada
        {
            get => _treinada;
            set
            {
                if (SetProperty(ref _treinada, value))
                {
                    OnPropertyChanged(nameof(BonusTreinamentoCalculado));
                    RecalcularValorTotal?.Invoke(this);
                }
            }
        }

        private int _outrosBonus;
        public int OutrosBonus
        {
            get => _outrosBonus;
            set
            {
                if (SetProperty(ref _outrosBonus, value))
                {
                    RecalcularValorTotal?.Invoke(this);
                }
            }
        }

        [ObservableProperty]
        private int _valorMetadeNivel;

        [ObservableProperty]
        private int _bonusTreinamentoCalculado;

        [ObservableProperty]
        private int _modAtributoCalculado;

        [ObservableProperty]
        private int _penalidadeAplicada;

        [ObservableProperty]
        private int _valorTotal;

        public ObservableCollection<string> ListaAtributosDisponiveis { get; } = new()
        {
            "FOR", "DES", "CON", "INT", "SAB", "CAR"
        };

        // Evento para solicitar recálculo ao ViewModel
        public event Action<PericiasModel>? RecalcularValorTotal;

        public PericiasModel (string nome, string atributoChavePadrao, bool soTreinado, bool aplicaPenalidade)
        {
            NomePericia = nome;
            AtributoChavePadrao = atributoChavePadrao;
            _atributoChaveSelecionado = atributoChavePadrao;
            SoTreinado = soTreinado;
            AplicaPenalidade = aplicaPenalidade;
            _treinada = false;
            _outrosBonus = 0;
        }
    }
}