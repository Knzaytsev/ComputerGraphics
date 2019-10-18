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
        private Line currentLine = new Line();
        private Point oldPoint;
        delegate void PickItem(object sender, MouseEventArgs e);
        event PickItem Pick;

        public MainWindow()
        {
            InitializeComponent();
            canvas.Children.Add(currentLine);
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
            currentLine.Stroke = Brushes.Black;
            currentLine = interactor.PickShape(e.Source as Line) as Line;
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
        }

        private void MainWindow_Pick(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void dragElement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Pick = null;
        }
    }
}
