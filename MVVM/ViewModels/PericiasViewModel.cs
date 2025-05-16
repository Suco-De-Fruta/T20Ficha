using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using T20FichaComDB.MVVM.Models;

namespace T20FichaComDB.MVVM.ViewModels
{
    public partial class PericiasViewModel : ObservableObject
    {
        private readonly PersonagemViewModel _personagemViewModel;
        private PersonagemModel? _personagem;

        public ObservableCollection<PericiasModel> ListaPericias { get; } = new();

        // Lista para os pickers
        public ObservableCollection<string> AtributosDiponiveis { get; } = new()
        {
            "FOR", "DES", "CON", "INT", "SAB", "CAR"
        };

        public PericiasViewModel (PersonagemViewModel personagemViewModel)
        {
            _personagemViewModel = personagemViewModel;
            _personagemViewModel.PropertyChanged += PersonagemViewModel_PropertyChanged;
            AtualizarVinculoPersonagem(_personagemViewModel.Personagem);

            CarregarPericiasBase();
        }

        private void PersonagemViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PersonagemViewModel.Personagem))
            {
                AtualizarVinculoPersonagem(_personagemViewModel.Personagem);
            }
        }

        private void AtualizarVinculoPersonagem(PersonagemModel? novoPersonagem)
        {
            if (_personagem != null)
            {
                _personagem.PropertyChanged -= PersonagemModel_PropertyChanged;
            }

            _personagem = novoPersonagem;

            if (_personagem != null)
            {
                _personagem.PropertyChanged += PersonagemModel_PropertyChanged;
            }
            RecalcularTodasPericias();
        }

        private void PersonagemModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PersonagemModel.Nivel):
                case nameof(PersonagemModel.Forca):
                case nameof(PersonagemModel.Destreza):
                case nameof(PersonagemModel.Constituicao):
                case nameof(PersonagemModel.Inteligencia):
                case nameof(PersonagemModel.Sabedoria):
                case nameof(PersonagemModel.Carisma):
                case nameof(PersonagemModel.PenalidadeArmaduraTotal):
                    RecalcularTodasPericias();
                    break;
            }
        }

        private void CarregarPericiasBase()
        {
            ListaPericias.Clear();
            var periciasBase = new List<Tuple<string, string, bool, bool>>
            {
                Tuple.Create("Acrobacia", "DES", false, true), Tuple.Create("Adestramento", "CAR", true, false),
                Tuple.Create("Atletismo", "FOR", false, true), Tuple.Create("Atuação", "CAR", false, false),
                Tuple.Create("Cavalgar", "DES", false, false), Tuple.Create("Conhecimento", "INT", true, false),
                Tuple.Create("Cura", "SAB", false, false), Tuple.Create("Diplomacia", "CAR", false, false),
                Tuple.Create("Enganação", "CAR", false, false), Tuple.Create("Fortitude", "CON", false, false),
                Tuple.Create("Furtividade", "DES", false, true), Tuple.Create("Guerra", "INT", true, false),
                Tuple.Create("Iniciativa", "DES", false, false), Tuple.Create("Intimidação", "CAR", false, false),
                Tuple.Create("Intuição", "SAB", false, false), Tuple.Create("Investigação", "INT", false, false),
                Tuple.Create("Jogatina", "CAR", true, false), Tuple.Create("Ladinagem", "DES", true, true),
                Tuple.Create("Luta", "FOR", false, false), Tuple.Create("Misticismo", "INT", true, false),
                Tuple.Create("Navegação", "SAB", true, false), Tuple.Create("Percepção", "SAB", false, false),
                Tuple.Create("Pilotagem", "DES", true, false), Tuple.Create("Pontaria", "DES", false, false),
                Tuple.Create("Reflexos", "DES", false, false), Tuple.Create("Religião", "SAB", true, false),
                Tuple.Create("Sobrevivência", "SAB", false, false), Tuple.Create("Vontade", "SAB", false, false)
            };
            foreach (var pBase in periciasBase)
            {
                var periciasDetalhe = new PericiasModel(pBase.Item1, pBase.Item2, pBase.Item3, pBase.Item4);
                periciasDetalhe.RecalcularValorTotal += OnPericiaDetalheRecalcularValorTotal;
                ListaPericias.Add(periciasDetalhe);
            }
            RecalcularTodasPericias();
        }

        private void OnPericiaDetalheRecalcularValorTotal(PericiasModel pericia)
        {
            if (_personagem == null) return;
            CalcularValoresPericia(pericia, _personagem);
        }

        private void RecalcularTodasPericias()
        {
            if (_personagem == null)
            {
                foreach(var pericia in ListaPericias)
                {
                    pericia.ValorMetadeNivel = 0;
                    pericia.ModAtributoCalculado = 0;
                    pericia.BonusTreinamentoCalculado = 0;
                    pericia.PenalidadeAplicada = 0;
                    pericia.ValorTotal = 0;
                }
                return;
            }

            Debug.WriteLine($"[PericiasViewModel] Recalculando todas as perícias para {_personagem.Nome} Nível {_personagem.Nivel}");
            foreach (var pericia in ListaPericias)
            {
                CalcularValoresPericia(pericia, _personagem);
            }
        }

        private void CalcularValoresPericia(PericiasModel pericia, PersonagemModel personagem)
        {
            pericia.ValorMetadeNivel = personagem.NMetade;
            pericia.ModAtributoCalculado = GetModAtributo (personagem, pericia.AtributoChaveSelecionado);
            pericia.BonusTreinamentoCalculado = CalcularBonusTreinamento (personagem.Nivel, pericia.Treinada);
            pericia.PenalidadeAplicada = pericia.AplicaPenalidade ? personagem.PenalidadeArmaduraTotal : 0;

            int valorTotal = pericia.ValorMetadeNivel +
                             pericia.ModAtributoCalculado +
                             pericia.BonusTreinamentoCalculado +
                             pericia.OutrosBonus -
                             pericia.PenalidadeAplicada;

            pericia.ValorTotal = valorTotal;
            Debug.WriteLine($"Perícia: {pericia.NomePericia}, Total: {pericia.ValorTotal}, N/2: {pericia.ValorMetadeNivel}, ModAtr: {pericia.ModAtributoCalculado}, " +
                $"Treino: {pericia.BonusTreinamentoCalculado}, Outros: {pericia.OutrosBonus}, PenArm: {pericia.PenalidadeAplicada}");
        }

        private int GetModAtributo (PersonagemModel personagem, string atributoChave)
        {
            return atributoChave.ToUpper() switch
            {
                "FOR" => personagem.Forca ?? 0,
                "DES" => personagem.Destreza ?? 0,
                "CON" => personagem.Constituicao ?? 0,
                "INT" => personagem.Inteligencia ?? 0,
                "SAB" => personagem.Sabedoria ?? 0,
                "CAR" => personagem.Carisma ?? 0,
                _ => 0,
            };
        }

        private int CalcularBonusTreinamento(int nivelPersonagem, bool isTreinada)
        {
            if (!isTreinada) return 0;

            if (nivelPersonagem >= 1 && nivelPersonagem <= 6) return 2;
            if (nivelPersonagem >= 7 && nivelPersonagem <= 14) return 4;
            if (nivelPersonagem >= 15) return 6;
            return 0;
        }

        public void OnViewAppearing()
        {
            Debug.WriteLine("[PericiasViewModel] OnViewAppearing - Atualizando vínculo e recalculando perícias.");
            AtualizarVinculoPersonagem(_personagemViewModel.Personagem);
        }
    }
}
