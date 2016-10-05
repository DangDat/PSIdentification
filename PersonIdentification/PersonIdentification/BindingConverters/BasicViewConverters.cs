using Microsoft.Band;
using PersonIdentification.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonIdentification.BindingConverters
{
    public class ConnectCommandTextConverter : OneWayValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert((IBandClient)value);
        }

        public string Convert(IBandClient client)
        {
            if (client == null)
            {
                return "Connect";
            }
            else
            {
                return "Disconnect";
            }
        }
    }

    public class ConnectCommandEnabledConverter : OneWayValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert((BasicsModel)value);
        }

        public bool Convert(BasicsModel vm)
        {
            if (vm.Connecting)
            {
                return false;
            }
            else
            {
                return (vm.Main.BandClient != null || vm.SelectedDevice != null);
            }
        }
    }
}
