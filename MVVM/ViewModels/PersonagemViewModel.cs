using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using T20FichaComDB.Data.Entities;
using T20FichaComDB.MVVM.Models;
using T20FichaComDB.Services;
using T20FichaComDB.MVVM.Views.Popups;
using CommunityToolkit.Maui.Views;
using System.Diagnostics;

namespace T20FichaComDB.MVVM.ViewModels
{
    public partial class PersonagemViewModel : ObservableObject
    {
        private readonly DataService _databaseService;
        private int _personagemAtualID = 0;

        [ObservableProperty]
        private PersonagemModel _personagem;

        // Coleções para preencher Pickers/ListViews na UI
        public ObservableCollection<string> RacasDisponiveis { get; } = new();
        public ObservableCollection<string> ClassesDisponiveis { get; } = new();
        public ObservableCollection<string> OrigensDisponiveis { get; } = new();
        public ObservableCollection<string> DivindadesDisponiveis { get; } = new();
        public ObservableCollection<PoderesData> PoderesRaca => _personagem?.PoderesRaca ?? new ObservableCollection<PoderesData>();
        public ObservableCollection<PoderesData> PoderesClasse => _personagem?.PoderesClasse ?? new ObservableCollection<PoderesData>();
        public ObservableCollection<PoderesData> PoderesConcedidos => _personagem?.PoderesConcedidos ?? new ObservableCollection<PoderesData>();
        public ObservableCollection<PoderesData> PoderesGerais => _personagem?.PoderesGerais ?? new ObservableCollection<PoderesData>();
        public ObservableCollection<PoderesData> PoderesOrigem => _personagem?.PoderesOrigem ?? new ObservableCollection<PoderesData>();

        private List<RacasData> _todasRacasComDetalhes = new ();

        private Dictionary<string, int> _modRaciaisAtuais = new();

        [ObservableProperty]
        private RacasData? _racaSelecionadaDetalhes;

        // Construtor para injeção de dependência 
        public PersonagemViewModel(DataService databaseService)
        {
            _databaseService = databaseService;
            Personagem = new PersonagemModel();
            Personagem.PropertyChanged += Personagem_PropertyChanged;

            SalvarPersonagemCommand = new AsyncRelayCommand(SalvarPersonagemAsync);

            // Carrega os dados iniciais (raças, classes, etc.)
            //InitializeAsync();
        }

        // Método para carregar listas de Raças, Classes, etc.
        public async Task InitializeAsync()
        {
            if (!RacasDisponiveis.Any())
            {
                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                if (RacasDisponiveis.Any()) RacasDisponiveis.Clear();
                if (OrigensDisponiveis.Any()) OrigensDisponiveis.Clear();
                if (ClassesDisponiveis.Any()) ClassesDisponiveis.Clear();
                if (DivindadesDisponiveis.Any()) DivindadesDisponiveis.Clear();
                if (PoderesRaca.Any()) PoderesRaca.Clear();
                _todasRacasComDetalhes.Clear();

                // -- CARREGAR DO BANCO DE DADOS
                var racasDB = await _databaseService.GetRacasAsync();
                var origensDB = await _databaseService.GetOrigensAsync();
                var classesDB = await _databaseService.GetClassesAsync();
                var divindadesDB = await _databaseService.GetDivindadesAsync();
                //var poderesDB = await _databaseService.GetPoderesPorTipoAsync(TipoPoderEnum.Raca);

                if (racasDB != null)
                {
                    _todasRacasComDetalhes.AddRange(racasDB);
                    foreach (var raca in racasDB.OrderBy(r => r.Nome)) RacasDisponiveis.Add(raca.Nome);
                }
                //if (poderesDB != null)
                //{
                //    foreach (var poder in poderesDB.OrderBy(p => p.Nome)) PoderesRaca.Add(poder);
                //}
                if (origensDB != null) foreach (var origem in origensDB.OrderBy(o => o.Nome)) OrigensDisponiveis.Add(origem.Nome);
                if (classesDB != null) foreach (var classe in classesDB.OrderBy(c => c.Nome)) ClassesDisponiveis.Add(classe.Nome);
                if (divindadesDB != null) foreach (var divindade in divindadesDB.OrderBy(d => d.Nome)) DivindadesDisponiveis.Add(divindade.Nome);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar dados: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Não foi possível carregar os dados de referência.", "OK");
            }

            System.Diagnostics.Debug.WriteLine($"RacasDisponiveis Count after load: {RacasDisponiveis.Count}");
        }

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
                        // UltimoSave será definido pelo DataService
                    };

