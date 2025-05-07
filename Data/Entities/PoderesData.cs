using SQLite;

namespace T20FichaComDB.Data.Entities
{
    public enum TipoPoderEnum
    {
        Raca,
        Classe,
        Geral,
        Concedido,
        Origem,
        Tormenta
    }

    public class PoderesData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(150), Unique]
        public string Nome { get; set; }

        public string Descricao { get; set; }

        [MaxLength(50)]
        public string TipoPoder { get; set; }

        public string PreRequisitos { get; set; }

        public string Fonte { get; set; }

        [Ignore]
        public TipoPoderEnum TipoPoderEnum
        {
            get => Enum.TryParse<TipoPoderEnum>(TipoPoder, true, out var result) ? result : TipoPoderEnum.Geral;
            set => TipoPoder = value.ToString();
        }
    }
}
