using SQLite;
using System.Text.Json;
using T20FichaComDB.Data.Entities;

namespace T20FichaComDB.Data
{
    public static class DataBaseSeeder
    {
        public static async Task SeedDataAsync(SQLiteAsyncConnection connection)
        {
            await SeedRacasAsync(connection);
            await SeedClassesAsync(connection);
            await SeedOrigensAsync(connection);
            await SeedDivindadesAsync(connection);
            await SeedMagiasAsync(connection);
        }

        private static async Task SeedRacasAsync(SQLiteAsyncConnection connection)
        {
            if (await connection.Table<RacasData>().CountAsync() == 0)
            {
                var racasBase = new List<RacasData>
                {
                    new RacasData { Nome = "Humano" }, new RacasData { Nome = "Anão" }, new RacasData { Nome = "Dahllan" },
                    new RacasData { Nome = "Elfo" }, new RacasData { Nome = "Goblin" }, new RacasData { Nome = "Minotauro" },
                    new RacasData { Nome = "Qareen" }, new RacasData { Nome = "Golem" }, new RacasData { Nome = "Hynne" },
                    new RacasData { Nome = "Kliren" }, new RacasData { Nome = "Medusa" }, new RacasData { Nome = "Osteon" },
                    new RacasData { Nome = "Sereia/Tritão" }, new RacasData { Nome = "Sílfide" }, new RacasData { Nome = "Suraggel" },
                    new RacasData { Nome = "Trog" }, new RacasData { Nome = "Bugbear" }, new RacasData { Nome = "Centauro" },
                    new RacasData { Nome = "Ceratops" }, new RacasData { Nome = "Elfo-do-Mar" }, new RacasData { Nome = "Finntroll" },
                    new RacasData { Nome = "Gnoll" }, new RacasData { Nome = "Harpia" }, new RacasData { Nome = "Hobgoblin" },
                    new RacasData { Nome = "Kallyanach" }, new RacasData { Nome = "Kaijin" }, new RacasData { Nome = "Kappa" },
                    new RacasData { Nome = "Kobolds" }, new RacasData { Nome = "Mashin" }, new RacasData { Nome = "Meio-orc" },
                    new RacasData { Nome = "Minauro" }, new RacasData { Nome = "Moreau" }, new RacasData { Nome = "Nagah" },
                    new RacasData { Nome = "Nezumi" }, new RacasData { Nome = "Ogro" }, new RacasData { Nome = "Orc" },
                    new RacasData { Nome = "Pteros" }, new RacasData { Nome = "Soterrado" }, new RacasData { Nome = "Tabrachi" },
                    new RacasData { Nome = "Tengu" }, new RacasData { Nome = "Trog anão" }, new RacasData { Nome = "Velocis" },
                    new RacasData { Nome = "Voracis" }, new RacasData { Nome = "Yidishan" }, new RacasData { Nome = "Duende" },
                    new RacasData { Nome = "Eiradaan" }, new RacasData { Nome = "Galokk" }, new RacasData { Nome = "Meio-Elfo" },
                    new RacasData { Nome = "Sátiro" }
                };
                await connection.InsertAllAsync(racasBase);
            }
        }

        private static async Task SeedClassesAsync(SQLiteAsyncConnection connection)
        {
            if (await connection.Table<ClassesData>().CountAsync() == 0)
            {
                var classesBase = new List<ClassesData>
                {
                    new ClassesData { Nome = "Arcanista" }, new ClassesData { Nome = "Bárbaro" }, new ClassesData { Nome = "Bardo" },
                    new ClassesData { Nome = "Bucaneiro" }, new ClassesData { Nome = "Caçador" }, new ClassesData { Nome = "Cavaleiro" },
                    new ClassesData { Nome = "Clérigo" }, new ClassesData { Nome = "Druida" }, new ClassesData { Nome = "Guerreiro" },
                    new ClassesData { Nome = "Ladino" }, new ClassesData { Nome = "Lutador" }, new ClassesData { Nome = "Nobre" },
                    new ClassesData { Nome = "Paladino" }, new ClassesData { Nome = "Treinador" }, new ClassesData { Nome = "Frade" },
                    new ClassesData { Nome = "Inventor" }, new ClassesData { Nome = "Inventor Alquimista" }, new ClassesData { Nome = "Lutador Atleta" },
                    new ClassesData { Nome = "Nobre Burguês" }, new ClassesData { Nome = "Bucaneiro Duelista" }, new ClassesData { Nome = "Druida Ermitão" },
                    new ClassesData { Nome = "Gurreiro Inovador" }, new ClassesData { Nome = "Bárbaro Machado de Pedra" }, new ClassesData { Nome = "Bardo Magimarcialista" },
                    new ClassesData { Nome = "Arcanista Necromante" }, new ClassesData { Nome = "Paladino Santo" }, new ClassesData { Nome = "Caçador Seteiro" },
                    new ClassesData { Nome = "Clérigo Usurpador" }, new ClassesData { Nome = "Cavaleiro Vassalo" }, new ClassesData { Nome = "Ladino Ventanista" },
                };
                await connection.InsertAllAsync(classesBase);
            }
        }

