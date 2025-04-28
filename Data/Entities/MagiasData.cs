using SQLite;

namespace T20FichaComDB.Data.Entities
{
    public enum TipoMagia
    {
        Arcana,
        Divina,
        Universal
    }

    public enum EscolaMagia
    {
        Abjuracao,
        Adivinhacao,
        Convocacao,
        Encantamento,
        Evocacao,
        Ilusao,
        Necromancia,
        Transmutacao,
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

        [MaxLength(30)]
        public string Escola { get; set; }

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

        public string Descricao { get; set; }

        [Ignore]
        public TipoMagia TipoEnum
        {
            get => Enum.TryParse<TipoMagia>(Tipo, true, out var result) ? result : TipoMagia.Universal;
            set => Tipo = value.ToString();
        }

        [Ignore]
        public EscolaMagia EscolaEnum
        {
            get
            {
                return Escola?.ToLower() switch
                {
                    "abjuração" or "abjur" => EscolaMagia.Abjuracao,
                    "adivinhação" or "adiv" => EscolaMagia.Adivinhacao,
                    "convocação" or "conv" => EscolaMagia.Convocacao,
                    "encantamento" or "encan" => EscolaMagia.Encantamento,
                    "evocação" or "evoc" => EscolaMagia.Evocacao,
                    "ilusão" => EscolaMagia.Ilusao,
                    "necromancia" or "necro" => EscolaMagia.Necromancia,
                    "transmutação" or "trans" => EscolaMagia.Transmutacao,
                };
            }
            set => Escola = value.ToString();
        }
    }
}
