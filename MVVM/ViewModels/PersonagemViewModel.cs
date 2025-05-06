using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using T20FichaComDB.Data.Entities;
using T20FichaComDB.MVVM.Models;
using T20FichaComDB.Services;

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

        private List<RacasData> _todasRacasComDetalhes = new ();
        private Dictionary<string, int> _modRaciaisAtuais = new();

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
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                RacasDisponiveis.Clear();
                OrigensDisponiveis.Clear();
                ClassesDisponiveis.Clear();
                DivindadesDisponiveis.Clear();

                // -- CARREGAR DO BANCO DE DADOS
                var racasDB = await _databaseService.GetRacasAsync();
                var origensDB = await _databaseService.GetOrigensAsync();
                var classesDB = await _databaseService.GetClassesAsync();
                var divindadesDB = await _databaseService.GetDivindadesAsync();

                _todasRacasComDetalhes.AddRange(racasDB);

                foreach (var raca in racasDB.OrderBy(r => r.Nome)) RacasDisponiveis.Add(raca.Nome);
                foreach (var origem in origensDB) OrigensDisponiveis.Add(origem.Nome);
                foreach (var classe in classesDB) ClassesDisponiveis.Add(classe.Nome);
                foreach (var divindade in divindadesDB) DivindadesDisponiveis.Add(divindade.Nome);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar dados: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Não foi possível carregar os dados de raças/classes/etc.", "OK");
            }
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
            PersonagemData data = await _databaseService.GetPersonagemPorIdAsync(id);
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

                    this.Personagem.PropertyChanged += Personagem_PropertyChanged;
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

                AplicarModificadoresRaca(data.RacaNome);

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


        // --- Seção de Recálculo ---
        private void Personagem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            bool recalcularStatus = false;

            if (e.PropertyName == nameof(PersonagemModel.Raca))
            {
                AplicarModificadoresRaca(Personagem.Raca);
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

        // --- LÓGICA PARA APLICAR OS MOD DE RAÇA
        private void AplicarModificadoresRaca (string? nomeNovaRaca)
        {
            if (Personagem == null) return;

            RemoverModRacaAnterior();

            // 1. REMOVER MOD RAÇA  
            if (string.IsNullOrWhiteSpace(nomeNovaRaca))
            {
                System.Diagnostics.Debug.WriteLine("Raça limpa ou inválida. Nenhum modificador aplicado.");
                return;
            }

            // 2. ENCONTRAR DADOS DA NOVA RAÇA
            var racaSelecionada = _todasRacasComDetalhes.FirstOrDefault(r => r.Nome.Equals(nomeNovaRaca, StringComparison.OrdinalIgnoreCase));

            if (racaSelecionada != null)
            {
                System.Diagnostics.Debug.WriteLine($"Raça '{nomeNovaRaca}' não encontrada nos dados carregados.");
                return; // Raça não encontrada
            }

            System.Diagnostics.Debug.WriteLine($"Aplicando modificadores para a raça: {racaSelecionada.Nome}");

            // 3. APLICAR MODIFICADORES FIXOS E GUARDAR
            AplicarModificador("Forca", racaSelecionada.ModForca);
            AplicarModificador("Destreza", racaSelecionada.ModDestreza);
            AplicarModificador("Constituicao", racaSelecionada.ModConstituicao);
            AplicarModificador("Inteligencia", racaSelecionada.ModInteligencia);
            AplicarModificador("Sabedoria", racaSelecionada.ModSabedoria);
            AplicarModificador("Carismoa", racaSelecionada.ModCarisma);

            // 4. LIDAR COM MODIFICADORES LIVRES
            if (racaSelecionada.ModLivres > 0 && !string.IsNullOrWhiteSpace(racaSelecionada.DescricaoModLivres))
            {
                System.Diagnostics.Debug.WriteLine($"Raça {racaSelecionada.Nome} tem {racaSelecionada.ModLivres} bônus livres: {racaSelecionada.DescricaoModLivres}");
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.DisplayAlert(
                        "Atributos Livres da Raça", $"Lembre-se de distribuir os bônus de atributo {racaSelecionada.Nome}: {racaSelecionada.DescricaoModLivres}", "OK");
                });
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
                return; // Não há modificadores anteriores para remover
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
