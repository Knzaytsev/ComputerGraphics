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
    class Composite : Component
    {
        List<Component> children = new List<Component>();

        public Composite(Shape shape, CoordinateSystemInteractor coordinate)
            : base(shape, coordinate)
        { }

        public override void Add(Component component)
        {
            children.Add(component);
        }

        public override void RemoveAll()
        {
            interactor.DeleteShape(shape);

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
            shape.Stroke = Brushes.Black;

            foreach(var s in children)
            {
                s.ClearColor();
            }
        }

        public override void MoveShape(Point oldPos, Point newPos)
        {
            var point = newPos;
            var r = 20f;
            var centerFirstEnd = new Point((shape as Line).X1, (shape as Line).Y1);
            var centerSecondEnd = new Point((shape as Line).X2, (shape as Line).Y2);
            var checkHittingFirstEnd = interactor.CheckHittingPoint(point, centerFirstEnd, r);
            var checkHittingSecondEnd = interactor.CheckHittingPoint(point, centerSecondEnd, r);
            if (checkHittingFirstEnd)
            {
                interactor.MoveFirstPoint(shape as Line, point, oldPos);
            }
            else if (checkHittingSecondEnd)
            {
                interactor.MoveSecondPoint(shape as Line, point, oldPos);
            }
            else
            {
                var deltaX = point.X - oldPos.X;
                var deltaY = point.Y - oldPos.Y;
                shape = interactor.MoveShape(deltaX, deltaY, shape) as Line;
            }

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
            Add(new Composite(shape, coordinate));
        }

        public override bool Contains(Shape shape)
        {
            return shape == this.shape;
        }

        public override double[] GetEquation()
        {
            return interactor.GetEquation(children.Last().Display(), coordinate);
        }
    }
}
