using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace PersonIdentification.BindingConverters
{
    public class BoolToVisibilityConverter : OneWayValueConverterBase
    {
        public enum Mapping
        {
            FalseCollapsed,
            TrueCollapsed
        }

        public Mapping DefaultMapping
        {
            get;
            set;
        }

        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert((bool)value, parameter != null && parameter is Mapping ? (Mapping)parameter : DefaultMapping);
        }

        public Visibility Convert(bool value, Mapping parameter)
        {
            switch (parameter)
            {
                default:
                case Mapping.FalseCollapsed:
                    return value ? Visibility.Visible : Visibility.Collapsed;

                case Mapping.TrueCollapsed:
                    return !value ? Visibility.Visible : Visibility.Collapsed;

            }
        }
    }

    public class NullToVisibilityConverter : OneWayValueConverterBase
    {
        public enum Mapping
        {
            NullCollapsed,
            NotNullCollapsed
        }

        public Mapping DefaultMapping
        {
            get;
            set;
        }

        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value, parameter != null && parameter is Mapping ? (Mapping)parameter : DefaultMapping);
        }

        public Visibility Convert(object value, Mapping parameter)
        {
            switch (parameter)
            {
                default:
                case Mapping.NullCollapsed:
                    return value != null ? Visibility.Visible : Visibility.Collapsed;

                case Mapping.NotNullCollapsed:
                    return value == null ? Visibility.Visible : Visibility.Collapsed;

            }
        }
    }

    public class BoolNotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }
    }

    public class NullToBoolConverter : OneWayValueConverterBase
    {
        public enum Mapping
        {
            NullFalse,
            NullTrue
        }

        public Mapping DefaultMapping
        {
            get;
            set;
        }

        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value, parameter != null && parameter is Mapping ? (Mapping)parameter : DefaultMapping);
        }

        public bool Convert(object value, Mapping parameter)
        {
            switch (parameter)
            {
                default:
                case Mapping.NullFalse:
                    return (value != null);

                case Mapping.NullTrue:
                    return (value == null);


            }
        }
    }
}
