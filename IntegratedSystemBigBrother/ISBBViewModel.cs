﻿using System;
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
using System.Globalization;
using System.Windows.Markup;

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

        public static SynchronizationContext UIContext { get; set; }

        public static RoutedEvent CameraSelected;

        public static readonly CentralProcessor MainProc;

        public static readonly CameraScheduler Scheduler;

        //private static Camera _currentCamera;

        private static CameraListener _currentListener;

        public static readonly Dictionary<string, Storyboard> Animations;

        static ISBBViewModel()
        {
            MainProc = new CentralProcessor();

            Scheduler = new CameraScheduler(MainProc);

            Camera cam1 = new CameraWatchingFromLeftCornerWall(),
                   cam2 = new CameraWatchingFromWallCentre(),
                   cam3 = new CameraWatchingFromRightCornerWall();

            MainProc.Network.Add("Камера 1", new PeripheralProcessor(cam1, "Камера 1"));
            MainProc.Network.Add("Камера 2", new PeripheralProcessor(cam2, "Камера 2"));
            MainProc.Network.Add("Камера 3", new PeripheralProcessor(cam3, "Камера 3"));

            cam1.AddEmployeeArrivalToSchedule(TimeSpan.FromSeconds(25), "Иван Петрович");
            cam1.AddEmployeeDepartureToSchedule(TimeSpan.FromSeconds(15), "Семён Семёныч");
            cam1.AddOutsiderOnObjectToSchedule(TimeSpan.FromSeconds(25));

            
            cam2.AddOutsiderOnObjectToSchedule(TimeSpan.FromSeconds(20));
            cam2.AddStandardSituationToSchedule(TimeSpan.FromSeconds(10));
            cam2.AddEmployeeArrivalToSchedule(TimeSpan.FromSeconds(15), "Вадим Вадимыч");

            cam3.AddStandardSituationToSchedule(TimeSpan.FromSeconds(15));
            cam3.AddEmployeeArrivalToSchedule(TimeSpan.FromSeconds(15), "Потап Ефимыч");
            cam3.AddOutsiderOnObjectToSchedule(TimeSpan.FromSeconds(20));
            

            MainProc.CameraSelected += ShowCameraScreen;
            MainProc.BigBrotherClosedEye += TurnOffScreen;

            Animations = new Dictionary<string, Storyboard>();
        }

        private static void OnViewSet()
        {
            View.DataContext = MainProc;
            InitStaticResources();
            RunApp();
        }

        private static void InitStaticResources()
        {
            //CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage("ru-RU")));

            Path employee = Camera.DrawEmployee(),
                 outsider = Camera.DrawOutsider();

            View.Resources.Add("EmployeePath", employee);
            View.Resources.Add("OutsiderPath", outsider);

            Camera cam1 = new CameraWatchingFromLeftCornerWall(),
                   cam2 = new CameraWatchingFromWallCentre(),
                   cam3 = new CameraWatchingFromRightCornerWall();
            AddAnimationsFor(cam1);
            AddAnimationsFor(cam2);
            AddAnimationsFor(cam3);
        }

        private static void AddAnimationsFor(Camera camera)
        {
            Animations.Add($"{camera.TypeKey}/EmployeeArrival", camera.BuildEmployeeArrivalAnimation(TimeSpan.FromSeconds(10)));
            Animations.Add($"{camera.TypeKey}/EmployeeDeparture", camera.BuildEmployeeDepartureAnimation(TimeSpan.FromSeconds(10)));
            Animations.Add($"{camera.TypeKey}/OutsiderOnObject", camera.BuildOutsiderOnObjectAnimation(TimeSpan.FromSeconds(10)));
        }

        private static void RunApp()
        {
            Task.Run(MainProc.SurveyNetwork);

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(TimeSpan.FromMinutes(10));

            Task.Run(() => Scheduler.RunScheduleAsync(tokenSource.Token));
        }

        private static void DisposeListener()
        {
            _currentListener?.Dispose();
            _currentListener = null;
        }

        private static void TurnOffScreen()
        {
            DisposeListener();
            //UnsubscribeOnSchedulerEvents();
            //_currentListener = new CameraBackgroundListener(MainProc);
            DispatchScreen(() => ClearChildrenFromScreen());
            DispatchScreen(() => SetScreenBackground(Brushes.Black));
        }

        private static void ShowCameraScreen(PeripheralProcessor selectedPPU)
        {
            DisposeListener();
            //UnsubscribeOnSchedulerEvents();
            DispatchScreen(() => ClearChildrenFromScreen());
            DispatchScreen(() => SetScreenBackground(Brushes.White));
            //_currentCamera = selectedCamera;
            _currentListener = new CameraOnScreenListener(selectedPPU);
            Task.Run(_currentListener.Listen);
        }

        private static void SetScreenBackground(Brush brush)
        {
            View.Screen.Background = brush;
        }

        private static bool ScreenContainsObject(Path obj)
        {
            return View.Screen.Children.Contains(obj);
        }

        private static void AddChildOnScreen(Path obj, string objName)
        {
            View.Screen.Children.Add(obj);
        }

        private static void RemoveChildFromScreen(Path obj, string objName)
        {
            View.Screen.Children.Remove(obj);
        }

        private static void ClearChildrenFromScreen()
        {
            View.Screen.Children.Clear();
        }

        public static void ClearChildrenFromScreenDispatched()
        {
            DispatchScreen(() => View.Screen.Children.Clear());
        }

        public static void ShowCorridor(Camera selectedCamera)
        {
            DispatchScreen(() => AddChildOnScreen(selectedCamera.Corridor, "corr"));
        }

        public static void ShowEmployee(Camera selectedCamera)
        {
            Path employee = (Path)View.Resources["EmployeePath"];
            selectedCamera.Actor = employee;
            /*
            DispatchAnimation(
                    selectedCamera.Animation, 
                    () => selectedCamera.Animation.Completed += (sender, e) =>
                        { DispatchScreen(() => RemoveChildFromScreen(employee)); });
                        */
        }

        public static void ShowOutsider(Camera selectedCamera)
        {
            Path outsider = (Path)View.Resources["OutsiderPath"];
            selectedCamera.Actor = outsider;
            /*
            DispatchAnimation(
                    selectedCamera.Animation,
                    () => selectedCamera.Animation.Completed += (sender, e) =>
                    { DispatchScreen(() => RemoveChildFromScreen(outsider)); });*/
        }

        public static void AddActorOnScreen(Path actor, string actorName)
        {
            //DispatchScreen(() => AddChildOnScreen(actor, actorName));
            /*
             * Код добавления актёра пробрасывается в поток пользовательского интерфейса,
             * чтобы можно было отловить исключение, если добавить объект Path не удастся.
             */
            UIContext.Send((obj) => AddChildOnScreen(actor, actorName), null);
        }

        public static void RemoveActorFromScreen(Path actor, string actorName)
        {
            DispatchScreen(() => RemoveChildFromScreen(actor, actorName));
        }

        public static void DispatchScreen(Action action)
        {
            View.Screen.Dispatcher.Invoke(action);
        }

        public static T DispatchScreen<T>(Func<T> func)
        {
            return View.Screen.Dispatcher.Invoke(func);
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
            double[] columnWidthPercentage = new double[4] { 0.1, 0.2, 0.2, 0.5 };

            for (int i = 0; i < columnWidthPercentage.Length; i++)
                eventLogGridView.Columns[i].Width = eventLogActualWidth * columnWidthPercentage[i];
        }
    }
}
