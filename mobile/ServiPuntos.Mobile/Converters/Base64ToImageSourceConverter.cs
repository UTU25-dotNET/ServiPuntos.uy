using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace ServiPuntos.Mobile.Converters
{
    public class Base64ToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string base64 && !string.IsNullOrWhiteSpace(base64))
            {
                try
                {

                    if (base64.StartsWith("data:image"))
                        return ImageSource.FromUri(new Uri(base64));

                    var bytes = System.Convert.FromBase64String(base64);
                    return ImageSource.FromStream(() => new System.IO.MemoryStream(bytes));
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
