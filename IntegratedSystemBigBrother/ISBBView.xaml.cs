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

            Screen.Children.Add(Camera.Corridors[1]);
            Camera.Corridors[1].Stroke = Brushes.Black;
            Camera.Corridors[1].StrokeThickness = 0.05;

            Path actor = Camera.DrawOutsider();
            Screen.Children.Add(actor);
            Canvas.SetTop(actor, 20);
            Canvas.SetLeft(actor, 20);
            actor.StrokeThickness = 0.1;
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
