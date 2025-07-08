using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace ServiPuntos.Mobile.Converters
{
    /// <summary>
    /// Devuelve true si la cadena no es null ni vacía, false en caso contrario.
    /// </summary>
    public class StringNotNullOrEmptyConverter : IValueConverter
    {
        // De la fuente al destino (ViewModel → UI)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            return !string.IsNullOrEmpty(s);
        }

        // No se usa en este escenario, pero lo dejamos implementado
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(StringNotNullOrEmptyConverter)} no soporta ConvertBack.");
        }
    }
}
