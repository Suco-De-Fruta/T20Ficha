using SQLite;

namespace T20FichaComDB.Data.Entities
{
    public class DivindadesData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100), Unique]
        public string Nome { get; set; }
        public enum PanteaoEnum
        {
            Maior,
            Menor
        }

        public PanteaoEnum Panteao { get; set; }

        public int? StatusDivino { get; set; }
    }
}
