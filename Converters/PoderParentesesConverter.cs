using System.Globalization;

namespace T20FichaComDB.Converters
{
    public class PoderParentesesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 3 &&
                values[0] is string poderNome &&
                values[1] is Dictionary<string, string> escolhasFeitas &&
                values[2] is string parentesesTipo)
            {
                if (escolhasFeitas.ContainsKey(poderNome))
                {
                    return parentesesTipo == "Abrir" ? " (" : ")";
                }
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}