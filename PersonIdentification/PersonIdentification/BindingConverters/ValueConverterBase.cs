using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace PersonIdentification.BindingConverters
{
    public abstract class OneWayValueConverterBase : IValueConverter
    {
        public abstract object Convert(object value, Type targetType, object parameter, string language);

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException();
        }
    }
}
