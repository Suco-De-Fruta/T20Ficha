using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using T20FichaComDB.Data.Entities;
using T20FichaComDB.MVVM.Models;
using T20FichaComDB.MVVM.Views.Popups;
using T20FichaComDB.MVVM.ViewModels.Popups;
using T20FichaComDB.Services;

namespace T20FichaComDB.MVVM.ViewModels
{
    public partial class PersonagemViewModel : ObservableObject
    {
        private readonly DataService _databaseService;
        public RacasViewModel RacasViewModel { get; }

        [ObservableProperty]
        private PersonagemModel _personagem;

        // Coleções para preencher Pickers/ListViews na UI 
        public ObservableCollection<string> ClassesDisponiveis { get; } = new();
        public ObservableCollection<string> OrigensDisponiveis { get; } = new();
        public ObservableCollection<string> DivindadesDisponiveis { get; } = new();

        private Dictionary<string, int> _ultimosModRaca = new();

        #region PROCESSO 1: Inicialização e Carregamento de Dados Essenciais

        // Construtor para injeção de dependência
        public PersonagemViewModel(DataService databaseService, RacasViewModel racaViewModel)
        {
            _databaseService = databaseService;
            RacasViewModel = racaViewModel;
            Personagem = new PersonagemModel();

            // Inscreve nos eventos de mudança
            Personagem.PropertyChanged += Personagem_PropertyChanged;
            RacasViewModel.RacaChanged += RacaViewModel_RacaChanged;

            SalvarPersonagemCommand = new AsyncRelayCommand(SalvarPersonagemAsync);
        }

        // Método para carregar listas de Raças, Classes, etc.
        public async Task InitializeAsync()
        {
            if (!RacasViewModel.RacasDisponiveisNomes.Any())
            {
                await RacasViewModel.InitializeAsync();
            }

            if (!ClassesDisponiveis.Any() && !OrigensDisponiveis.Any() && !DivindadesDisponiveis.Any())
            {
                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                if (OrigensDisponiveis.Any()) OrigensDisponiveis.Clear();
                if (ClassesDisponiveis.Any()) ClassesDisponiveis.Clear();
                if (DivindadesDisponiveis.Any()) DivindadesDisponiveis.Clear();

                var origensDB = await _databaseService.GetOrigensAsync();
                var classesDB = await _databaseService.GetClassesAsync();
                var divindadesDB = await _databaseService.GetDivindadesAsync();

                if (origensDB != null) foreach (var origem in origensDB.OrderBy(o => o.Nome)) OrigensDisponiveis.Add(origem.Nome);
                if (classesDB != null) foreach (var classe in classesDB.OrderBy(c => c.Nome)) ClassesDisponiveis.Add(classe.Nome);
                if (divindadesDB != null) foreach (var divindade in divindadesDB.OrderBy(d => d.Nome)) DivindadesDisponiveis.Add(divindade.Nome);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar dados: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Não foi possível carregar os dados de referência.", "OK");
            }
        }

        #endregion FIM DO PROCESSO 1

        #region PROCESSO 2: Gerenciamento de Dados do Personagem (Salvar, Carregar, Consultar)

        // Comando para Salvar
        public IAsyncRelayCommand SalvarPersonagemCommand { get; }

