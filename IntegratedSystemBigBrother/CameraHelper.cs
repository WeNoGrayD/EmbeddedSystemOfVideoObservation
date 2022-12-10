using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace IntegratedSystemBigBrother
{
    /// <summary>
    /// Часть класса камеры, которая выполняет работу по настройке поведения камеры.
    /// </summary>
    public partial class Camera
    {
        public static Path[] Corridors;

        static Camera()
        {
            Corridors = new Path[3];
            DrawCorridor1();
            DrawCorridor2();
            DrawCorridor3();
        }

        static Path DrawActor()
        {
            PathGeometry actorGeometry = new PathGeometry();

            PathFigure headAndBody = new PathFigure() { StartPoint = new Point(2, 6)};
            headAndBody.Segments.Add(new LineSegment(new Point(2, 3), true));
            headAndBody.Segments.Add(new ArcSegment(new Point(2, 0), new Size(1.5, 1.5), 0, true, SweepDirection.Clockwise, true));
            headAndBody.Segments.Add(new ArcSegment(new Point(2, 3), new Size(1.5, 1.5), 0, false, SweepDirection.Clockwise, true));

            PathFigure arms = new PathFigure() { StartPoint = new Point(0, 6) };
            arms.Segments.Add(new PolyLineSegment(new Point[] { new Point(2, 3), new Point(4, 6) }, true));

            PathFigure legs = new PathFigure() { StartPoint = new Point(0, 9) };
            legs.Segments.Add(new PolyLineSegment(new Point[] { new Point(2, 6), new Point(4, 9) }, true));

            actorGeometry.Figures.Add(headAndBody);
            actorGeometry.Figures.Add(arms);
            actorGeometry.Figures.Add(legs);

            Path actor = new Path() { Data = actorGeometry, RenderTransform = new ScaleTransform(10, 10) };
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

        static void DrawCorridor1()
        {
            PathGeometry corridorGeometry = new PathGeometry();

            PathFigure trapezoidLeft = new PathFigure() { StartPoint = new Point(0, 0) };
            trapezoidLeft.Segments.Add(new PolyLineSegment(new Point[] { new Point(0, 8), new Point(5, 6), new Point(5, 3), new Point(0, 0) }, true));

            PathFigure trapezoidRight = new PathFigure() { StartPoint = new Point(12, 0) };
            trapezoidRight.Segments.Add(new PolyLineSegment(new Point[] { new Point(12, 8), new Point(10, 6), new Point(10, 3), new Point(12, 0) }, true));

            PathFigure trapezoidTop = new PathFigure() { StartPoint = new Point(0, 0) };
            trapezoidTop.Segments.Add(new PolyLineSegment(new Point[] { new Point(5, 3), new Point(10, 3), new Point(12, 0), new Point(0, 0) }, true));

            PathFigure trapezoidBottom = new PathFigure() { StartPoint = new Point(12, 8) };
            trapezoidBottom.Segments.Add(new PolyLineSegment(new Point[] { new Point(10, 6), new Point(5, 6), new Point(0, 8), new Point(12, 8) }, true));

            corridorGeometry.Figures.Add(trapezoidLeft);
            corridorGeometry.Figures.Add(trapezoidRight);
            corridorGeometry.Figures.Add(trapezoidTop);
            corridorGeometry.Figures.Add(trapezoidBottom);

            Corridors[0] = new Path() { Data = corridorGeometry, RenderTransform = new ScaleTransform(10, 10) };
        }

        static void DrawCorridor2()
        {
            PathGeometry corridorGeometry = new PathGeometry();

            PathFigure trapezoidLeft = new PathFigure() { StartPoint = new Point(0, 0) };
            trapezoidLeft.Segments.Add(new PolyLineSegment(new Point[] { new Point(0, 8), new Point(1, 6), new Point(1, 3), new Point(0, 0) }, true));

            PathFigure trapezoidRight = new PathFigure() { StartPoint = new Point(12, 0) };
            trapezoidRight.Segments.Add(new PolyLineSegment(new Point[] { new Point(12, 8), new Point(11, 6), new Point(11, 3), new Point(12, 0) }, true));

            PathFigure trapezoidTop = new PathFigure() { StartPoint = new Point(0, 0) };
            trapezoidTop.Segments.Add(new PolyLineSegment(new Point[] { new Point(1, 3), new Point(11, 3), new Point(12, 0), new Point(0, 0) }, true));

            PathFigure trapezoidBottom = new PathFigure() { StartPoint = new Point(12, 8) };
            trapezoidBottom.Segments.Add(new PolyLineSegment(new Point[] { new Point(11, 6), new Point(1, 6), new Point(0, 8), new Point(12, 8) }, true));

            corridorGeometry.Figures.Add(trapezoidLeft);
            corridorGeometry.Figures.Add(trapezoidRight);
            corridorGeometry.Figures.Add(trapezoidTop);
            corridorGeometry.Figures.Add(trapezoidBottom);

            Corridors[1] = new Path() { Data = corridorGeometry, RenderTransform = new ScaleTransform(10, 10) };
        }

        static void DrawCorridor3()
        {
            PathGeometry corridorGeometry = new PathGeometry();

            PathFigure trapezoidLeft = new PathFigure() { StartPoint = new Point(0, 0) };
            trapezoidLeft.Segments.Add(new PolyLineSegment(new Point[] { new Point(2, 3), new Point(2, 6), new Point(0, 8), new Point(0, 0) }, true));

            PathFigure trapezoidRight = new PathFigure() { StartPoint = new Point(12, 0) };
            trapezoidRight.Segments.Add(new PolyLineSegment(new Point[] { new Point(12, 8), new Point(7, 6), new Point(7, 3), new Point(12, 0) }, true));

            PathFigure trapezoidTop = new PathFigure() { StartPoint = new Point(0, 0) };
            trapezoidTop.Segments.Add(new PolyLineSegment(new Point[] { new Point(2, 3), new Point(7, 3), new Point(12, 0), new Point(0, 0) }, true));

            PathFigure trapezoidBottom = new PathFigure() { StartPoint = new Point(12, 8) };
            trapezoidBottom.Segments.Add(new PolyLineSegment(new Point[] { new Point(7, 6), new Point(2, 6), new Point(0, 8), new Point(12, 8) }, true));

            corridorGeometry.Figures.Add(trapezoidLeft);
            corridorGeometry.Figures.Add(trapezoidRight);
            corridorGeometry.Figures.Add(trapezoidTop);
            corridorGeometry.Figures.Add(trapezoidBottom);

            Corridors[2] = new Path() { Data = corridorGeometry, RenderTransform = new ScaleTransform(10, 10) };
        }

        public async Task SetSendStandardSituationDataPackageBehavior(
            TimeSpan duration,
            params object[] additionParams)
        {
            SendPackage = () => new CameraStandardSituationDataPackage(DateTime.Now);
            await Task.Delay(duration);
            return;
        }

        public async Task SetSendEmployeeArrivalDataPackageBehavior(
            TimeSpan duration,
            params object[] additionParams)
        {
            string employeeName = (string)additionParams[0];
            SendPackage = () => new CameraEmployeeArrivalDataPackage(DateTime.Now, employeeName);
            await Task.Delay(duration);
            return;
        }

        public async Task SetSendEmployeeDepartureDataPackageBehavior(
            TimeSpan duration,
            params object[] additionParams)
        {
            string employeeName = (string)additionParams[0];
            SendPackage = () => new CameraEmployeeDepartureDataPackage(DateTime.Now, employeeName);
            await Task.Delay(duration);
            return;
        }

        public async Task SetSendOutsiderOnObjectDataPackageBehavior(
            TimeSpan duration,
            params object[] additionParams)
        {
            SendPackage = () => new CameraOutsiderOnObjectDataPackage(DateTime.Now);
            await Task.Delay(duration);
            return;
        }
    }
}
