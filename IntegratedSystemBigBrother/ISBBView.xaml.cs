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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IntegratedSystemBigBrother
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class ISBBView : Window
    {
        public ISBBView()
        {
            InitializeComponent();

            Camera cam = new CameraWatchingFromWallCentre();

            Screen.Children.Add(cam.Corridor);

            Path employee = Camera.DrawEmployee(),
                 outsider = Camera.DrawOutsider();
            Screen.Children.Add(employee);
            //Screen.Children.Add(outsider);
            //Canvas.SetTop(actor, 20);
            //Canvas.SetLeft(actor, 20);
            //actor.Margin = new Thickness(8, 1, 0, 0);
            //employee.BeginStoryboard(cam.BuildEmployeeArrivalAnimation(employee, TimeSpan.FromSeconds(10), Screen));
            employee.BeginStoryboard(cam.BuildEmployeeDepartureAnimation(employee, TimeSpan.FromSeconds(10), Screen));
            //outsider.BeginStoryboard(cam.BuildOutsiderOnObjectAnimation(outsider, TimeSpan.FromSeconds(10), Screen));
        }

        private void EventLogSizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridView eventLogGridView = (GridView)EventLog.View;
            double eventLogActualWidth = EventLog.ActualWidth;
            double[] columnWidthPercentage = new double[4] { 0.08, 0.21, 0.31, 0.4 };

            for (int i = 0; i < columnWidthPercentage.Length; i++)
                eventLogGridView.Columns[i].Width = eventLogActualWidth * columnWidthPercentage[i];
        }
    }
}