        private static async Task SeedDivindadesAsync(SQLiteAsyncConnection connection)
        {
            if (await connection.Table<DivindadesData>().CountAsync() == 0)
            {
                var divindadesBase = new List<DivindadesData>
                {
                    new DivindadesData { Nome = "Aharadak" }, new DivindadesData { Nome = "Allihanna" }, new DivindadesData { Nome = "Arsenal" },
                    new DivindadesData { Nome = "Azgher" }, new DivindadesData { Nome = "Hyninn" }, new DivindadesData { Nome = "Kallyadranoch" },
                    new DivindadesData { Nome = "Khalmyr" }, new DivindadesData { Nome = "Lena" }, new DivindadesData { Nome = "Lin-Wu" },
                    new DivindadesData { Nome = "Marah" }, new DivindadesData { Nome = "Megalokk" }, new DivindadesData { Nome = "Nimb" },
                    new DivindadesData { Nome = "Oceano" }, new DivindadesData { Nome = "Sszzaas" }, new DivindadesData { Nome = "Tanna-Toh" },
                    new DivindadesData { Nome = "Tenebra" }, new DivindadesData { Nome = "Thwor" }, new DivindadesData { Nome = "Thyatis" },
                    new DivindadesData { Nome = "Valkaria" }, new DivindadesData { Nome = "Wynna" }, new DivindadesData { Nome = "Gwendolynn" },
                    new DivindadesData { Nome = "Mauziell" }, new DivindadesData { Nome = "Tibar" }
                };
                await connection.InsertAllAsync(divindadesBase);
            }
        }

        private static async Task SeedOrigensAsync(SQLiteAsyncConnection connection)
        { 
            if (await connection.Table<OrigensData>().CountAsync() == 0)
            {
                var origensBase = new List<OrigensData>
                {
                    new OrigensData { Nome = "Acólito" }, new OrigensData { Nome = "Amigo dos Animais" }, new OrigensData { Nome = "Amnésico" },
                    new OrigensData { Nome = "Artesão" }, new OrigensData { Nome = "Artista" }, new OrigensData { Nome = "Assistente de Laboratório" },
                    new OrigensData { Nome = "Batedor" }, new OrigensData { Nome = "Capanga" }, new OrigensData { Nome = "Charlatão" },
                    new OrigensData { Nome = "Circense" }, new OrigensData { Nome = "Criminoso" }, new OrigensData { Nome = "Curandeiro" },
                    new OrigensData { Nome = "Eremita" }, new OrigensData { Nome = "Escravo" }, new OrigensData { Nome = "Estudioso" },
                    new OrigensData { Nome = "Fazendeiro" }, new OrigensData { Nome = "Forasteiro" }, new OrigensData { Nome = "Gladiador" },
                    new OrigensData { Nome = "Guarda" }, new OrigensData { Nome = "Herdeiro" }, new OrigensData { Nome = "Herói Camponês" },
                    new OrigensData { Nome = "Marujo" }, new OrigensData { Nome = "Mateiro" }
                };
                await connection.InsertAllAsync(origensBase);
            }
        }

        private static async Task SeedMagiasAsync (SQLiteAsyncConnection connection)
        {
            if (await connection.Table<MagiasData>().CountAsync() == 0)
            {
                System.Diagnostics.Debug.WriteLine("--- SeedMagiasAsync: Tabela de Magias vazia, tentando popular... ---");

                List<MagiasData> magiasBase = null;
                try
                {
                    string jsonFileName = "Data/Seeds/MagiasDatabase.json";
                    System.Diagnostics.Debug.WriteLine($"--- SeedMagiasAsync: Tentando ler o arquivo: {jsonFileName} ---");
                    using var stream = await FileSystem.OpenAppPackageFileAsync(jsonFileName);
                    if (stream == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"--- ERRO SeedMagiasAsync: Não foi possível encontrar o arquivo {jsonFileName} no pacote. Verifique a Build Action! ---");
                        return;
                    }

                    using var reader = new StreamReader(stream);
                    string jsonContent = await reader.ReadToEndAsync();
                    System.Diagnostics.Debug.WriteLine($"--- SeedMagiasAsync: Conteúdo JSON lido (primeiros 200 chars): {jsonContent.Substring(0, Math.Min(jsonContent.Length, 200))} ---");

                    magiasBase = JsonSerializer.Deserialize<List<MagiasData>>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    System.Diagnostics.Debug.WriteLine($"--- SeedMagiasAsync: JSON desserializado. Número de magias encontradas: {magiasBase?.Count ?? 0} ---");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"--- ERRO FATAL SeedMagiasAsync: Falha ao ler/desserializar JSON: {ex.ToString()} ---");
                    return;
                }

                if (magiasBase != null && magiasBase.Any())
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"--- SeedMagiasAsync: Tentando inserir {magiasBase.Count} magias no banco... ---");
                        await connection.InsertAllAsync(magiasBase);
                        System.Diagnostics.Debug.WriteLine($"--- SeedMagiasAsync: {magiasBase.Count} magias INSERIDAS com sucesso! ---");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("--- SeedMagiasAsync: Nenhuma magia encontrada no arquivo JSON ou erro na desserialização. Banco não populado. ---");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("--- SeedMagiasAsync: Tabela de Magias já contém dados. Pulo a população. ---");
                }
            }
        }
    }
}
