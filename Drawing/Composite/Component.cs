using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace Drawing.Composite
{
    abstract class Component
    {
        protected Shape shape;

        public Component(Shape shape)
        {
            this.shape = shape;
        }

        public abstract void Display();
        public abstract void Add(Component c);
        public abstract void Remove(Component c);
        public abstract void ClearColor();
        public abstract void MoveShape(Point oldPos, Point newPos);
    }
}
