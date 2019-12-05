using Drawing.Composite;
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
using Drawing.Data;

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
        private Composite.MainShape shape;
        private bool downed = false;
        private Point oldPoint;
        private double phi = 0;
        private double theta = 0;
        private double zc = 10;

        public MainWindow()
        {
            InitializeComponent();
            coordinateSystem = new CoordinateSystem2DInteractor(canvas.Width, canvas.Height);
            canvas.Children.Add(currentLine);
            shape = new Composite.MainShape(currentLine, coordinateSystem);
            var matrix = MakeOperationMatrix(phi, theta, zc);
            SetValuesDataGrid(matrix);
            ActivateDoZ();
            //canvas.MouseMove += dragElement_MouseMove;
            MouseMove += dragElement_MouseMove;
            MouseDown += pickElement_MouseDown;
            MouseUp += dropElement_MouseUp;
            KeyDown += pickFewElements_KeyDown;
            KeyUp += upKey_KeyUp;

        }

        private void upKey_KeyUp(object sender, KeyEventArgs e)
        {
            downed = false;
        }

        private void pickFewElements_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
                downed = true;
        }

        private void createLine_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Add(interactor.CreateRandomLine(canvas.Width, canvas.Height, Brushes.Black, 5));
        }

        private void deleteLine_Click(object sender, RoutedEventArgs e)
        {
            shape.RemoveAll();
            currentLine = new Line();
            canvas.Children.Add(currentLine);
            shape = new Composite.MainShape(currentLine, coordinateSystem);
        }

        private void pickElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Shape)
            {
                if (!downed)
                {
                    shape.ClearColor();
                    currentLine = new Line();
                    canvas.Children.Add(currentLine);
                    shape = new Composite.MainShape(currentLine, coordinateSystem);
                }

                shape.PickAddShape(e.Source as Shape);

                var equation = shape.GetEquation();
                lineEquation.Content = "(" + equation[0] + "; " + equation[1] + "; " + equation[2] + ")";

                var endCoordinates = shape.GetCoordinates();

                endsCoord.Content = "(" + endCoordinates[0] + "; " + endCoordinates[1] + ")\n" +
                    "(" + endCoordinates[2] + "; " + endCoordinates[3] + ")\n" +
                    "(" + endCoordinates[4] + "; " + endCoordinates[5] + ")";
            }
        }

        private void dragElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.GetPosition(canvas).X < canvas.Width)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    shape.MoveShape(oldPoint, e.GetPosition(canvas));
                }
                var curPos = coordinateSystem.GetPoint(e.GetPosition(canvas));
                mousePosition.Content = curPos[0] + "; " + curPos[1];
            }
            oldPoint = e.GetPosition(canvas);
        }

        private void coordinateSystem_Button_Click(object sender, RoutedEventArgs e)
        {
            var lines = canvas.Children.Cast<FrameworkElement>().Where(x => x.Name == "Axis").ToArray();
            if (!lines.Any())
            {
                var createdLines = coordinateSystem.CreateAxes(canvas.Width, canvas.Height, 50);
                foreach (var l in createdLines)
                {
                    canvas.Children.Add(l);
                }
            }
            else
            {
                for (var i = 0; i < lines.Length; ++i)
                {
                    canvas.Children.Remove(lines[i]);
                }
            }
        }

        private void dropElement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Shape)
            {
                var equation = shape.GetEquation();
                lineEquation.Content = "(" + equation[0] + "; " + equation[1] + "; " + equation[2] + ")";

                var endCoordinates = shape.GetCoordinates();
                endsCoord.Content = "(" + endCoordinates[0] + "; " + endCoordinates[1] + ")\n" +
                    "(" + endCoordinates[2] + "; " + endCoordinates[3] + ")\n" +
                    "(" + endCoordinates[4] + "; " + endCoordinates[5] + ")";
            }
        }

        private void doZ_Click(object sender, RoutedEventArgs e)
        {
            var z1 = double.Parse(addZ1TextBlock.Text);
            var z2 = double.Parse(addZ2TextBlock.Text);
            shape.AddZ(new double[] { z1, z2 });

            var endCoordinates = shape.GetCoordinates();
            endsCoord.Content = "(" + endCoordinates[0] + "; " + endCoordinates[1] + ")\n" +
                "(" + endCoordinates[2] + "; " + endCoordinates[3] + ")\n" +
                "(" + endCoordinates[4] + "; " + endCoordinates[5] + ")";
        }

        private void phiSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            phi = e.NewValue;
            var matrix = MakeOperationMatrix(phi, 0, zc);
            SetValuesDataGrid(matrix);
            //dgOperationMatrix.ItemsSource = matrix;
            shape.Compute3D(matrix);
        }

        private double[,] MakeOperationMatrix(double phi, double theta, double zc)
        {
            var phiAngle = phi * Math.PI / 180;
            var thetaAngle = theta * Math.PI / 180;
            var sinPhi = Math.Sin(phiAngle);
            var sinTheta = Math.Sin(thetaAngle);
            var cosPhi = Math.Cos(phiAngle);
            var cosTheta = Math.Cos(thetaAngle);
            return new double[4, 4] {
                { cosPhi, sinPhi * sinTheta, 0, sinPhi * cosTheta / zc },
                { 0, cosTheta, 0, -sinTheta / zc },
                { sinPhi, -cosPhi * sinTheta, 0, cosPhi * cosTheta / zc },
                { 0, 0, 0, 1 }
            };
        }

        private void thetaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            theta = e.NewValue;
            var matrix = MakeOperationMatrix(0, theta, zc);
            SetValuesDataGrid(matrix);
            //dgOperationMatrix.ItemsSource = matrix;
            shape.Compute3D(matrix);
        }

        private void zcSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            zc = e.NewValue + 10;
            var matrix = MakeOperationMatrix(0, 0, zc);
            SetValuesDataGrid(matrix);
            //dgOperationMatrix.ItemsSource = matrix;
            shape.Compute3D(matrix);
        }

        private GridData GetRow(int index, double[,] matrix)
        {
            var row = new GridData()
            {
                X = matrix[index, 0],
                Y = matrix[index, 1],
                Z = matrix[index, 3],
                OK = 1
            };
            return row;
        }

        private void SetValuesDataGrid(double[,] matrix)
        {
            var rows = new List<GridData>();
            for (var i = 0; i < 4; ++i)
            {
                var row = GetRow(i, matrix);
                //dgOperationMatrix.Items.Add(row);
                rows.Add(row);
            }
            dgOperationMatrix.ItemsSource = rows;
        }

        private void KeyUpZ1(object sender, KeyEventArgs e)
        {
            ActivateDoZ();
        }

        private void ActivateDoZ()
        {
            try
            {
                var z1 = double.Parse(addZ1TextBlock.Text);
                var z2 = double.Parse(addZ2TextBlock.Text);
                doZ.IsEnabled = true;
            }
            catch
            {
                doZ.IsEnabled = false;
            }
        }

        private void KeyUpZ2(object sender, KeyEventArgs e)
        {
            ActivateDoZ();
        }
    }
}
