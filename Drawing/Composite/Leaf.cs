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
    class Leaf : Component
    {
        public Leaf(Shape shape)
        : base(shape)
        { }

        public override void Display()
        {
            Console.WriteLine(shape);
        }

        public override void Add(Component component)
        {
            throw new NotImplementedException();
        }

        public override void Remove(Component component)
        {
            throw new NotImplementedException();
        }

        public override void ClearColor()
        {
            shape.Stroke = Brushes.Black;
        }

        public override void MoveShape(Point oldPos, Point newPos)
        {
            var deltaX = newPos.X - oldPos.X;
            var deltaY = newPos.Y - oldPos.Y;
            var interactor = new LineInteractor();
            interactor.MoveShape(deltaX, deltaY, shape);
        }
    }
}
