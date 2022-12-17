using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Threading;

namespace IntegratedSystemBigBrother
{
    /// <summary>
    /// Часть класса камеры, которая выполняет работу по настройке поведения камеры
    /// и по рисованию, также объявляются абстрактные методы рисования коридора и анимации актёра.
    /// </summary>
    public abstract partial class Camera
    {
        static Path DrawActor()
        {
            PathGeometry actorGeometry = new PathGeometry();

            PathFigure headAndBody = new PathFigure() { StartPoint = new Point(2, 6), IsFilled = true };
            headAndBody.Segments.Add(new LineSegment(new Point(2, 3), true));
            headAndBody.Segments.Add(new ArcSegment(new Point(2, 0), new Size(1.5, 1.5), 0, true, SweepDirection.Clockwise, true));
            headAndBody.Segments.Add(new ArcSegment(new Point(2, 3), new Size(1.5, 1.5), 0, false, SweepDirection.Clockwise, true));

            PathFigure arms = new PathFigure() { StartPoint = new Point(0, 6), IsFilled = false };
            arms.Segments.Add(new PolyLineSegment(new Point[] { new Point(2, 3), new Point(4, 6) }, true));

            PathFigure legs = new PathFigure() { StartPoint = new Point(0, 9), IsFilled = false };
            legs.Segments.Add(new PolyLineSegment(new Point[] { new Point(2, 6), new Point(4, 9) }, true));

            actorGeometry.Figures.Add(headAndBody);
            actorGeometry.Figures.Add(arms);
            actorGeometry.Figures.Add(legs);

            Path actor = new Path()
            {
                Data = actorGeometry,
                Fill = Brushes.White,
                LayoutTransform = new ScaleTransform(0.5, 0.5)
            };
            Grid.SetZIndex(actor, 1);
            return actor;
        }

        public static Path DrawEmployee()
        {
            Path employee = DrawActor();
            employee.Stroke = Brushes.Blue;
            return employee;
        }

        public static Path DrawOutsider()
        {
            Path outsider = DrawActor();
            outsider.Stroke = Brushes.Red;
            return outsider;
        }

        public abstract void DrawCorridor();

        public abstract Storyboard BuildEmployeeArrivalAnimation(TimeSpan duration);

        public abstract Storyboard BuildEmployeeDepartureAnimation(TimeSpan duration);

        public abstract Storyboard BuildOutsiderOnObjectAnimation(TimeSpan duration);

        protected static void SetDesiredFrameRateForStoryboard(Storyboard animations)
        {
            foreach (Timeline animation in animations.Children)
                Storyboard.SetDesiredFrameRate(animation, 30);
        }

        protected static void SetMarginActorProperty(Timeline animation)
        {
            Storyboard.SetTargetProperty(animation, new PropertyPath(Path.MarginProperty));
        }

        protected static void SetScaleXActorProperty(Timeline animation)
        {
            Storyboard.SetTargetProperty(animation, new PropertyPath($"LayoutTransform." + ScaleTransform.ScaleXProperty));
        }

        protected static void SetScaleYActorProperty(Timeline animation)
        {
            Storyboard.SetTargetProperty(animation, new PropertyPath($"LayoutTransform." + ScaleTransform.ScaleYProperty));
        }

        protected static void SetStrokeThicknessActorProperty(Timeline animation)
        {
            Storyboard.SetTargetProperty(animation, new PropertyPath(Path.StrokeThicknessProperty));
        }

        public async Task SetSendStandardSituationDataPackageBehavior(
            TimeSpan duration,
            params object[] additionParams)
        {
            SendPackage = () => new CameraStandardSituationDataPackage(DateTime.Now, IsFirstPackageInSeries);
            //await Task.Delay(duration).ConfigureAwait(false);
            return;
        }

        public async Task SetSendEmployeeArrivalDataPackageBehavior(
            TimeSpan duration,
            params object[] additionParams)
        {
            string employeeName = (string)additionParams[0];
            SendPackage = () => new CameraEmployeeArrivalDataPackage(DateTime.Now, IsFirstPackageInSeries, employeeName);
            Animation = ISBBViewModel.Animations[$"{this.TypeKey}/EmployeeArrival"];
            //ISBBViewModel.UIContext.Send((obj) => Animation = BuildEmployeeArrivalAnimation(TimeSpan.FromSeconds(10)), null);
            //Animation.RepeatBehavior = RepeatBehavior.Forever;
            //Actor.BeginStoryboard(Animation);
            //await Task.Delay(duration).ConfigureAwait(false);
            //Animation.Stop();
            return;
        }

        public async Task SetSendEmployeeDepartureDataPackageBehavior(
            TimeSpan duration,
            params object[] additionParams)
        {
            string employeeName = (string)additionParams[0];
            SendPackage = () => new CameraEmployeeDepartureDataPackage(DateTime.Now, IsFirstPackageInSeries, employeeName);
            Animation = ISBBViewModel.Animations[$"{this.TypeKey}/EmployeeDeparture"];
            //ISBBViewModel.UIContext.Send((obj) => Animation = BuildEmployeeDepartureAnimation(TimeSpan.FromSeconds(10)), null);
            //Animation.RepeatBehavior = RepeatBehavior.Forever;
            //Animation.RepeatBehavior = RepeatBehavior.Forever;
            //Actor.BeginStoryboard(Animation);
            //await Task.Delay(duration).ConfigureAwait(false);
            //Animation.Stop();
            return;
        }

        public async Task SetSendOutsiderOnObjectDataPackageBehavior(
            TimeSpan duration,
            params object[] additionParams)
        {
            SendPackage = () => new CameraOutsiderOnObjectDataPackage(DateTime.Now, IsFirstPackageInSeries);
            Animation = ISBBViewModel.Animations[$"{this.TypeKey}/OutsiderOnObject"];
            //ISBBViewModel.UIContext.Send((obj) => Animation = BuildOutsiderOnObjectAnimation(TimeSpan.FromSeconds(10)), null);
            //Animation.RepeatBehavior = RepeatBehavior.Forever;
            //Actor.BeginStoryboard(Animation);
            //await Task.Delay(duration).ConfigureAwait(false);
            //Animation.Stop();
            return;
        }
    }
}