                int savedRowCount = await _databaseService.SalvarPersonagemAsync(personagemData);

                if (Personagem.Id == 0 && savedRowCount > 0)
                {
                    var savedData = await _databaseService.GetPersonagemPorNomeAsync(Personagem.Nome);
                    if (savedData != null)
                    {
                        Personagem.Id = savedData.Id;
                        System.Diagnostics.Debug.WriteLine($"Novo ID {Personagem.Id} atribuído ao personagem {Personagem.Nome}.");
                    }
                }
                else if (savedRowCount > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Personagem {Personagem.Nome} (ID: {Personagem.Id} salvo com sucesso.");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Falha ao salvar personagem {Personagem.Nome} (ID: {Personagem.Id}).");
                }

                await Shell.Current.DisplayAlert("Sucesso", "Personagem salvo com sucesso!", "OK");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar personagem: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível salvar o personagem: {ex.Message}", "OK");
            }
        }

        public async Task CarregarPersonagemAsync(int id)
        {
            PersonagemData? data = await _databaseService.GetPersonagemPorIdAsync(id);
            if (data != null)
            {
                if (this.Personagem == null || this.Personagem.Id != data.Id)
                {
                    if (this.Personagem != null)
                    {
                        this.Personagem.PropertyChanged -= Personagem_PropertyChanged;
                    }
                    this.Personagem = new PersonagemModel
                    {
                        Id = data.Id,
                        Nome = data.Nome,
                        JogadorNome = data.JogadorNome,
                        Nivel = data.Nivel,
                        Raca = data.RacaNome,
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

                    if (!string.IsNullOrWhiteSpace(Personagem.Raca))
                    {
                        if (!_todasRacasComDetalhes.Any())
                        {
                            var racasDB = await _databaseService.GetRacasAsync();
                            if (racasDB != null) _todasRacasComDetalhes.AddRange(racasDB);
                        }
                        RacaSelecionadaDetalhes = _todasRacasComDetalhes.FirstOrDefault(r => r.Nome.Equals(Personagem.Raca, StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        RacaSelecionadaDetalhes = null;
                    }

                    this.Personagem.PropertyChanged += Personagem_PropertyChanged;
                    this.Personagem.Raca = data.RacaNome;
                    this.Personagem.Classe = data.ClasseNome;
                    this.Personagem.Origem = data.OrigemNome;
                    this.Personagem.Divindade = data.DivindadeNome;
                }

                Personagem.MagiasConhecidas.Clear();

                if (!string.IsNullOrWhiteSpace(data.MagiasConhecidasIDs))
                {
                    try
                    {
                        List<int> magiasIds = data.MagiasConhecidasIDs
                                                  .Split(',')
                                                  .Select(int.Parse)
                                                  .ToList();
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
                        System.Diagnostics.Debug.WriteLine($"Erro ao processar Magias Conhecidas ID '{data.MagiasConhecidasIDs}': {ex.Message}");
                    }
                }


                // Recalcula PV/PM com base nos dados carregados
                RecalculateAll();

                // Define PV/PM atuais (se não foram salvos, define como máximo)
                Personagem.PVatual = data.PVatuais > 0 ? data.PVatuais : Personagem.MaxPV;
                Personagem.PMatual = data.PMatuais > 0 ? data.PMatuais : Personagem.MaxPM;


                System.Diagnostics.Debug.WriteLine($"Personagem {Personagem.Nome} (ID: {Personagem.Id}) carregado.");

            }
            else
            {
                await Shell.Current.DisplayAlert("Erro", $"Personagem com ID {id} não encontrado.", "OK");
                if (this.Personagem != null)
                {
                    this.Personagem.PropertyChanged -= Personagem_PropertyChanged;
                }
                this.Personagem = new PersonagemModel();
                this.Personagem.PropertyChanged += Personagem_PropertyChanged;
                _modRaciaisAtuais.Clear();
                RecalculateAll();
            }
        }

        public async Task<PersonagemData> GetPersonagemPorNomeAsync(string nome)
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
                System.Diagnostics.Debug.WriteLine($"Erro ao buscar último personagem: {ex.Message}");
            }
            return null;
        }

        // --- LÓGICA PARA APLICAR OS MOD DE RAÇA
        private async Task AplicarModificadoresRaca (string? nomeNovaRaca)
        {
            System.Diagnostics.Debug.WriteLine($"[AplicarModificadoresRaca] Iniciado. Nova Raça: '{nomeNovaRaca ?? "NULL"}'");

            // ETAPA 0: VALIDAÇÃO INICIAL
            if (Personagem == null)
            {
                System.Diagnostics.Debug.WriteLine("[AplicarModificadoresRaca] ERRO: Personagem é NULL. Abortando.");
                return;
            }

            if (Personagem.PoderesRaca == null)
            {
                System.Diagnostics.Debug.WriteLine($"[AplicarModificadoresRaca] ALERTA CRÍTICO: Personagem.PoderesRaca era NULL. Reinstanciando. Personagem HASH: {Personagem.GetHashCode()}");
                Personagem.PoderesRaca = new System.Collections.ObjectModel.ObservableCollection<PoderesData>();
            }

            // ETAPA 1: LIMPEZA DOS MOD E PODERES DA RAÇA ANTERIOR
            RemoverModRacaAnterior();
            Personagem.PoderesRaca.Clear();
            System.Diagnostics.Debug.WriteLine("[AplicarModificadoresRaca] Modificadores e Poderes da raça anterior removidos/limpos.");
            RacaSelecionadaDetalhes = null;

            // ETAPA 2: LIDAR COM A SELEÇÃO DE NENHUMA RAÇA/INVALIDA
            if (string.IsNullOrWhiteSpace(nomeNovaRaca))
            {
                System.Diagnostics.Debug.WriteLine("[AplicarModificadoresRaca] Nome da nova raça é nulo ou vazio. Nenhum modificador ou poder de raça será aplicado.");
                RecalculateAll();
                return;
            }

            // ETAPA 3: ENCONTRAR OS DETALHES DA NOVA RAÇA
            if (_todasRacasComDetalhes == null || !_todasRacasComDetalhes.Any())
            {
                System.Diagnostics.Debug.WriteLine("[AplicarModificadoresRaca] ERRO: _todasRacasComDetalhes está vazia ou nula. As raças não foram carregadas do DataService. Tentando carregar agora...");
                await LoadDataAsync();
                if (_todasRacasComDetalhes == null || !_todasRacasComDetalhes.Any())
                {
                    System.Diagnostics.Debug.WriteLine("[AplicarModificadoresRaca] ERRO FATAL: Falha ao carregar _todasRacasComDetalhes. Abortando aplicação de raça.");
                    RecalculateAll();
                    return;
                }
            }

            var racaEncontrada = _todasRacasComDetalhes.FirstOrDefault(r => r.Nome.Equals(nomeNovaRaca, StringComparison.OrdinalIgnoreCase));

            if (racaEncontrada == null)
            {
                System.Diagnostics.Debug.WriteLine($"[AplicarModificadoresRaca] Raça '{nomeNovaRaca}' não encontrada na lista _todasRacasComDetalhes.");
                RecalculateAll();
                return;
            }

            // ETAPA 4: APLICAR MODIFICADORES DA NOVA RAÇA
            RacaSelecionadaDetalhes = racaEncontrada;
            System.Diagnostics.Debug.WriteLine($"[AplicarModificadoresRaca] Aplicando modificadores para a raça: {racaEncontrada.Nome}");

            AplicarModificador("Forca", racaEncontrada.ModForca);
            AplicarModificador("Destreza", racaEncontrada.ModDestreza);
            AplicarModificador("Constituicao", racaEncontrada.ModConstituicao);
            AplicarModificador("Inteligencia", racaEncontrada.ModInteligencia);
            AplicarModificador("Sabedoria", racaEncontrada.ModSabedoria);
            AplicarModificador("Carisma", racaEncontrada.ModCarisma);


            // ETAPA 5: CARREGAR E APLICAR PODERES DA NOVA RAÇA
            if (RacaSelecionadaDetalhes.ListaPoderesRacaNomes != null && RacaSelecionadaDetalhes.ListaPoderesRacaNomes.Any())
            {
                System.Diagnostics.Debug.WriteLine($"[AplicarModificadoresRaca] Raça '{RacaSelecionadaDetalhes.Nome}' tem os seguintes nomes de poderes para carregar: {string.Join(", ", RacaSelecionadaDetalhes.ListaPoderesRacaNomes)}");
                try 
                {
                    List<PoderesData> poderesDaRacaDb = await _databaseService.GetPoderesPorNomeETipoAsync(
                        RacaSelecionadaDetalhes.ListaPoderesRacaNomes, TipoPoderEnum.Raca);

                    if (poderesDaRacaDb != null && poderesDaRacaDb.Any())
                    {
                        foreach (var poder in poderesDaRacaDb.OrderBy(p => p.Nome))
                        {
                            Personagem.PoderesRaca.Add(poder);
                        }
                        System.Diagnostics.Debug.WriteLine($"[AplicarModificadoresRaca] {Personagem.PoderesRaca.Count} poderes raciais carregados e adicionados para {RacaSelecionadaDetalhes.Nome}.");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[AplicarModificadoresRaca] Nenhum poder de raça encontrado no banco para os nomes fornecidos para a raça {RacaSelecionadaDetalhes.Nome}.");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[AplicarModificadoresRaca] Nenhum poder de raça encontrado no banco para os nomes fornecidos para a raça {RacaSelecionadaDetalhes.Nome}.");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[AplicarModificadoresRaca] Raça '{RacaSelecionadaDetalhes.Nome}' não possui nomes de poderes de raça listados.");
            }

            // ETAPA 6: LIDAR COM MOD DE ATRIBUTOS LIVRES
            if (racaEncontrada.ModLivres > 0 && !string.IsNullOrWhiteSpace(RacaSelecionadaDetalhes.DescricaoModLivres))
            {
                System.Diagnostics.Debug.WriteLine($"[AplicarModificadoresRaca] Raça {racaEncontrada.Nome} tem {racaEncontrada.ModLivres} bônus livres: {RacaSelecionadaDetalhes.DescricaoModLivres}");

                string mensagemPopup = $"A raça {RacaSelecionadaDetalhes.Nome} concede: {RacaSelecionadaDetalhes.DescricaoModLivres}.";

                if (!string.IsNullOrWhiteSpace(RacaSelecionadaDetalhes.ExcecoesModLivres))
                {
                    mensagemPopup += $"\n(Exceto em: {RacaSelecionadaDetalhes.ExcecoesModLivres})";
                }
                mensagemPopup += "\nLembre-se de distribuir estes pontos manualmente nos atributos.";

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var popupViewModel = new AtributosLivresPopupViewModel(mensagemPopup);
                    var popup = new AtributosLivresPopupView(popupViewModel);
                    await Shell.Current.ShowPopupAsync(popup);
                });
            }

            // ETAPA 7: RECALCULAR TODOS OS STATUS
            RecalculateAll();
            System.Diagnostics.Debug.WriteLine("[AplicarModificadoresRaca] Finalizado. Status recalculados.");
        }

        private void PersonagemViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PersonagemViewModel.Personagem))
            {
                AtualizarPersonagem(Personagem);
            }
        }

        private void AtualizarPersonagem(PersonagemModel? personagem)
        {
            if (personagem != null && _personagem.PoderesRaca != null)
            {
                _personagem.PoderesRaca.CollectionChanged -= PoderesRaca_CollectionChanged;
            }
            _personagem = personagem;

            Debug.WriteLine($"PoderesViewModel.AtualizarPersonagem: Personagem is {(personagem == null ? "NULL" : "NOT NULL")}");

            if (_personagem != null)
            {
                Debug.WriteLine($"PoderesViewModel.AtualizarPersonagem: PoderesRaca Count: {personagem.PoderesRaca?.Count ?? -1}");

                if (_personagem.PoderesRaca != null)
                {
                    _personagem.PoderesRaca.CollectionChanged += PoderesRaca_CollectionChanged;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("AVISO CRÍTICO: _personagem.PoderesRaca é NULL ao tentar INSCREVER em PoderesViewModel.");
                }
                OnPropertyChanged(nameof(PoderesRaca));
            }
            else
            {
                OnPropertyChanged(nameof(PoderesRaca));
            }
        }

        private void PoderesRaca_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PoderesRaca));
        }

        [RelayCommand]
        private async Task MostrarDetalhesPoder(PoderesData? poder)
        {
            if (poder == null) return;

            var popupViewModel = new DetalhesPoderesPopupViewModel(poder);
            var popup = new DetalhesPoderesPopupView(popupViewModel);
            await Shell.Current.ShowPopupAsync(popup);
        }


        // --- Sessão de Recálculo ---
        private async void Personagem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            bool recalcularStatus = false;

            if (e.PropertyName == nameof(PersonagemModel.Raca))
            {
                await AplicarModificadoresRaca(Personagem.Raca);
                recalcularStatus = true;
            }

            switch (e.PropertyName)
            {
                case nameof(PersonagemModel.Nivel):
                case nameof(PersonagemModel.Classe):
                case nameof(PersonagemModel.Constituicao):
                case nameof(PersonagemModel.Inteligencia):
                case nameof(PersonagemModel.Sabedoria):
                case nameof(PersonagemModel.Carisma):
                case nameof(PersonagemModel.Destreza):
                case nameof(PersonagemModel.Forca):
                    recalcularStatus = true;

                    break;
            }

            if (recalcularStatus)
            {
                RecalculateAll();
            }
        }

        private void AplicarModificador (string atributo, int valor)
        {
            if (valor == 0) return;

            _modRaciaisAtuais[atributo] = valor;

            switch (atributo)
            {
                case "Forca": Personagem.Forca = (Personagem.Forca ?? 0) + valor; break;
                case "Destreza": Personagem.Destreza = (Personagem.Destreza ?? 0) + valor; break;
                case "Constituicao": Personagem.Constituicao = (Personagem.Constituicao ?? 0) + valor; break;
                case "Inteligencia": Personagem.Inteligencia = (Personagem.Inteligencia ?? 0) + valor; break;
                case "Sabedoria": Personagem.Sabedoria = (Personagem.Sabedoria ?? 0) + valor; break;
                case "Carisma": Personagem.Carisma = (Personagem.Carisma ?? 0) + valor; break; 
            }
            System.Diagnostics.Debug.WriteLine($"Aplicado modificador racial: {atributo} +({valor})");
        }

        private void RemoverModRacaAnterior()
        {
            if (Personagem == null || !_modRaciaisAtuais.Any())
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine("Removendo modificadores raciais anteriores...");

            foreach (var mod in _modRaciaisAtuais)
            {
                string atributo = mod.Key;
                int valorRemover = mod.Value;

                switch (atributo)
                {
                    case "Forca": Personagem.Forca = (Personagem.Forca ?? 0) - valorRemover; break;
                    case "Destreza": Personagem.Destreza = (Personagem.Destreza ?? 0) - valorRemover; break;
                    case "Constituicao": Personagem.Constituicao = (Personagem.Constituicao ?? 0) - valorRemover; break;
                    case "Inteligencia": Personagem.Inteligencia = (Personagem.Inteligencia ?? 0) - valorRemover; break;
                    case "Sabedoria": Personagem.Sabedoria = (Personagem.Sabedoria ?? 0) - valorRemover; break;
                    case "Carisma": Personagem.Carisma = (Personagem.Carisma ?? 0) - valorRemover; break;
                }

                System.Diagnostics.Debug.WriteLine($"Removido modificador racial: {atributo} -({valorRemover})");
            }
            _modRaciaisAtuais.Clear();
        }


        // Recalcula tudo - útil ao carregar um personagem
        private void RecalculateAll()
        {
            if (Personagem == null) return;
            System.Diagnostics.Debug.WriteLine("Recalculando Status (PV, PM, Defesa)...");

            CalculatePV();
            CalculatePM();
            CalculateDefesa();
            //CalculatePericias(); 
        }

        // --- Comandos Simples ---
        [RelayCommand]
        private void NivelUp()
        {
            if (Personagem.Nivel < Personagem.MaxNivel)
            {
                Personagem.Nivel++;
                RecalculateAll();
            }
        }

        [RelayCommand]
        private void FullHeal()
        {
            Personagem.PVatual = Personagem.MaxPV;
            Personagem.PMatual = Personagem.MaxPM;
        }

        // --- Lógica de Cálculo ---

        private void CalculatePV()
        {
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
                default: basePV = 10; pvPerNivel = 3; break; // Valor para classe não encontrada/definida
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

            // Ajusta PM atual
            if (Personagem.PMatual > Personagem.MaxPM || Personagem.PMatual <= 0)
            {
                Personagem.PMatual = Personagem.MaxPM;
            }
        }

        private int GetAtributoMod(string atributoChave)
        {
            return atributoChave switch
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
            int baseDef = 10;
            int destrezaMod = Personagem.Destreza ?? 0;

            // --- Placeholder para Bônus de Armadura e Escudo ---
            int armaduraBonus = 0;
            int escudoBonus = 0;
            int outroBonus = 0; 

            Personagem.Defesa = baseDef + destrezaMod + armaduraBonus + escudoBonus + outroBonus;
        }

    }
}
