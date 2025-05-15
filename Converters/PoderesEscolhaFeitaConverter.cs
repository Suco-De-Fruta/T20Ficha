using System.Globalization;
using T20FichaComDB.Data.Entities;

namespace T20FichaComDB.Converters
{
    public class PoderesEscolhaFeitaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string poderNome && parameter is Dictionary<string, string> escolhasFeitas)
            {
                return escolhasFeitas.ContainsKey(poderNome);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

