using SQLite;

namespace T20FichaComDB.Data.Entities
{
    public class PersonagemData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Nome { get; set; }

        [MaxLength(100)]
        public string JogadorNome { get; set; }

        public int Nivel { get; set; }

        // ARMAZENA DADOS
        [MaxLength(100)]
        public string RacaNome { get; set; }

        [MaxLength(100)]
        public string ClasseNome { get; set; }

        [MaxLength(100)]
        public string OrigemNome { get; set; }

        [MaxLength(100)]
        public string DivindadeNome { get; set; }

        // ATRIBUTOS    
        public int Forca { get; set; }
        public int Destreza { get; set; }
        public int Constituicao { get; set; }
        public int Inteligencia { get; set; }
        public int Sabedoria { get; set; }
        public int Carisma { get; set; }

        // PV E PM
        public int PVatuais { get; set; }
        public int PMatuais { get; set; }

        public DateTime UltimoSave { get; set; }
    }
}
