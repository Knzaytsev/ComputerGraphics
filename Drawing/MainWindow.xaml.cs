using Drawing.Interactors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Drawing
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LineInteractor interactor = new LineInteractor();
        private CoordinateSystemInteractor coordinateSystem;
        private Line currentLine = new Line();
        private Point oldPoint;

        public MainWindow()
        {
            InitializeComponent();
            coordinateSystem = new CoordinateSystem2DInteractor(canvas.Width, canvas.Height);
        }

        private void createLine_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Add(interactor.CreateRandomLine(canvas.Width, canvas.Height, Brushes.Black, 5));
        }

        private void deleteLine_Click(object sender, RoutedEventArgs e)
        {
            interactor.DeleteShape(currentLine);
            currentLine = new Line();
            canvas.Children.Add(currentLine);
        }

        private void pickElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(e.Source as Shape is null))
            {
                currentLine.Stroke = Brushes.Black;
                currentLine = interactor.PickShape(e.Source as Shape) as Line;
            }
        }

        private void dragElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if(e.Source == currentLine)
                {
                    var point = e.GetPosition(canvas);
                    var r = 10f;
                    var centerFirstEnd = new Point(currentLine.X1, currentLine.Y1);
                    var centerSecondEnd = new Point(currentLine.X2, currentLine.Y2);
                    var checkHittingFirstEnd = interactor.CheckHittingPoint(point, centerFirstEnd, r);
                    var checkHittingSecondEnd = interactor.CheckHittingPoint(point, centerSecondEnd, r);
                    if (checkHittingFirstEnd)
                    {
                        interactor.MoveFirstPoint(currentLine, point, oldPoint);
                    }
                    else if (checkHittingSecondEnd)
                    {
                        interactor.MoveSecondPoint(currentLine, point, oldPoint);
                    }
                    else
                    {
                        var deltaX = point.X - oldPoint.X;
                        var deltaY = point.Y - oldPoint.Y;
                        currentLine = interactor.MoveShape(deltaX, deltaY, currentLine) as Line;
                    }
                }
            }
            oldPoint = e.GetPosition(canvas);
            var curPos = coordinateSystem.GetPoint(e.GetPosition(canvas));
            mousePosition.Content = curPos[0] + "; " + curPos[1];
        }

        private void coordinateSystem_Button_Click(object sender, RoutedEventArgs e)
        {
            var lines = canvas.Children.Cast<FrameworkElement>().Where(x => x.Name == "Axis").ToArray();
            if (!lines.Any())
            {
                var createdLines = coordinateSystem.CreateAxes(canvas.Width, canvas.Height, 200);
                foreach (var l in createdLines)
                {
                    canvas.Children.Add(l);
                }
            }
            else
            {
                for(var i = 0; i < lines.Length; ++i)
                {
                    canvas.Children.Remove(lines[i]);
                }
            }
        }
    }
}
