using SQLite;

namespace T20FichaComDB.Data.Entities
{
    public class RacasData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Nome { get; set; }
    }
}
