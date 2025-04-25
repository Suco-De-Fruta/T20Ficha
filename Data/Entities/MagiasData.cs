using SQLite;

namespace T20FichaComDB.Data.Entities
{
    public enum TipoMagia
    {
        Arcana,
        Divina,
        Universal
    }
    public class MagiasData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(150)]
        public string Nome { get; set; }

        public int Circulo { get; set; }

        [MaxLength(20)]
        public string Tipo { get; set; }

        [MaxLength(50)]
        public string Execucao { get; set; }

        [MaxLength(50)]
        public string Alcance { get; set; }

        [MaxLength(50)]
        public string Duracao { get; set; }

        [MaxLength(200)]
        public string AlvoAreaEfeito { get; set; }

        [MaxLength(100)]
        public string Resistencia { get; set; }

        [MaxLength(1000)]
        public string Descricao { get; set; }

        [Ignore]
        public TipoMagia TipoEnum
        {
            get => Enum.TryParse<TipoMagia>(Tipo, true, out var result) ? result : TipoMagia.Universal;
            set => Tipo = value.ToString();
        }

    }
}
