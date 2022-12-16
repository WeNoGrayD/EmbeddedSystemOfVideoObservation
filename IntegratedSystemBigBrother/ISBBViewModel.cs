using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public static readonly CameraScheduler Scheduler;

        //private static Camera _currentCamera;

        private static CameraListener _currentListener;

        static ISBBViewModel()
        {
            MainProc = new CentralProcessor();

            CameraScheduler Scheduler = new CameraScheduler(MainProc);

            Camera cam1 = new CameraWatchingFromLeftCornerWall(),
                   cam2 = new CameraWatchingFromWallCentre(),
                   cam3 = new CameraWatchingFromRightCornerWall();

            MainProc.Network.Add("Камера 1", new PeripheralProcessor(cam1, "Камера 1"));
            MainProc.Network.Add("Камера 2", new PeripheralProcessor(cam2, "Камера 2"));
            MainProc.Network.Add("Камера 3", new PeripheralProcessor(cam3, "Камера 3"));

            cam1.AddEmployeeArrivalToSchedule(TimeSpan.FromSeconds(3), "Иван Петрович");
            cam1.AddEmployeeDepartureToSchedule(TimeSpan.FromSeconds(5), "Семён Семёныч");
            cam1.AddOutsiderOnObjectToSchedule(TimeSpan.FromSeconds(4));

            cam2.AddEmployeeDepartureToSchedule(TimeSpan.FromSeconds(4), "Олег Егорыч");
            cam2.AddStandardSituationToSchedule(TimeSpan.FromSeconds(3));
            cam2.AddEmployeeArrivalToSchedule(TimeSpan.FromSeconds(3), "Вадим Вадимыч");

            cam3.AddStandardSituationToSchedule(TimeSpan.FromSeconds(4));
            cam3.AddEmployeeArrivalToSchedule(TimeSpan.FromSeconds(4), "Потап Ефимыч");
            cam3.AddOutsiderOnObjectToSchedule(TimeSpan.FromSeconds(6));

            MainProc.CameraSelected += ShowCameraScreen;
            MainProc.BigBrotherClosedEye += TurnOffScreen;

            Task.Run(MainProc.SurveyNetwork);
            using (CancellationTokenSource tokenSource = new CancellationTokenSource())
            {
                tokenSource.CancelAfter(TimeSpan.FromMinutes(10));
                Task.Run(() => Scheduler.RunScheduleAsync(tokenSource.Token));
            }
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
        }

        private static void TurnOffScreen()
        {
            DisposeListener();
            //_currentListener = new CameraBackgroundListener(MainProc);
            View.Screen.Dispatch(() => ClearChildrenFromScreen());
            View.Screen.Dispatch(() => SetScreenBackground(Brushes.Black));
        }

        private static void ShowCameraScreen(Camera selectedCamera)
        {
            DisposeListener();
            View.Screen.Dispatch(() => ClearChildrenFromScreen());
            View.Screen.Dispatch(() => SetScreenBackground(Brushes.Transparent));
            //_currentCamera = selectedCamera;
            _currentListener = new CameraOnScreenListener(selectedCamera);
            Task.Run((Action)_currentListener.Listen);
        }

        private static void SetScreenBackground(Brush brush)
        {
            View.Screen.Background = brush;
        }

        private static bool ScreenContainsObject(Path obj)
        {
            return View.Screen.Children.Contains(obj);
        }

        private static void AddChildOnScreen(Path obj, string name)
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

        public static void ShowCorridor(Camera selectedCamera)
        {
            View.Screen.Dispatch(() => AddChildOnScreen(selectedCamera.Corridor, "corr"));
        }

        public static void ShowEmployee(Camera selectedCamera)
        {
            if (!(bool)View.Screen.Dispatch(() => ScreenContainsObject(selectedCamera.Actor)))
            {
                Path employee = (Path)View.Resources["EmployeePath"];
                selectedCamera.Actor = employee;
                View.Screen.Dispatch(() => AddChildOnScreen(employee, "emp"));
                DispatchAnimation(
                        selectedCamera.Animation, 
                        () => selectedCamera.Animation.Completed += (sender, e) =>
                            { View.Screen.Dispatch(() => RemoveChildFromScreen(employee)); });
            }
            Scheduler.CameraBehaviorStart += StartObserve;
        }

        public static void ShowOutsider(Camera selectedCamera)
        {
            if (!(bool)View.Screen.Dispatch(() => ScreenContainsObject(selectedCamera.Actor)))
            {
                Path outsider = (Path)View.Resources["OutsiderPath"];
                View.Screen.Dispatch(() => AddChildOnScreen(outsider, "out"));
                selectedCamera.Animation.Completed += (sender, e) =>
                { View.Screen.Dispatch(() => RemoveChildFromScreen(outsider)); };
            }
        }

        private static async Task StartObserve(PeripheralProcessor ppu, CameraBehaviorRecord behaviorRecord)
        {
            ppu.AgregatedCamera.StartObserve();
        }

        private static async Task StopObserve(PeripheralProcessor ppu, CameraBehaviorRecord behaviorRecord)
        {
            ppu.AgregatedCamera.StopObserve();
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
