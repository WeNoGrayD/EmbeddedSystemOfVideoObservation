using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;

namespace IntegratedSystemBigBrother
{
    public class CameraMessageImportanceEnumToImageConverter : IValueConverter
    {
        private static readonly Dictionary<CameraDataPackage.EventDescription, string> _matching =
            new Dictionary<CameraDataPackage.EventDescription, string>()
            {
                { CameraDataPackage.EventDescription.EmployeeArrival, "warning.png" },
                { CameraDataPackage.EventDescription.EmployeeDeparture, "warning.png" },
                { CameraDataPackage.EventDescription.OutsiderOnObject, "alert.png" },
            };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CameraDataPackage.EventDescription eventDescription = (CameraDataPackage.EventDescription)value;
            return new BitmapImage(new Uri($"../../Images/{_matching[eventDescription]}", UriKind.Relative));
            //return ((DateTime)value).ToString("dd.MM.yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
