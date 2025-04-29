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

        // Construtor para injeção de dependência (recomendado)
        public PersonagemViewModel(DataService databaseService)
        {
            _databaseService = databaseService;
            Personagem = new PersonagemModel();
            Personagem.PropertyChanged += Personagem_PropertyChanged;

            SalvarPersonagemCommand = new AsyncRelayCommand(SalvarPersonagemAsync);

            // Carrega os dados iniciais (raças, classes, etc.)
            InitializeAsync();
            RecalculateAll();
        }

        public PersonagemViewModel()
        {
            // Se usar este, _databaseService precisará ser injetado/definido posteriormente
            // ou obtido de um Service Locator / DI Container estático.
            Personagem = new PersonagemModel();
            // ... inicialização de comandos e coleções ...
        }

        public async Task CarregarPersonagemAsync(int id)
        {
            PersonagemData data = await _databaseService.GetPersonagemPorIdAsync(id);
            if (data != null)
            {
                _personagemAtualID = data.Id;

                
                Personagem.Nome = data.Nome;
                Personagem.JogadorNome = data.JogadorNome;
                Personagem.Nivel = data.Nivel;
                Personagem.Raca = data.RacaNome;
                Personagem.Classe = data.ClasseNome;
                Personagem.Origem = data.OrigemNome;
                Personagem.Divindade = data.DivindadeNome;
                Personagem.Forca = data.Forca;
                Personagem.Destreza = data.Destreza;
                Personagem.Constituicao = data.Constituicao;
                Personagem.Inteligencia = data.Inteligencia;
                Personagem.Sabedoria = data.Sabedoria;
                Personagem.Carisma = data.Carisma;

                // Recalcula PV/PM com base nos dados carregados
                RecalculateAll();

                // Define PV/PM atuais (se não foram salvos, define como máximo)
                Personagem.PVatual = data.PVatuais > 0 ? data.PVatuais : Personagem.MaxPV;
                Personagem.PMatual = data.PMatuais > 0 ? data.PMatuais : Personagem.MaxPM;

                // TODO: Carregar Perícias, Poderes, Inventário, Magias se/quando implementado

                Personagem.PropertyChanged -= Personagem_PropertyChanged;
                Personagem.PropertyChanged += Personagem_PropertyChanged;
            }
            else
            {
                await Shell.Current.DisplayAlert("Erro", $"Personagem com ID {id} não encontrado.", "OK");
                Personagem = new PersonagemModel();
                _personagemAtualID = 0;
                RecalculateAll();
            }
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

                var racasDB = await _databaseService.GetRacasAsync();
                var origensDB = await _databaseService.GetOrigensAsync();
                var classesDB = await _databaseService.GetClassesAsync();
                var divindadesDB = await _databaseService.GetDivindadesAsync();

                foreach (var raca in racasDB) RacasDisponiveis.Add(raca.Nome);
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
                var personagemData = new PersonagemData
                {
                    Id = _personagemAtualID,
                    Nome = Personagem.Nome,
                    JogadorNome = Personagem.JogadorNome,
                    Nivel = Personagem.Nivel,
                    RacaNome = Personagem.Raca,
                    ClasseNome = Personagem.Classe,
                    OrigemNome = Personagem.Origem,
                    DivindadeNome = Personagem.Divindade,
                    Forca = Personagem.Forca ?? 0,
                    Destreza = Personagem.Destreza ?? 0,
                    Constituicao = Personagem.Constituicao ?? 0,
                    Inteligencia = Personagem.Inteligencia ?? 0,
                    Sabedoria = Personagem.Sabedoria ?? 0,
                    Carisma = Personagem.Carisma ?? 0,
                    PVatuais = Personagem.PVatual,
                    PMatuais = Personagem.PMatual
                    // UltimoSave será definido pelo DataService
                    // TODO: Salvar Perícias, Poderes, Inventário, Magias se/quando implementado
                };

                int savedId = await _databaseService.SalvarPersonagemAsync(personagemData);

                if (_personagemAtualID == 0 && savedId != 0)
                {
                    _personagemAtualID = savedId;
                }

                await Shell.Current.DisplayAlert("Sucesso", "Personagem salvo com sucesso!", "OK");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar personagem: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível salvar o personagem: {ex.Message}", "OK");
            }
        }


        // --- Seção de Recálculo ---
        private void Personagem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PersonagemModel.Nivel):
                case nameof(PersonagemModel.Classe):
                    CalculatePV();
                    CalculatePM();
                    //CalculatePericias(); // Descomente se implementar perícias
                    break;

                case nameof(PersonagemModel.Constituicao):
                    CalculatePV();
                    //CalculatePericiasAtributoChave("CON"); // Descomente se implementar perícias
                    break;

                // Adicione outros atributos que afetam PM ou Pericias
                case nameof(PersonagemModel.Inteligencia):
                    CalculatePM(); // Exemplo: se INT afeta PM
                                   //CalculatePericiasAtributoChave("INT");
                    break;
                case nameof(PersonagemModel.Sabedoria):
                    CalculatePM(); // Exemplo: se SAB afeta PM
                                   //CalculatePericiasAtributoChave("SAB");
                    break;
                case nameof(PersonagemModel.Carisma):
                    CalculatePM(); // Exemplo: se CAR afeta PM
                                   //CalculatePericiasAtributoChave("CAR");
                    break;

                case nameof(PersonagemModel.Destreza):
                    CalculateDefesa();
                    //CalculatePericiasAtributoChave("DES");
                    break;

                case nameof(PersonagemModel.Forca):
                    //CalculatePericiasAtributoChave("FOR");
                    break;
                    // case nameof(PersonagemModel.EquipArmadura): // Exemplo: Se mudar armadura, recalcula defesa
                    //    CalculateDefesa();
                    //    break;
            }
        }

        // Recalcula tudo - útil ao carregar um personagem
        private void RecalculateAll()
        {
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
                // Recalculos são acionados pelo PropertyChanged se o handler estiver ativo,
                // ou chame RecalculateAll() ou métodos específicos aqui se não estiver usando o handler.
                RecalculateAll(); // Exemplo de chamada direta
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

        // Retorna o valor do atributo baseado na string de abreviação
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
            // o item equipado e então buscar o bônus desse item.
            int armaduraBonus = 0; // Ex: GetBonusFromItem(Personagem.EquipArmadura);
            int escudoBonus = 0;   // Ex: GetBonusFromItem(Personagem.EquipEscudo);
            int outroBonus = 0;   // Ex: Bônus de raça, poder, etc.

            // --- Placeholder para Penalidade de Armadura e Limite de Destreza ---
            // bool usingArmaduraPesada = false; // Ex: IsArmaduraPesada(Personagem.EquipArmadura);
            // int maxDexBonus = 99; // Ex: GetMaxDexBonus(Personagem.EquipArmadura);
            // destrezaMod = Math.Min(destrezaMod, maxDexBonus); // Aplica limite de DES
            // if (usingArmaduraPesada && destrezaMod > 0) destrezaMod = 0; // Regra de armadura pesada (verificar T20 JdA)

            Personagem.Defesa = baseDef + destrezaMod + armaduraBonus + escudoBonus + outroBonus;
        }


        // ---------- LOGICA DE PERICIAS -----------

        /*
        private void Pericia_PropertyChanged (object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pericias.ETreinado) && sender is Pericias changedPercia)
            {
                CalculatePericiaIndividual(changedPercia);
            }
          // if (e.PropertyName == nameof(Pericias.BonusOutros) && sender is Pericias changedPericia) { recalcular TotalBonus... }
        }

        private void CalculatePericiaIndividual(Pericias pericias)
        {
            if (pericias?.AtributoChave == null) return;

            // Usa Personagem.NMetade da classe PersonagemModel
            int bonusNivel = Personagem.NMetade;
            int bonusAtributo = GetAtributoMod(pericias.AtributoChave);
            int bonusTreinado = 0;

            if (pericias.ETreinado)
            {
                // Bônus de treinamento T20 JdA pág. 118
                if (Personagem.Nivel >= 15) bonusTreinado = 6;
                else if (Personagem.Nivel >= 7) bonusTreinado = 4;
                else bonusTreinado = 2;
            }

            int bonusOutros = pericias.BonusOutros; // Assumindo que Pericias tem essa propriedade

            // Penalidade por usar perícia não treinada que exige treinamento
            // (ApenasTreinada é uma propriedade hipotética na classe Pericias)
            // if (pericias.ApenasTreinada && !pericias.ETreinado)
            // {
            //     // Em T20, você simplesmente não pode usar a perícia,
            //     // a menos que regras específicas permitam.
            //     // Poderia definir o bônus total como um valor muito baixo ou lançar exceção?
            //     // Ou a UI deveria desabilitar a rolagem?
            //     // Vamos assumir que a UI impede ou mostra um aviso.
            //     // Definir um valor numérico aqui pode ser enganoso.
            // }

            // --- Placeholder para Penalidade de Armadura ---
            // int armorPenalty = GetCurrentArmorPenalty(); // Precisa buscar do item de armadura equipado
            // if (pericias.AplicaPenalidadeArmadura) // Propriedade hipotética em Pericias
            // {
            //    bonusOutros -= armorPenalty;
            // }

            // Atualiza as propriedades da perícia (se existirem na classe Pericias)
            // pericias.BonusNivel = bonusNivel;
            // pericias.BonusAtributo = bonusAtributo;
            // pericias.BonusTreino = bonusTreinado;
            // pericias.BonusOutros = bonusOutros; // Já inclui penalidade de armadura, se houver
            // pericias.BonusTotal = bonusNivel + bonusAtributo + bonusTreinado + bonusOutros;
        }

        private void CalculatePericias()
        {
            // Assumindo que Personagem.Pericias é ObservableCollection<Pericias> em PersonagemModel
            if (Personagem?.Pericias == null) return;
            foreach (var pericias in Personagem.Pericias)
            {
                CalculatePericiaIndividual(pericias);
            }
        }

        private void CalculatePericiasAtributoChave(string atributoNome)
        {
             if (Personagem?.Pericias == null) return;
            // Usa LINQ para filtrar as perícias relevantes
            foreach (var pericias in Personagem.Pericias.Where(s => s.AtributoChave == atributoNome))
            {
                CalculatePericiaIndividual(pericias);
            }
        }
        */


        // ---------- NAVEGAÇÃO -----------
        // Adapte conforme a estrutura de navegação do seu App (MAUI Shell, etc.)
        /*
        public INavigation Navigation { get; set; } // Injete ou obtenha de outra forma

        [RelayCommand]
        private async Task NavigateToDetails()
        {
            if (Navigation != null)
            {
                // Certifique-se que PersonagemDetalhesView existe e aceita este ViewModel
                await Navigation.PushAsync(new PersonagemDetalhesView { BindingContext = this });
            }
            else
            {
                 System.Diagnostics.Debug.WriteLine("Erro: Serviço de Navegação não disponível.");
                 // Talvez um DisplayAlert aqui
            }
        }
        */
    }
}
