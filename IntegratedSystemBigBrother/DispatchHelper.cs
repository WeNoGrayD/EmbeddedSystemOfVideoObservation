using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IntegratedSystemBigBrother
{
    public static class DispatchHelper
    {
        public static void Dispatch<T>(this T dispatchedElement, Action action)
            where T : UIElement
        {
            dispatchedElement.Dispatcher.Invoke(action);
        }

        public static object Dispatch<T>(this T dispatchedElement, Func<object> func)
            where T : UIElement
        {
            return dispatchedElement.Dispatcher.Invoke(func);
        }
    }
}
