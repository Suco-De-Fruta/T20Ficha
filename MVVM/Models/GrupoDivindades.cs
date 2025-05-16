using System.Collections.ObjectModel;
using T20FichaComDB.Data.Entities;

namespace T20FichaComDB.MVVM.Models
{
    public class GrupoDivindade : ObservableCollection<DivindadesData>
    {
        public string NomeDoGrupo { get; private set; }

        public GrupoDivindade(string nomeDoGrupo, IEnumerable<DivindadesData> divindades)
            : base(divindades)
        {
            NomeDoGrupo = nomeDoGrupo;
        }
    }
}