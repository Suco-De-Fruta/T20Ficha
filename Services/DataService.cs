using SQLite;
using SQLitePCL;
using System.IO;
using Microsoft.Maui.Storage;
using T20FichaComDB.Data.Entities;
using T20FichaComDB.Data;      

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
            if (_connection == null)
            {
                _connection = new SQLiteAsyncConnection(DatabasePath, Flags);

                await _connection.CreateTableAsync<RacasData>();     
                await _connection.CreateTableAsync<OrigensData>();   
                await _connection.CreateTableAsync<ClassesData>();   
                await _connection.CreateTableAsync<DivindadesData>();
                await _connection.CreateTableAsync<PersonagemData>();

                await DataBaseSeeder.SeedDataAsync(_connection);
            }
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