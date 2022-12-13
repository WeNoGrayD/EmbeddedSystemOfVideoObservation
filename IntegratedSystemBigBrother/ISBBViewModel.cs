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
    public class ISBBViewModel
    {
        private static ISBBView _view;
        public static ISBBView View
        {
            get { return _view; }
            set
            {
                _view = value;
                OnViewSet();
            }
        }

        public static readonly CentralProcessor MainProc;

        private static Camera _currentCamera;

        private static CameraListener _currentListener;

        static ISBBViewModel()
        {
            MainProc = new CentralProcessor();

            Camera cam1 = new CameraWatchingFromLeftCornerWall();
            cam1.AddEmployeeArrivalToSchedule(TimeSpan.FromSeconds(7), "Иван Петрович");
            MainProc.Network.Add("Камера 1", new PeripheralProcessor(cam1, "Камера 1"));
            MainProc.Network.Add("Камера 2", new PeripheralProcessor(new CameraWatchingFromWallCentre(), "Камера 2"));
            MainProc.Network.Add("Камера 3", new PeripheralProcessor(new CameraWatchingFromRightCornerWall(), "Камера 3"));

            MainProc.CameraSelected += ShowCameraScreen;
            MainProc.BigBrotherClosedEye += TurnOffScreen;

            Task.Run(MainProc.SurveyNetwork);
        }

        private static void OnViewSet()
        {
            View.DataContext = MainProc;
            InitStaticResources();
        }

        private static void InitStaticResources()
        {
            Path employee = Camera.DrawEmployee(),
                 outsider = Camera.DrawOutsider();

            View.Resources.Add("EmployeePath", employee);
            View.Resources.Add("OutsiderPath", outsider);
        }

        private static void DisposeListener()
        {
            _currentListener?.Dispose();
            _currentListener = null;
        }

        private static void TurnOffScreen()
        {
            DisposeListener();
            View.Screen.Dispatch(() => SetScreenBackground(Brushes.Black));
        }

        private static void ShowCameraScreen(Camera selectedCamera)
        {
            DisposeListener();
            View.Screen.Dispatch(() => ClearChildrenFromScreen());
            View.Screen.Dispatch(() => SetScreenBackground(Brushes.Transparent));
            _currentCamera = selectedCamera;
            ShowCorridor();
            _currentListener = new CameraListener(selectedCamera);
            Task.Run((Action)_currentListener.ListenCamera);
        }

        private static void SetScreenBackground(Brush brush)
        {
            View.Screen.Background = brush;
        }

        private static bool ScreenContainsObject(Path obj)
        {
            return View.Screen.Children.Contains(obj);
        }

        private static void AddChildOnScreen(Path obj)
        {
            View.Screen.Children.Add(obj);
        }

        private static void RemoveChildFromScreen(Path obj)
        {
            View.Screen.Children.Remove(obj);
        }

        private static void ClearChildrenFromScreen()
        {
            View.Screen.Children.Clear();
        }

        private static void ShowCorridor()
        {
            View.Screen.Dispatch(() => AddChildOnScreen(_currentCamera.Corridor));
        }

        public static void ShowEmployee()
        {
            if (!(bool)View.Screen.Dispatch(() => ScreenContainsObject(_currentCamera.Actor)))
            {
                Path employee = (Path)View.Resources["EmployeePath"];
                View.Screen.Dispatch(() => AddChildOnScreen(employee));
                DispatchAnimation(
                        _currentCamera.Animation, 
                        () => _currentCamera.Animation.Completed += (sender, e) =>
                            { View.Screen.Dispatch(() => RemoveChildFromScreen(employee)); });
            }
        }

        public static void ShowOutsider()
        {
            if (!(bool)View.Screen.Dispatch(() => ScreenContainsObject(_currentCamera.Actor)))
            {
                Path outsider = (Path)View.Resources["OutsiderPath"];
                View.Screen.Dispatch(() => AddChildOnScreen(outsider));
                _currentCamera.Animation.Completed += (sender, e) =>
                { View.Screen.Dispatch(() => RemoveChildFromScreen(outsider)); };
            }
        }

        public static void DispatchActor(Path actor, Action action)
        {
            actor.Dispatcher.Invoke(action);
        }

        public static void DispatchAnimation(Storyboard sb, Action action)
        {
            sb.Dispatcher.Invoke(action);
        }

        public static void OnCameraSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string cameraName = (string)e.AddedItems[0];
            MainProc.OnCameraSelected(cameraName);
        }

        public static void OnEventLogSizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridView eventLogGridView = (GridView)ISBBViewModel.View.EventLog.View;
            double eventLogActualWidth = ISBBViewModel.View.EventLog.ActualWidth;
            double[] columnWidthPercentage = new double[4] { 0.08, 0.21, 0.31, 0.4 };

            for (int i = 0; i < columnWidthPercentage.Length; i++)
                eventLogGridView.Columns[i].Width = eventLogActualWidth * columnWidthPercentage[i];
        }
    }
}
