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

        public Composite(Shape shape)
            : base(shape)
        { }

        public override void Add(Component component)
        {
            children.Add(component);
        }

        public override void Remove(Component component)
        {
            children.Remove(component);
        }

        public override void Display()
        {
            Console.WriteLine(shape);

            foreach (Component component in children)
            {
                component.Display();
            }
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
            var deltaX = newPos.X - oldPos.X;
            var deltaY = newPos.Y - oldPos.Y;
            var interactor = new LineInteractor();
            interactor.MoveShape(deltaX, deltaY, shape);

            foreach (var c in children)
            {
                c.MoveShape(oldPos, newPos);
            }
        }
    }
}
