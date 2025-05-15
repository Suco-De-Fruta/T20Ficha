using System.Globalization;
using System.Collections.Generic;

namespace T20FichaComDB.Converters
{
    public class OpcaoEscolhidaPoderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string poderNome && parameter is Dictionary<string, string> escolhasFeitas)
            {
                if (escolhasFeitas.TryGetValue(poderNome, out var escolha))
                {
                    return escolha; // Retorna a escolha se encontrada676
                }
                // Se a chave (poderNome) não existe no dicionário, também retorna string vazia
                return string.Empty;
            }
            // Se os tipos de 'value' ou 'parameter' não são os esperados, retorna string vazia
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

