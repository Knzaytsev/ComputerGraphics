using Drawing.Interactors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Drawing.Composite
{
    class MainShape : Component
    {
        List<Component> children = new List<Component>();

        public MainShape(Shape shape, CoordinateSystemInteractor coordinate)
            : base(shape, coordinate)
        { }

        public override void Add(Component component)
        {
            children.Add(component);
        }

        public override void RemoveAll()
        {
            foreach (var c in children)
            {
                c.RemoveAll();
            }
        }

        public override Shape Display()
        {
            return shape;
        }

        public override void ClearColor()
        {
            foreach(var s in children)
            {
                s.ClearColor();
            }
        }

        public override void MoveShape(Point oldPos, Point newPos)
        {
            foreach (var c in children)
            {
                c.MoveShape(oldPos, newPos);
            }
        }

        public override void PickAddShape(Shape shape)
        {
            if (Contains(shape))
                return;

            foreach (var c in children)
            {
                if (c.Contains(shape))
                    return;
            }
            shape = interactor.PickShape(shape);
            Add(new UnderLine(shape as Line, coordinate));
        }

        public override bool Contains(Shape shape)
        {
            return shape == this.shape;
        }

        public override double[] GetEquation()
        {
            return children.Last().GetEquation();
        }

        public override double[] GetCoordinates()
        {
            return children.Last().GetCoordinates();
        }

        public void AddZ(double[] z)
        {
            (children.Last() as UnderLine).SetZ(z);
        }

        public void ProjectReal3D(/*double[,] operation, */double zc)
        {
            var childrenCount = children.Count;
            var data = MakeDataFromLines(childrenCount);

            var cm = new ComputingMatrix();
            cm.AddData(data);
            cm.AddOperation(new double[,]
            {
                    {1, 0, 0, 0 },
                    {0, 1, 0, 0 },
                    {0, 0, 0, -1 / zc },
                    {0, 0, 0, 1 }
            });
            cm.ComputeMatrix();

            var result = cm.GetResult();

            for(var i = 0; i < childrenCount; ++i)
            {
                var line = children[i].Display() as Line;
                var newPoint1 = coordinate.ToNormalCoordinates(new Point(result[i * 2, 0], result[i * 2, 1]));
                var newPoint2 = coordinate.ToNormalCoordinates(new Point(result[i * 2 + 1, 0], result[i * 2 + 1, 1]));

                line.X1 += newPoint1[0] - line.X1;
                line.Y1 += newPoint1[1] - line.Y1;
                line.X2 += newPoint2[0] - line.X2;
                line.Y2 += newPoint2[1] - line.Y2;
            }
            /*cm.ComputeMatrix();
            //cm.AddOperation(cm.GetNonNormalizedResult());
            cm.AddOperation(cm.GetResult());
            cm.AddData(data);
            cm.ComputeMatrix();


            var result = cm.GetResult();

            for (var i = 0; i < childrenCount; ++i)
            {
                var line = children[i].Display() as Line;
                var newPoint1 = coordinate.ToNormalCoordinates(new Point(result[i * 2, 0], result[i * 2, 1]));
                var newPoint2 = coordinate.ToNormalCoordinates(new Point(result[i * 2 + 1, 0], result[i * 2 + 1, 1]));

                line.X1 += newPoint1[0] - line.X1;
                line.Y1 += newPoint1[1] - line.Y1;
                line.X2 += newPoint2[0] - line.X2;
                line.Y2 += newPoint2[1] - line.Y2;
            }*/
        }

        public void ComputeReal3D(double[,] operation)
        {
            var childrenCount = children.Count;
            var data = MakeDataFromLines(childrenCount);
            
            ComputingMatrix cm = new ComputingMatrix();
            cm.AddData(data);
            cm.AddOperation(operation);
            cm.ComputeMatrix();

            var result = cm.GetResult();

            for(var i = 0; i < childrenCount; ++i)
            {
                var underLine = children[i] as UnderLine;
                var point1 = coordinate.ToNormalCoordinates(new Point(result[i * 2, 0], result[i * 2, 1]));
                var point2 = coordinate.ToNormalCoordinates(new Point(result[i * 2 + 1, 0], result[i * 2 + 1, 1]));
                var realMatrix = new double[,]
                {
                    { point1[0], point1[1], result[i * 2, 2], result[i * 2, 3] },
                    { point2[0], point2[1], result[i * 2 + 1, 2], result[i * 2 + 1, 3] }
                };
                underLine.SetRealMatrix(realMatrix);
            }
        }

        private double[,] MakeDataFromLines(int n)
        {
            var data = new double[n * 2, 4];
            for (var i = 0; i < n; ++i)
            {
                var line = children[i];
                var dataLine = (line as UnderLine).GetRealMatrix();
                var point1 = coordinate.GetPoint(new Point(dataLine[0, 0], dataLine[0, 1]));
                data[i * 2, 0] = point1[0];
                data[i * 2, 1] = point1[1];
                data[i * 2, 2] = dataLine[0, 2];
                data[i * 2, 3] = 1;

                var point2 = coordinate.GetPoint(new Point(dataLine[1, 0], dataLine[1, 1]));
                data[i * 2 + 1, 0] = point2[0];
                data[i * 2 + 1, 1] = point2[1];
                data[i * 2 + 1, 2] = dataLine[1, 2];
                data[i * 2 + 1, 3] = 1;
            }
            return data;
        }
    }
}
