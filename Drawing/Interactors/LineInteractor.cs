using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Drawing.Interactors
{
    class LineInteractor : ShapeInteractor
    {
        private Random rnd = new Random();

        public Line CreateRandomLine(double width, double height, SolidColorBrush brush, int thickness)
        {
            var firstPoint = GetPoint(width, height);
            var secondPoint = GetPoint(width, height);
            return new Line()
            {
                X1 = firstPoint.X,
                Y1 = firstPoint.Y,
                X2 = secondPoint.X,
                Y2 = secondPoint.Y,
                Stroke = brush,
                StrokeThickness = thickness
            };
        }

        public override void DeleteShape(Shape line)
        {
            ((Canvas)line.Parent).Children.Remove(line);
        }

        public override Shape MoveShape(double x, double y, Shape line)
        {
            var changedLine = line as Line;
            double[,] matrixData = {
                { changedLine.X1, changedLine.Y1, 1 }, 
                { changedLine.X2, changedLine.Y2, 1 } 
            };
            double[,] matrixOp = { 
                { 1, 0, 0 }, 
                { 0, 1, 0 }, 
                { x, y, 1 } 
            };

            var D = Matrix<double>.Build.DenseOfArray(matrixData);
            var O = Matrix<double>.Build.DenseOfArray(matrixOp);

            var DS = D.Multiply(O);
            changedLine.X1 = DS[0, 0];
            changedLine.Y1 = DS[0, 1];
            changedLine.X2 = DS[1, 0];
            changedLine.Y2 = DS[1, 1];
            return changedLine;
        }

        public void MoveFirstPoint(Line line, Point point, Point oldPoint)
        {
            line.X1 += point.X - oldPoint.X;
            line.Y1 += point.Y - oldPoint.Y;
        }

        public void MoveSecondPoint(Line line, Point point, Point oldPoint)
        {
            line.X2 += point.X - oldPoint.X;
            line.Y2 += point.Y - oldPoint.Y;
        }

        public override Shape PickShape(Shape line)
        {
            line.Stroke = Brushes.Red;
            return line;
        }

        public bool CheckHittingPoint(Point point, Point center, double r)
        {
            double x = point.X - center.X;
            double y = point.Y - center.Y;
            return r * r >= x * x + y * y;
        }

        private Point GetPoint(double width, double height)
        {
            var x = rnd.NextDouble() * width;
            var y = rnd.NextDouble() * height;
            return new Point(x, y);
        }

        public override double[] GetEquation(Shape shape, CoordinateSystemInteractor coordinate)
        {
            var line = shape as Line;
            Point startPoint = new Point(line.X1, line.Y1);
            Point endPoint = new Point(line.X2, line.Y2);
            var firstPoint = coordinate.GetPoint(startPoint);
            var lastPoint = coordinate.GetPoint(endPoint);
            double A = firstPoint[1] - lastPoint[1];
            double B = lastPoint[0] - firstPoint[0];
            double C = firstPoint[0] * lastPoint[1] - lastPoint[0] * firstPoint[1];
            return new double[] { A, B, C };
        }
    }
}
