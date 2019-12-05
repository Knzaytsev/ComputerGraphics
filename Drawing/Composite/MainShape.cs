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
            //return interactor.GetEquation(children.Last().Display(), coordinate);
        }

        public override double[] GetCoordinates()
        {
            return children.Last().GetCoordinates();
        }

        public void AddZ(double[] z)
        {
            (children.Last() as UnderLine).SetZ(z);
            /*foreach(var c in children)
            {
                (c as UnderLine).SetZ(z);
            }*/
        }

        public void ProjectReal3D(double[,] operation)
        {
            var childrenCount = children.Count;
            var data = new double[childrenCount * 2, 4];
            for (var i = 0; i < childrenCount; ++i)
            {
                var line = children[i];
                var dataLine = (line as UnderLine).GetRealMatrix();
                data[i * 2, 0] = dataLine[0, 0];
                data[i * 2, 1] = dataLine[0, 1];
                data[i * 2, 2] = dataLine[0, 2];
                data[i * 2, 3] = 1;

                data[i * 2 + 1, 0] = dataLine[1, 0];
                data[i * 2 + 1, 1] = dataLine[1, 1];
                data[i * 2 + 1, 2] = dataLine[1, 2];
                data[i * 2 + 1, 3] = 1;
            }

            ComputingMatrix cm = new ComputingMatrix();
            cm.AddData(data);
            cm.AddOperation(operation);
            cm.ComputeMatrix();

            var result = cm.GetResult();

            for (var i = 0; i < childrenCount; ++i)
            {
                var visibleMatrix = new double[,]
                {
                    { result[i * 2, 0], result[i * 2, 1] },
                    { result[i * 2 + 1, 0], result[i * 2 + 1, 1] }
                };
                (children[i] as UnderLine).SetVisibleMatrix(visibleMatrix);
            }
        }

        public void ComputeReal3D(double[,] operation)
        {
            var childrenCount = children.Count;
            var data = new double[childrenCount * 2, 4];
            for (var i = 0; i < childrenCount; ++i)
            {
                var line = children[i];
                var dataLine = (line as UnderLine).GetRealMatrix();
                data[i * 2, 0] = dataLine[0, 0];
                data[i * 2, 1] = dataLine[0, 1];
                data[i * 2, 2] = dataLine[0, 2];
                data[i * 2, 3] = 1;

                data[i * 2 + 1, 0] = dataLine[1, 0];
                data[i * 2 + 1, 1] = dataLine[1, 1];
                data[i * 2 + 1, 2] = dataLine[1, 2];
                data[i * 2 + 1, 3] = 1;
            }

            ComputingMatrix cm = new ComputingMatrix();
            cm.AddData(data);
            cm.AddOperation(operation);
            cm.ComputeMatrix();

            var result = cm.GetResult();

            for(var i = 0; i < childrenCount; ++i)
            {
                var realMatrix = new double[,]
                {
                    { result[i * 2, 0], result[i * 2, 1], result[i * 2, 2], result[i * 2, 3] },
                    { result[i * 2 + 1, 0], result[i * 2 + 1, 1], result[i * 2 + 1, 2], result[i * 2 + 1, 3] }
                };
                (children[i] as UnderLine).SetRealMatrix(realMatrix);
            }
        } 

        public void Compute3D(double[,] operation)
        {
            var childrenCount = children.Count;
            var data = new double[childrenCount * 2, 4];
            for(var i = 0; i < childrenCount; ++i)
            {
                var line = children[i];
                var dataLine = (line as UnderLine).GetRealMatrix();
                data[i * 2, 0] = dataLine[0, 0];
                data[i * 2, 1] = dataLine[0, 1];
                data[i * 2, 2] = dataLine[0, 2];
                data[i * 2, 3] = 1;

                data[i * 2 + 1, 0] = dataLine[1, 0];
                data[i * 2 + 1, 1] = dataLine[1, 1];
                data[i * 2 + 1, 2] = dataLine[1, 2];
                data[i * 2 + 1, 3] = 1;
                /*var line = children[i].Display() as Line;
                var z = (children[i] as UnderLine).GetZ();
                var point = coordinate.GetPoint(new Point(line.X1, line.Y1));
                //data[i, 0] = line.X1;
                //data[i, 1] = line.Y1;
                data[i*2, 0] = point[0];
                data[i*2, 1] = point[1];
                data[i*2, 2] = z[0];
                data[i*2, 3] = 1;
                point = coordinate.GetPoint(new Point(line.X2, line.Y2));
                //data[i + 1, 0] = line.X2;
                //data[i + 1, 1] = line.Y2;
                data[i*2 + 1, 0] = point[0];
                data[i*2 + 1, 1] = point[1];
                data[i*2 + 1, 2] = z[1];
                data[i*2 + 1, 3] = 1;*/
            }
            ComputingMatrix cm = new ComputingMatrix();
            cm.AddData(data);
            cm.AddOperation(operation);
            cm.ComputeMatrix();
            var result = cm.GetResult();

            for(var i = 0; i < childrenCount; ++i)
            {
                var line = children[i].Display() as Line;
                /*line.X1 = result[i, 0];
                line.Y1 = result[i, 1];
                line.X2 = result[i + 1, 0];
                line.Y2 = result[i + 1, 1];*/
                var point = coordinate.ToNormalCoordinates(new Point(result[2*i, 0], result[2*i, 1]));
                line.X1 = point[0];
                line.Y1 = point[1];
                point = coordinate.ToNormalCoordinates(new Point(result[2*i + 1, 0], result[2*i + 1, 1]));
                line.X2 = point[0];
                line.Y2 = point[1];

                /*var oldPoint = new Point(line.X1, line.Y1);
                var newPoint = new Point(point[0], point[1]);

                children[i].MoveShape(oldPoint, newPoint);*/
            }
        }

        public void Compute3DVisible(double[,] operation)
        {

        }
    }
}
