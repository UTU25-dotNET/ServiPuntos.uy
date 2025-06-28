using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace ServiPuntos.Mobile.Converters
{
    public class TipoToButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tipo = value as string;
            return tipo == "Oferta" ? "Comprar" : "Canjear";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
