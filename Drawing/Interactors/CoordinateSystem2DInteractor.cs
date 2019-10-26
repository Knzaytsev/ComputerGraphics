using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Drawing.Interactors
{
    public class CoordinateSystem2DInteractor : CoordinateSystemInteractor
    {
        public CoordinateSystem2DInteractor(double width, double heigth)
        {
            vectorCenter = new double[2] { width / 2, heigth / 2 };
        }

        public override List<UIElement> CreateAxes(double width, double heigth, double step = 10)
        {
            List<UIElement> lines = new List<UIElement>();
            
            var centerWidth = vectorCenter[0];
            var centerHeigth = vectorCenter[1];
            var startPointX = centerWidth % step;
            var x = -centerWidth + startPointX;
            var startPointY = centerHeigth % step;
            var y = centerHeigth - startPointY;

            for (var i = startPointX; i < width; i += step)
            {
                Line mark = new Line()
                {
                    X1 = i,
                    X2 = i,
                    Y1 = centerHeigth - 5,
                    Y2 = centerHeigth + 5,
                    Stroke = Brushes.Gray,
                    Focusable = false,
                    IsEnabled = false,
                    Name = "Axis"
                };

                Label label = new Label()
                {
                    Name = "Axis",
                    Content = x,
                    Foreground = Brushes.Gray,
                    IsEnabled = false,
                    Focusable = false,
                    Margin = new Thickness(i + 6, centerHeigth + 6, 0, 0)
                };
                lines.Add(label);

                x += step;
                lines.Add(mark);
            }

            for (var i = startPointY; i < heigth; i += step)
            {
                Line mark = new Line()
                {
                    Y1 = i,
                    Y2 = i,
                    X1 = centerWidth - 5,
                    X2 = centerWidth + 5,
                    Stroke = Brushes.Gray,
                    Focusable = false,
                    IsEnabled = false,
                    Name = "Axis"
                };

                Label label = new Label()
                {
                    Name = "Axis",
                    Content = y,
                    Foreground = Brushes.Gray,
                    IsEnabled = false,
                    Focusable = false,
                    Margin = new Thickness(centerWidth + 6, i + 6, 0, 0)
                };
                lines.Add(label);

                y -= step;
                lines.Add(mark);
            }

            Line lineX = new Line()
            {
                X1 = width / 2,
                X2 = width / 2,
                Y1 = 0,
                Y2 = heigth,
                Stroke = Brushes.Gray,
                StrokeThickness = 2,
                Name = "Axis",
                Focusable = false,
                IsEnabled = false
            };
            lines.Add(lineX);

            Line lineY = new Line()
            {
                X1 = 0,
                X2 = width,
                Y1 = heigth / 2,
                Y2 = heigth / 2,
                Stroke = Brushes.Gray,
                StrokeThickness = 2,
                Name = "Axis",
                Focusable = false,
                IsEnabled = false
            };
            lines.Add(lineY);

            return lines;
        }

        public override double[] GetPoint(Point mousePoint)
        {
            double x = mousePoint.X - vectorCenter[0];
            double y = vectorCenter[1] - mousePoint.Y;
            return new double[2] { x, y };
        }

        public override void SetOffsetVector(double[] value)
        {
            vectorOffset = value ?? vectorOffset;
            var offsetX = vectorOffset[0];
            var offsetY = vectorOffset[1];
            var centerX = vectorCenter[0];
            var centerY = vectorCenter[1];
            vectorCenter = new double[2] { centerX - offsetX, centerY - offsetY };
        }

        public override void SetScale(int scale)
        {
            this.Scale = scale;
        }
    }
}
