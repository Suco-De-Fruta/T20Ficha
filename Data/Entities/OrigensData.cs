using SQLite;

namespace T20FichaComDB.Data.Entities
{
    public class OrigensData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100), Unique]
        public string Nome { get; set; }
    }
}
