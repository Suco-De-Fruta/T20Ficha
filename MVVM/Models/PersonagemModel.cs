using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using T20FichaComDB.Data.Entities;

namespace T20FichaComDB.MVVM.Models
{
    public partial class PersonagemModel : ObservableObject
    {
        public int MaxNivel = 20;

        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string _nome;

        [ObservableProperty]
        private string _jogadorNome;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NMetade))]
        [NotifyPropertyChangedFor(nameof(Patamar))]
        private int _nivel = 1;

        [ObservableProperty]
        private string? _raca;

        [ObservableProperty]
        private string? _origem;

        [ObservableProperty]
        private string? _classe;

        [ObservableProperty]
        private string? _divindade;

        // ------ ATRIBUTOS ----///
        [ObservableProperty]
        private int? _forca;

        [ObservableProperty]
        private int? _destreza;

        [ObservableProperty]
        private int? _constituicao;

        [ObservableProperty]
        private int? _inteligencia;

        [ObservableProperty]
        private int? _sabedoria;

        [ObservableProperty]
        private int? _carisma;

        // ------ STATUS ---- //
        [ObservableProperty]
        private int _maxPV;

        [ObservableProperty]
        private int _PVatual;

        [ObservableProperty]
        private int _maxPM;

        [ObservableProperty]
        private int _PMatual;

        // ----- PODERES --- //
        [ObservableProperty]
        private ObservableCollection<PoderesData> _poderesRaca = new();

        private Dictionary<string, string> _escolhasDePoderesFeitas = new();

        public Dictionary<string, string> EscolhasDePoderesFeitas
        {
            get => _escolhasDePoderesFeitas;
            set
            {
                if (SetProperty(ref _escolhasDePoderesFeitas, value))
                {
                    EscolhasDePoderesFeitasChanged?.Invoke();
                }
            }
        }

        public event Action? EscolhasDePoderesFeitasChanged;

        [ObservableProperty]
        private ObservableCollection<PoderesData> _poderesClasse = new();

        [ObservableProperty]
        private ObservableCollection<PoderesData> _poderesOrigem = new();

        [ObservableProperty]
        private ObservableCollection<PoderesData> _poderesConcedidos = new();

        [ObservableProperty]
        private ObservableCollection<PoderesData> _poderesGerais = new();

        // ----- COMBATE --- //
        [ObservableProperty]
        private int _defesa;

        // ------- PROPRIEDADE PARA CALCULOS ------- ///

        public int NMetade => _nivel / 2;


        // ----- PERÍCIAS --- //
        //public ObservableCollection<Pericias> Pericias { get; set; } = new();

        //// ------ PLACEHOLDER PARA O INVENTARIO ---- //
        //public ObservableCollection<string> InventarioItens { get; set; } = new();
        //[ObservableProperty] string? _equipArmadura;
        //[ObservableProperty] string? _equipEscudo;
        //[ObservableProperty] string? _equipArma;
        //[ObservableProperty] int _tibares;

        [ObservableProperty]
        private int _penalidadeArmaduraTotal;

        // ------ MAGIAS -------- //
        [ObservableProperty]
        public ObservableCollection<MagiasData> _magiasConhecidas = new();


        // ------- PROPRIEDADE DO PATAMAR --------- // 
        public string Patamar
        {
            get
            {
                if (Nivel >= 17) return "Lenda";
                if (Nivel >= 11) return "Campeão";
                if (Nivel >= 5) return "Veterano";
                return "Iniciante";
            }
        }
        // --------- CONSTRUTOR ---------- //

        public PersonagemModel()
        {
            Nivel = 1;
            MagiasConhecidas = new ObservableCollection<MagiasData>();
            PenalidadeArmaduraTotal = 0;

            PoderesRaca = new ObservableCollection<PoderesData>();

            PoderesClasse = new ObservableCollection<PoderesData>();
            PoderesGerais = new ObservableCollection<PoderesData>();
            PoderesConcedidos = new ObservableCollection<PoderesData>();
        }
    }
}
