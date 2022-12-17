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
    public class CameraWatchingFromLeftCornerWall : Camera
    {
        public override string TypeKey { get { return "LeftCornerWallCamera"; } }

        public override void DrawCorridor()
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
            Thickness movingFrom = new Thickness(4, 3.3, 0, 0),
                      movingTo = new Thickness(6, 2, 0, 0);
            ThicknessAnimation employeeForwardMovingAnimation =
                new ThicknessAnimation(movingFrom, movingTo, duration) { FillBehavior = FillBehavior.HoldEnd };

            double scalingFrom = 0.33,
                   scalingTo = 0.67;
            DoubleAnimation employeeForwardScalingXAnimation =
                new DoubleAnimation(scalingFrom, scalingTo, duration) { FillBehavior = FillBehavior.HoldEnd },
                            employeeForwardScalingYAnimation =
                new DoubleAnimation(scalingFrom, scalingTo, duration) { FillBehavior = FillBehavior.HoldEnd };

            double strokeThicknessFrom = 0.1,
                   strokeThicknessTo = 0.3;
            DoubleAnimation employeeForwardStrokeThicknessAnimation =
                new DoubleAnimation(strokeThicknessFrom, strokeThicknessTo, duration) { FillBehavior = FillBehavior.HoldEnd };

            Storyboard employeeStoryboard = new Storyboard();
            employeeStoryboard.Children.Add(employeeForwardMovingAnimation);
            employeeStoryboard.Children.Add(employeeForwardScalingXAnimation);
            employeeStoryboard.Children.Add(employeeForwardScalingYAnimation);
            employeeStoryboard.Children.Add(employeeForwardStrokeThicknessAnimation);

            SetMarginActorProperty(employeeForwardMovingAnimation);
            SetScaleXActorProperty(employeeForwardScalingXAnimation);
            SetScaleYActorProperty(employeeForwardScalingYAnimation);
            SetStrokeThicknessActorProperty(employeeForwardStrokeThicknessAnimation);

            SetDesiredFrameRateForStoryboard(employeeStoryboard);

            return employeeStoryboard;
        }

        public override Storyboard BuildEmployeeDepartureAnimation(TimeSpan duration)
        {
            Thickness movingFrom = new Thickness(6, 2, 0, 0),
                      movingTo = new Thickness(4, 3.3, 0, 0);
            ThicknessAnimation employeeBackwardMovingAnimation =
                new ThicknessAnimation(movingFrom, movingTo, duration) { FillBehavior = FillBehavior.HoldEnd };

            double scalingFrom = 0.67,
                   scalingTo = 0.33;
            DoubleAnimation employeeBackwardScalingXAnimation =
                new DoubleAnimation(scalingFrom, scalingTo, duration) { FillBehavior = FillBehavior.HoldEnd },
                            employeeBackwardScalingYAnimation =
                new DoubleAnimation(scalingFrom, scalingTo, duration) { FillBehavior = FillBehavior.HoldEnd };

            double strokeThicknessFrom = 0.3,
                   strokeThicknessTo = 0.1;
            DoubleAnimation employeeBackwardStrokeThicknessAnimation =
                new DoubleAnimation(strokeThicknessFrom, strokeThicknessTo, duration) { FillBehavior = FillBehavior.HoldEnd };

            Storyboard employeeStoryboard = new Storyboard();
            employeeStoryboard.Children.Add(employeeBackwardMovingAnimation);
            employeeStoryboard.Children.Add(employeeBackwardScalingXAnimation);
            employeeStoryboard.Children.Add(employeeBackwardScalingYAnimation);
            employeeStoryboard.Children.Add(employeeBackwardStrokeThicknessAnimation);

            SetMarginActorProperty(employeeBackwardMovingAnimation);
            SetScaleXActorProperty(employeeBackwardScalingXAnimation);
            SetScaleYActorProperty(employeeBackwardScalingYAnimation);
            SetStrokeThicknessActorProperty(employeeBackwardStrokeThicknessAnimation);

            SetDesiredFrameRateForStoryboard(employeeStoryboard);

            return employeeStoryboard;
        }

        public override Storyboard BuildOutsiderOnObjectAnimation(TimeSpan duration)
        {
            TimeSpan backwardAnimationDelay = TimeSpan.FromSeconds(2);

            Thickness movingFrom = new Thickness(4, 3.3, 0, 0),
                      movingTo = new Thickness(6, 2, 0, 0);
            ThicknessAnimation outsiderForwardMovingAnimation =
                new ThicknessAnimation(movingFrom, movingTo, duration) { FillBehavior = FillBehavior.HoldEnd },
                               outsiderBackwardMovingAnimation =
                new ThicknessAnimation(movingTo, movingFrom, duration)
                {
                    FillBehavior = FillBehavior.HoldEnd,
                    BeginTime = duration + backwardAnimationDelay
                };

            double scalingFrom = 0.33,
                   scalingTo = 0.67;
            DoubleAnimation outsiderForwardScalingXAnimation =
                new DoubleAnimation(scalingFrom, scalingTo, duration) { FillBehavior = FillBehavior.HoldEnd },
                            outsiderForwardScalingYAnimation =
                new DoubleAnimation(scalingFrom, scalingTo, duration) { FillBehavior = FillBehavior.HoldEnd },
                            outsiderBackwardScalingXAnimation =
                new DoubleAnimation(scalingTo, scalingFrom, duration)
                {
                    FillBehavior = FillBehavior.HoldEnd,
                    BeginTime = duration + backwardAnimationDelay
                },
                            outsiderBackwardScalingYAnimation =
                new DoubleAnimation(scalingTo, scalingFrom, duration)
                {
                    FillBehavior = FillBehavior.HoldEnd,
                    BeginTime = duration + backwardAnimationDelay
                };

            double strokeThicknessFrom = 0.1,
                   strokeThicknessTo = 0.3;
            DoubleAnimation outsiderForwardStrokeThicknessAnimation =
                new DoubleAnimation(strokeThicknessFrom, strokeThicknessTo, duration) { FillBehavior = FillBehavior.HoldEnd },
                            outsiderBackwardStrokeThicknessAnimation =
                new DoubleAnimation(strokeThicknessTo, strokeThicknessFrom, duration)
                {
                    FillBehavior = FillBehavior.HoldEnd,
                    BeginTime = duration + backwardAnimationDelay
                };

            Storyboard outsiderStoryboard = new Storyboard()
            {
                Duration = duration + duration + backwardAnimationDelay
            };
            outsiderStoryboard.Children.Add(outsiderForwardMovingAnimation);
            outsiderStoryboard.Children.Add(outsiderForwardScalingXAnimation);
            outsiderStoryboard.Children.Add(outsiderForwardScalingYAnimation);
            outsiderStoryboard.Children.Add(outsiderForwardStrokeThicknessAnimation);
            outsiderStoryboard.Children.Add(outsiderBackwardMovingAnimation);
            outsiderStoryboard.Children.Add(outsiderBackwardScalingXAnimation);
            outsiderStoryboard.Children.Add(outsiderBackwardScalingYAnimation);
            outsiderStoryboard.Children.Add(outsiderBackwardStrokeThicknessAnimation);

            SetMarginActorProperty(outsiderForwardMovingAnimation);
            SetScaleXActorProperty(outsiderForwardScalingXAnimation);
            SetScaleYActorProperty(outsiderForwardScalingYAnimation);
            SetStrokeThicknessActorProperty(outsiderForwardStrokeThicknessAnimation);
            SetMarginActorProperty(outsiderBackwardMovingAnimation);
            SetScaleXActorProperty(outsiderBackwardScalingXAnimation);
            SetScaleYActorProperty(outsiderBackwardScalingYAnimation);
            SetStrokeThicknessActorProperty(outsiderBackwardStrokeThicknessAnimation);

            SetDesiredFrameRateForStoryboard(outsiderStoryboard);

            return outsiderStoryboard;
        }
    }
}
