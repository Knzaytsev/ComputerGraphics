using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Drawing.Interactors
{
    interface ShapeInteractor
    {
        void DeleteShape(Shape shape);

        Shape MoveShape(double x, double y, Shape shape);

        Shape PickShape(Shape shape);


    }
}
