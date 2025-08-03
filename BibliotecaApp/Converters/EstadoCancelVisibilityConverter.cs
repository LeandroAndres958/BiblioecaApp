using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BibliotecaApp.Converters
{
    public class EstadoCancelVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string estado = value as string;
            if (estado == null)
                return Visibility.Collapsed;

            // Mostrar botón Cancelar solo si el estado es "prestado"
            if (estado.Equals("activo", StringComparison.OrdinalIgnoreCase))
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

