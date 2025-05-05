using SQLite;

namespace T20FichaComDB.Data.Entities
{
    public class RacasData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Nome { get; set; }

        public int ModForca { get; set; }
        public int ModDestreza { get; set; }
        public int ModConstituicao { get; set; }
        public int ModInteligencia { get; set; }
        public int ModSabedoria { get; set; }
        public int ModCarisma { get; set; }

        public int ModLivres {  get; set; }
        public string DescricaoModLivres { get; set; }
        public string ExcecoesModLivres { get; set; }
    }
}