        private async Task SalvarPersonagemAsync()
        {
            try
            {
                if (this.Personagem == null)
                {
                    await Shell.Current.DisplayAlert("Erro", "Nenhum personagem carregado para salvar.", "OK");
                    return;
                }

                // Temporariamente reverte os modificadores raciais para salvar os atributos base CORRETOS
                Dictionary<string, int> modsAtuaisCopia = new Dictionary<string, int>(_ultimosModRaca);
                ReverterUltimosModificadoresDeAtributo();

                string magiasIdString = string.Empty;
                if (Personagem.MagiasConhecidas != null && Personagem.MagiasConhecidas.Any())
                {
                    magiasIdString = string.Join(",", Personagem.MagiasConhecidas.Select(m => m.Id));
                }

                var personagemData = new PersonagemData
                {
                    Id = Personagem.Id,
                    Nome = Personagem.Nome,
                    JogadorNome = Personagem.JogadorNome,
                    Nivel = Personagem.Nivel,
                    RacaNome = Personagem.Raca ?? string.Empty,
                    ClasseNome = Personagem.Classe ?? string.Empty,
                    OrigemNome = Personagem.Origem ?? string.Empty,
                    DivindadeNome = Personagem.Divindade ?? string.Empty,
                    MagiasConhecidasIDs = magiasIdString,

                    Forca = Personagem.Forca ?? 0,
                    Destreza = Personagem.Destreza ?? 0,
                    Constituicao = Personagem.Constituicao ?? 0,
                    Inteligencia = Personagem.Inteligencia ?? 0,
                    Sabedoria = Personagem.Sabedoria ?? 0,
                    Carisma = Personagem.Carisma ?? 0,
                    PVatuais = Personagem.PVatual,
                    PMatuais = Personagem.PMatual,
                };

                int savedRowCount = await _databaseService.SalvarPersonagemAsync(personagemData);

                // Reaplicar os modificadores raciais após salvar, para manter o estado da UI consistente
                foreach (var modEntry in modsAtuaisCopia)
                {
                    AplicarModDeAtributo(modEntry.Key, modEntry.Value, rastrear: true);
                }

                if (Personagem.Id == 0 && savedRowCount > 0)
                {
                    var savedData = await _databaseService.GetPersonagemPorNomeAsync(Personagem.Nome);
                    if (savedData != null)
                    {
                        Personagem.Id = savedData.Id; // Atualiza o ID no modelo
                        Debug.WriteLine($"Novo ID {Personagem.Id} atribuído ao personagem {Personagem.Nome}.");
                    }
                }
                else if (savedRowCount > 0)
                {
                    Debug.WriteLine($"Personagem {Personagem.Nome} (ID: {Personagem.Id}) salvo com sucesso.");
                }
                else
                {
                    Debug.WriteLine($"Falha ao salvar personagem {Personagem.Nome} (ID: {Personagem.Id}).");
                }

                await Shell.Current.DisplayAlert("Sucesso", "Personagem salvo com sucesso!", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao salvar personagem: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível salvar o personagem: {ex.Message}", "OK");
            }
        }


        public async Task CarregarPersonagemAsync(int id)
        {
            PersonagemData? data = await _databaseService.GetPersonagemPorIdAsync(id);
            if (data != null)
            {
                if (this.Personagem != null)
                {
                    this.Personagem.PropertyChanged -= Personagem_PropertyChanged;
                    ReverterUltimosModificadoresDeAtributo();
                }

                this.Personagem = new PersonagemModel // Atribui uma NOVA instância de PersonagemModel
                {
                    Id = data.Id,
                    Nome = data.Nome,
                    JogadorNome = data.JogadorNome,
                    Nivel = data.Nivel,
                    Classe = data.ClasseNome,
                    Origem = data.OrigemNome,
                    Divindade = data.DivindadeNome,
                    Forca = data.Forca,        
                    Destreza = data.Destreza,
                    Constituicao = data.Constituicao,
                    Inteligencia = data.Inteligencia,
                    Sabedoria = data.Sabedoria,
                    Carisma = data.Carisma
                };
                this.Personagem.PropertyChanged += Personagem_PropertyChanged;
                await RacasViewModel.SelecionarRacaAsync(data.RacaNome);

                Personagem.MagiasConhecidas.Clear();
                if (!string.IsNullOrWhiteSpace(data.MagiasConhecidasIDs))
                {
                    try
                    {
                        List<int> magiasIds = data.MagiasConhecidasIDs.Split(',').Select(int.Parse).ToList();
                        if (magiasIds.Any())
                        {
                            List<MagiasData> magiasDoBanco = await _databaseService.GetMagiasPorIdsAsync(magiasIds);
                            foreach (var magia in magiasDoBanco.OrderBy(m => m.Circulo).ThenBy(m => m.Nome))
                            {
                                Personagem.MagiasConhecidas.Add(magia);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Erro ao processar Magias Conhecidas ID '{data.MagiasConhecidasIDs}': {ex.Message}");
                    }
                }
                Personagem.PVatual = data.PVatuais > 0 ? data.PVatuais : Personagem.MaxPV;
                Personagem.PMatual = data.PMatuais > 0 ? data.PMatuais : Personagem.MaxPM;

                Debug.WriteLine($"Personagem {Personagem.Nome} (ID: {Personagem.Id}) carregado. Atributos base definidos.");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erro", $"Personagem com ID {id} não encontrado.", "OK");
                if (this.Personagem != null)
                {
                    this.Personagem.PropertyChanged -= Personagem_PropertyChanged;
                }
                // Se não encontrou, cria um novo
                this.Personagem = new PersonagemModel();
                this.Personagem.PropertyChanged += Personagem_PropertyChanged;
                await RacasViewModel.SelecionarRacaAsync(null);
                RecalculateAll();
            }
        }

        public async Task<PersonagemData?> GetPersonagemPorNomeAsync(string nome)
        {
            return await _databaseService.GetPersonagemPorNomeAsync(nome);
        }

        public async Task<int?> GetPersonagemMaisRecenteIdAsync()
        {
            try
            {
                var personagens = await _databaseService.GetPersonagensAsync();
                if (personagens != null && personagens.Any())
                {
                    return personagens.First().Id;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao buscar último personagem: {ex.Message}");
            }
            return null;
        }

        #endregion FIM DO PROCESSO 2

        #region PROCESSO 2: Lógica de Modificadores de Raça

        private async void RacaViewModel_RacaChanged(object? sender, EventArgs e)
        {

            if (Personagem == null) return;

            Debug.WriteLine($"[PersonagemViewModel] RacaViewModel_RacaChanged disparado. Nova raça selecionada no RacasViewModel: {RacasViewModel.RacaSelecionadaDetalhes?.Nome ?? "Nenhuma"}");

            // 1. Reverter os modificadores de atributos da raça ANTERIOR
            ReverterUltimosModificadoresDeAtributo();

            // 2. Limpar poderes da raça anterior do PersonagemModel
            if (Personagem.PoderesRaca.Any())
            {
                var nomesPoderesRacaAnterior = Personagem.PoderesRaca
                                               .Where(p => p.RequerEscolha)
                                               .Select(p => p.Nome)
                                               .ToList();

                foreach (var nomePoder in nomesPoderesRacaAnterior)
                {
                    if (Personagem.EscolhasDePoderesFeitas.ContainsKey(nomePoder))
                    {
                        Personagem.EscolhasDePoderesFeitas.Remove(nomePoder);
                        Debug.WriteLine($"[PersonagemViewModel] Escolha para o poder '{nomePoder}' da raça anterior removida.");
                    }
                }
                Personagem.PoderesRaca.Clear();
            }

            // 3. Se uma nova raça foi selecionada no RacasViewModel, aplicar seus dados
            if (RacasViewModel.RacaSelecionadaDetalhes != null)
            {
                Personagem.Raca = RacasViewModel.RacaSelecionadaDetalhes.Nome;

                foreach (var mod in RacasViewModel.ModAtributosRaca)
                {
                    AplicarModDeAtributo(mod.Key, mod.Value, rastrear: true);
                }

                foreach (var poder in RacasViewModel.PoderesDaRacaSelecionada)
                {
                    if (!Personagem.PoderesRaca.Any(p => p.Id == poder.Id)) // Evitar duplicatas
                    {
                        Personagem.PoderesRaca.Add(poder);
                    }
                }
                Debug.WriteLine($"[PersonagemViewModel] {Personagem.PoderesRaca.Count} poderes da raça '{Personagem.Raca}' adicionados.");

                if (RacasViewModel.ModLivresRaca > 0 && !string.IsNullOrWhiteSpace((string)RacasViewModel.DescricaoModLivresRaca))
                {
                    string mensagemPopup = $"A raça {RacasViewModel.RacaSelecionadaDetalhes.Nome} concede: {RacasViewModel.DescricaoModLivresRaca}.";
                    if (!string.IsNullOrWhiteSpace((string)RacasViewModel.ExcecoesModLivresRaca))
                    {
                        mensagemPopup += $"\n(Exceto em: {RacasViewModel.ExcecoesModLivresRaca})";
                    }
                    mensagemPopup += $"\nVocê tem {RacasViewModel.ModLivresRaca} ponto(s) para distribuir manualmente.";

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        var popupViewModel = new AtributosLivresPopupViewModel(mensagemPopup);
                        var popup = new AtributosLivresPopupView(popupViewModel);
                        await Shell.Current.ShowPopupAsync(popup);
                    });
                }
            }
            else
            {
                Personagem.Raca = null;
                Debug.WriteLine("[PersonagemViewModel] Nenhuma raça selecionada. Modificadores e poderes raca limpos.");
            }

            // 4. Recalcular todos os status derivados (PV, PM, Defesa, etc.)
            RecalculateAll();
            Debug.WriteLine("[PersonagemViewModel] Status derivados recalculados após mudança de raça.");
        }

        private void AplicarModDeAtributo(string atributoNome, int valorModificador, bool rastrear = false)
        {
            if (Personagem == null || valorModificador == 0) return;

            switch (atributoNome)
            {
                case "Forca": Personagem.Forca = (Personagem.Forca ?? 0) + valorModificador; break;
                case "Destreza": Personagem.Destreza = (Personagem.Destreza ?? 0) + valorModificador; break;
                case "Constituicao": Personagem.Constituicao = (Personagem.Constituicao ?? 0) + valorModificador; break;
                case "Inteligencia": Personagem.Inteligencia = (Personagem.Inteligencia ?? 0) + valorModificador; break;
                case "Sabedoria": Personagem.Sabedoria = (Personagem.Sabedoria ?? 0) + valorModificador; break;
                case "Carisma": Personagem.Carisma = (Personagem.Carisma ?? 0) + valorModificador; break;
                default:
                    Debug.WriteLine($"[PersonagemViewModel] Tentativa de aplicar modificador a atributo desconhecido: {atributoNome}");
                    return;
            }

            if (rastrear)
            {
                _ultimosModRaca[atributoNome] = valorModificador;
            }
            Debug.WriteLine($"[PersonagemViewModel] Atributo '{atributoNome}' modificado por {valorModificador}. Novo valor: {GetAtributoValor(atributoNome)}");
        }

        private void ReverterUltimosModificadoresDeAtributo()
        {
            if (Personagem == null || !_ultimosModRaca.Any())
            {
                _ultimosModRaca.Clear();
                return;
            }

            Debug.WriteLine("[PersonagemViewModel] Revertendo últimos modificadores de atributos raca...");
            foreach (var modEntry in _ultimosModRaca)
            {
                AplicarModDeAtributo(modEntry.Key, -modEntry.Value, rastrear: false);
            }
            _ultimosModRaca.Clear(); // Limpa após a reversão
            Debug.WriteLine("[PersonagemViewModel] Modificadores raca anteriores revertidos.");
        }

        private int? GetAtributoValor(string atributoNome)
        {
            if (Personagem == null) return 0;
            return atributoNome switch
            {
                "Forca" => Personagem.Forca,
                "Destreza" => Personagem.Destreza,
                "Constituicao" => Personagem.Constituicao,
                "Inteligencia" => Personagem.Inteligencia,
                "Sabedoria" => Personagem.Sabedoria,
                "Carisma" => Personagem.Carisma,
                _ => 0
            };
        }

        #endregion FIM DO PROCESSO 3

        #region PROCESSO 4: Cálculo e Atualização de Atributos e Status Derivados

        private async void Personagem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            bool recalcularStatus = false;

            switch (e.PropertyName)
            {
                case nameof(PersonagemModel.Raca):
                    if (RacasViewModel.RacaSelecionadaDetalhes?.Nome != Personagem.Raca)
                    {
                        string? racaParaSelecionar = Personagem.Raca; // Captura antes de potencial chamada async
                        await RacasViewModel.SelecionarRacaAsync(racaParaSelecionar);
                    }
                    break;

                case nameof(PersonagemModel.Nivel):
                case nameof(PersonagemModel.Classe):
                case nameof(PersonagemModel.Forca):
                case nameof(PersonagemModel.Destreza):
                case nameof(PersonagemModel.Constituicao):
                case nameof(PersonagemModel.Inteligencia):
                case nameof(PersonagemModel.Sabedoria):
                case nameof(PersonagemModel.Carisma):
                    recalcularStatus = true;
                    break;
            }

            if (recalcularStatus)
            {
                RecalculateAll();
            }
        }

        private void RecalculateAll()
        {
            if (Personagem == null) return;
            Debug.WriteLine("Recalculando Status (PV, PM, Defesa)...");

            CalculatePV();
            CalculatePM();
            CalculateDefesa();
        }

        private void CalculatePV()
        {
            if (Personagem == null) return;

            int basePV = 10;
            int pvPerNivel = 4;

            switch (Personagem.Classe?.ToLower())
            {
                case "arcanista": basePV = 8; pvPerNivel = 2; break;
                case "bárbaro": basePV = 24; pvPerNivel = 6; break;
                case "bardo": basePV = 12; pvPerNivel = 3; break;
                case "bucaneiro": basePV = 16; pvPerNivel = 4; break;
                case "caçador": basePV = 16; pvPerNivel = 4; break;
                case "cavaleiro": basePV = 20; pvPerNivel = 5; break;
                case "clérigo": basePV = 16; pvPerNivel = 4; break;
                case "druida": basePV = 16; pvPerNivel = 4; break;
                case "guerreiro": basePV = 20; pvPerNivel = 5; break;
                case "inventor": basePV = 12; pvPerNivel = 3; break;
                case "ladino": basePV = 12; pvPerNivel = 3; break;
                case "lutador": basePV = 20; pvPerNivel = 5; break;
                case "nobre": basePV = 16; pvPerNivel = 4; break;
                case "paladino": basePV = 20; pvPerNivel = 5; break;
                case "treinador": basePV = 12; pvPerNivel = 3; break;
                default: basePV = 10; pvPerNivel = 3; break;
            }

            int conMod = Personagem.Constituicao ?? 0;
            int nivel = Personagem.Nivel;

            int maxPV = basePV + conMod;
            if (nivel > 1)
            {
                maxPV += (pvPerNivel + conMod) * (nivel - 1);
            }

            Personagem.MaxPV = maxPV;

            if (Personagem.PVatual > maxPV || Personagem.PVatual <= 0)
            {
                Personagem.PVatual = maxPV;
            }
        }

        private void CalculatePM()
        {
            if (Personagem == null) return;

            int basePM = 0;
            int pmPerNivel = 0;
            int atributoMod = 0;
            string? atributoChave = null;

            switch (Personagem.Classe?.ToLower())
            {
                case "arcanista": basePM = 6; pmPerNivel = 6; atributoChave = "INT"; break;
                case "bardo": basePM = 4; pmPerNivel = 4; atributoChave = "CAR"; break;
                case "clérigo": basePM = 5; pmPerNivel = 5; atributoChave = "SAB"; break;
                case "bárbaro": basePM = 3; pmPerNivel = 3; break;
                case "bucaneiro": basePM = 3; pmPerNivel = 3; break;
                case "caçador": basePM = 4; pmPerNivel = 4; break;
                case "cavaleiro": basePM = 3; pmPerNivel = 3; break;
                case "druida": basePM = 4; pmPerNivel = 4; atributoChave = "SAB"; break;
                case "guerreiro": basePM = 3; pmPerNivel = 3; break;
                case "inventor": basePM = 4; pmPerNivel = 4; atributoChave = "INT"; break;
                case "ladino": basePM = 4; pmPerNivel = 4; break;
                case "lutador": basePM = 3; pmPerNivel = 3; break;
                case "nobre": basePM = 4; pmPerNivel = 4; atributoChave = "CAR"; break;
                case "paladino": basePM = 3; pmPerNivel = 3; atributoChave = "CAR"; break;
                case "treinador": basePM = 4; pmPerNivel = 4; atributoChave = "CAR"; break;
                default: basePM = 1; pmPerNivel = 1; break;
            }

            if (atributoChave != null)
            {
                atributoMod = GetAtributoMod(atributoChave);
            }

            int nivel = Personagem.Nivel;
            int maxPM = basePM + atributoMod;

            if (nivel > 1)
            {
                maxPM += pmPerNivel * (nivel - 1);
            }

            Personagem.MaxPM = Math.Max(1, maxPM);
            if (basePM == 0 && pmPerNivel == 0 && atributoMod == 0)
            {
                Personagem.MaxPM = 0;
            }

            if (Personagem.PMatual > Personagem.MaxPM || Personagem.PMatual <= 0 && Personagem.MaxPM > 0)
            {
                Personagem.PMatual = Personagem.MaxPM;
            }
            else if (Personagem.MaxPM == 0)
            {
                Personagem.PMatual = 0;
            }
        }

        // Retorna o valor do modificador de um atributo.
        private int GetAtributoMod(string atributoChave)
        {
            if (Personagem == null) return 0;
            return atributoChave.ToUpper() switch
            {
                "FOR" => Personagem.Forca ?? 0,
                "DES" => Personagem.Destreza ?? 0,
                "CON" => Personagem.Constituicao ?? 0,
                "INT" => Personagem.Inteligencia ?? 0,
                "SAB" => Personagem.Sabedoria ?? 0,
                "CAR" => Personagem.Carisma ?? 0,
                _ => 0
            };
        }

        private void CalculateDefesa()
        {
            if (Personagem == null) return;

            int baseDef = 10;
            int destrezaMod = Personagem.Destreza ?? 0;

            // --- Placeholder para Bônus de Armadura e Escudo ---
            // Estes valores viriam de itens equipados, poderes, etc.
            int armaduraBonus = 0;
            int escudoBonus = 0;
            int outroBonus = 0;
            Personagem.Defesa = baseDef + destrezaMod + armaduraBonus + escudoBonus + outroBonus;
        }

        #endregion FIM DO PROCESSO 4

        #region PROCESSO 5: Ações Simples do Personagem (Comandos)

        [RelayCommand]
        private void NivelUp()
        {
            if (Personagem != null && Personagem.Nivel < Personagem.MaxNivel) // MaxNivel é uma constante no PersonagemModel
            {
                Personagem.Nivel++;
            }
        }

        [RelayCommand]
        private void FullHeal()
        {
            if (Personagem != null)
            {
                Personagem.PVatual = Personagem.MaxPV;
                Personagem.PMatual = Personagem.MaxPM;
            }
        }

        #endregion FIM DO PROCESSO 5
    }
}
