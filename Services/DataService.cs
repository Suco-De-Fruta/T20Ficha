using SQLite;
using T20FichaComDB.Data;
using T20FichaComDB.Data.Entities;

namespace T20FichaComDB.Services
{
    public class DataService
    {
        private static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        private const string DatabaseFilename = "NewT20Ficha.db3";
        private const SQLiteOpenFlags Flags =
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create    |
            SQLiteOpenFlags.SharedCache;

        private SQLiteAsyncConnection _connection;

        // Garante que a conexão seja inicializada apenas uma vez
        private async Task EnsureInitializedAsync()
        {
            System.Diagnostics.Debug.WriteLine("Entrando em EnsureInitializedAsync");

            if (_connection == null)
            {
                try
                {
                    _connection = new SQLiteAsyncConnection(DatabasePath, Flags);

                    await _connection.CreateTableAsync<RacasData>();
                    await _connection.CreateTableAsync<OrigensData>();
                    await _connection.CreateTableAsync<ClassesData>();
                    await _connection.CreateTableAsync<DivindadesData>();
                    await _connection.CreateTableAsync<PersonagemData>();
                    await _connection.CreateTableAsync<MagiasData>();

                    await DataBaseSeeder.SeedDataAsync(_connection);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao inicializar o banco de dados: {ex.Message}");
                    throw;
                }
            }

            System.Diagnostics.Debug.WriteLine("Finalizando EnsureInitializedAsync");
        }


        // ----- MÉTODOS PARA OBTER DADOS DE REFERÊNCIA -----

        public async Task<List<RacasData>> GetRacasAsync()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<RacasData>().OrderBy(r => r.Nome).ToListAsync();

        }

        public async Task<List<ClassesData>> GetClassesAsync()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<ClassesData>().OrderBy(c => c.Nome).ToListAsync();
        }

        public async Task<List<OrigensData>> GetOrigensAsync()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<OrigensData>().OrderBy(o => o.Nome).ToListAsync();
        }

        public async Task<List<DivindadesData>> GetDivindadesAsync()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<DivindadesData>().OrderBy(d => d.Nome).ToListAsync();
        }


        // MAGIAS E BUSCA DE MAGIAS
        public async Task<List<MagiasData>> GetMagiasAsync()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<MagiasData>().OrderBy(m => m.Circulo).ThenBy(m => m.Nome).ToListAsync();
        }

        public async Task<List<MagiasData>> GetMagiasPorCirculoETipoAsync(int circulo, string tipo)
        {
            await EnsureInitializedAsync();
            bool buscarUniversal = tipo != TipoMagia.Universal.ToString();

            var query = _connection.Table<MagiasData>().Where(m => m.Circulo == circulo);
            if (buscarUniversal)
            {
                query = query.Where(m => m.Nome == tipo || m.Tipo == TipoMagia.Universal.ToString());
            } else
            {
                query = query.Where(m => m.Tipo == tipo);
            }

            return await query.OrderBy(m => m.Nome).ToListAsync();
        }

        public async Task <List<MagiasData>> GetMagiasArcanasPorCirculoAsync(int circulo)
        {
            await EnsureInitializedAsync();
            return await _connection.Table<MagiasData>()
                                    .Where(m => m.Circulo == circulo && (m.Tipo == "Arcana" || m.Tipo == "Universal"))
                                    .OrderBy(m => m.Nome).ToListAsync();
        }

        public async Task<List<MagiasData>> GetMagiasDivinasPorCirculoAsync(int circulo)
        {
            await EnsureInitializedAsync();
            return await _connection.Table<MagiasData>()
                                    .Where(m => m.Circulo == circulo && (m.Tipo == "Divina" || m.Tipo == "Universal"))
                                    .OrderBy(m => m.Nome).ToListAsync();
        }


        // ----- MÉTODOS PARA GERENCIAR PERSONAGENS -----

        public async Task<int> SalvarPersonagemAsync(PersonagemData personagem)
        {
            await EnsureInitializedAsync();

            personagem.UltimoSave = DateTime.UtcNow;
            return await _connection.InsertOrReplaceAsync(personagem);
        }

        public async Task<List<PersonagemData>> GetPersonagensAsync()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<PersonagemData>().OrderByDescending(p => p.UltimoSave).ToListAsync();
        }

        public async Task<PersonagemData> GetPersonagemPorIdAsync(int id)
        {
            await EnsureInitializedAsync();
            return await _connection.Table<PersonagemData>().Where(p => p.Id == id).FirstOrDefaultAsync();
            // _connection.FindAsync<PersonagemData>(id);
        }


        public async Task<int> DeletarPersonagemAsync(PersonagemData personagem)
        {
            await EnsureInitializedAsync();
            return await _connection.DeleteAsync(personagem);
        }

        public async Task<int> DeletarPersonagemPorIdAsync(int id)
        {
            await EnsureInitializedAsync();
            return await _connection.DeleteAsync<PersonagemData>(id);
        }
    }
}