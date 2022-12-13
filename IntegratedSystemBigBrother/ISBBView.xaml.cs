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
        public readonly CentralProcessor MainProc;

        public ISBBView()
        {
            InitializeComponent();

            ISBBViewModel.View = this;

            /*Camera cam = new CameraWatchingFromWallCentre();

            
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
            */
        }

        private void OnCameraSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ISBBViewModel.OnCameraSelectorSelectionChanged(sender, e);
        }

        private void OnEventLogSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ISBBViewModel.OnEventLogSizeChanged(sender, e);
        }
    }
}
