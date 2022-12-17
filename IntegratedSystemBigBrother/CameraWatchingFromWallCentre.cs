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

namespace IntegratedSystemBigBrother
{
    public class CameraWatchingFromWallCentre : Camera
    {
        public override string TypeKey { get { return "WallCentreCamera"; } }

        public override void DrawCorridor()
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

            Path corridor = new Path()
            {
                Data = corridorGeometry,
                Stroke = Brushes.Black,
                StrokeThickness = 0.05
            };
            Grid.SetZIndex(corridor, 0);

            Corridor = corridor;
        }

        public override Storyboard BuildEmployeeArrivalAnimation(TimeSpan duration)
        {
            Thickness movingFrom = new Thickness(1, 2.5, 0, 0),
                      movingTo = new Thickness(9, 2.5, 0, 0);
            ThicknessAnimation employeeForwardMovingAnimation =
                new ThicknessAnimation(movingFrom, movingTo, duration) { FillBehavior = FillBehavior.HoldEnd };

            double strokeThicknessFrom = 0.3,
                   strokeThicknessTo = 0.3;
            DoubleAnimation employeeForwardStrokeThicknessAnimation =
                new DoubleAnimation(strokeThicknessFrom, strokeThicknessTo, duration) { FillBehavior = FillBehavior.HoldEnd };

            Storyboard employeeStoryboard = new Storyboard()
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            employeeStoryboard.Children.Add(employeeForwardMovingAnimation);
            employeeStoryboard.Children.Add(employeeForwardStrokeThicknessAnimation);

            SetMarginActorProperty(employeeForwardMovingAnimation);
            SetStrokeThicknessActorProperty(employeeForwardStrokeThicknessAnimation);

            SetDesiredFrameRateForStoryboard(employeeStoryboard);

            return employeeStoryboard;
        }

        public override Storyboard BuildEmployeeDepartureAnimation(TimeSpan duration)
        {
            Thickness movingFrom = new Thickness(9, 2.5, 0, 0),
                      movingTo = new Thickness(1, 2.5, 0, 0);
            ThicknessAnimation employeeBackwardMovingAnimation =
                new ThicknessAnimation(movingFrom, movingTo, duration) { FillBehavior = FillBehavior.HoldEnd };

            double strokeThicknessFrom = 0.3,
                   strokeThicknessTo = 0.3;
            DoubleAnimation employeeBackwardStrokeThicknessAnimation =
                new DoubleAnimation(strokeThicknessFrom, strokeThicknessTo, duration) { FillBehavior = FillBehavior.HoldEnd };

            Storyboard employeeStoryboard = new Storyboard()
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            employeeStoryboard.Children.Add(employeeBackwardMovingAnimation);
            employeeStoryboard.Children.Add(employeeBackwardStrokeThicknessAnimation);

            SetMarginActorProperty(employeeBackwardMovingAnimation);
            SetStrokeThicknessActorProperty(employeeBackwardStrokeThicknessAnimation);

            SetDesiredFrameRateForStoryboard(employeeStoryboard);

            return employeeStoryboard;
        }

        public override Storyboard BuildOutsiderOnObjectAnimation(TimeSpan duration)
        {
            TimeSpan backwardAnimationDelay = TimeSpan.FromSeconds(2);

            Thickness movingFrom = new Thickness(1, 2.5, 0, 0),
                      movingTo = new Thickness(9, 2.5, 0, 0);
            ThicknessAnimation outsiderForwardMovingAnimation =
                new ThicknessAnimation(movingFrom, movingTo, duration) { FillBehavior = FillBehavior.HoldEnd },
                               outsiderBackwardMovingAnimation =
                new ThicknessAnimation(movingTo, movingFrom, duration)
                {
                    FillBehavior = FillBehavior.HoldEnd,
                    BeginTime = duration + backwardAnimationDelay
                };

            double strokeThicknessFrom = 0.3,
                   strokeThicknessTo = 0.3;
            DoubleAnimation outsiderTwowayStrokeThicknessAnimation =
                new DoubleAnimation(strokeThicknessFrom, strokeThicknessTo, duration + duration + backwardAnimationDelay) { FillBehavior = FillBehavior.HoldEnd };

            Storyboard outsiderStoryboard = new Storyboard()
            {
                Duration = duration + duration + backwardAnimationDelay,
                RepeatBehavior = RepeatBehavior.Forever
            };
            outsiderStoryboard.Children.Add(outsiderForwardMovingAnimation);
            outsiderStoryboard.Children.Add(outsiderBackwardMovingAnimation);
            outsiderStoryboard.Children.Add(outsiderTwowayStrokeThicknessAnimation);

            SetMarginActorProperty(outsiderForwardMovingAnimation);
            SetMarginActorProperty(outsiderBackwardMovingAnimation);
            SetStrokeThicknessActorProperty(outsiderTwowayStrokeThicknessAnimation);

            SetDesiredFrameRateForStoryboard(outsiderStoryboard);

            return outsiderStoryboard;
        }
    }
}
