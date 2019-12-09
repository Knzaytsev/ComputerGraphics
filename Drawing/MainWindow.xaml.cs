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
        private double gamma = 0;
        private double zc = 10;
        private int id = 0;
        private List<LineData> dataLine = new List<LineData>();
        private bool median = false;
        private bool height = false;

        public MainWindow()
        {
            InitializeComponent();
            coordinateSystem = new CoordinateSystem2DInteractor(canvas.Width, canvas.Height);
            canvas.Children.Add(currentLine);
            shape = new Composite.MainShape(currentLine, coordinateSystem);
            //var matrix = MakeOperationMatrix(phi, theta, zc);
            //SetValuesDataGrid(matrix);
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
            var line = interactor.CreateRandomLine(canvas.Width, canvas.Height, Brushes.Black, 5, id);
            canvas.Children.Add(line);
            dataLine.Add(new LineData()
            {
                Id = id,
                X1 = line.X1,
                Y1 = line.Y1,
                X2 = line.X2,
                Y2 = line.Y2,
                Z1 = 0,
                Z2 = 0,
                StrokeThickness = line.StrokeThickness
            });
            ++id;
        }

        private void deleteLine_Click(object sender, RoutedEventArgs e)
        {
            shape.RemoveAll(dataLine);
            currentLine = new Line();
            canvas.Children.Add(currentLine);
            shape = new Composite.MainShape(currentLine, coordinateSystem);
        }

        private void pickElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (median)
            {
                var line = shape.GetLastShape() as Line;
                var medianLine = interactor.CreateMedian(e.GetPosition(canvas), line, id);
                canvas.Children.Add(medianLine);
                dataLine.Add(new LineData
                {
                    Id = id,
                    X1 = medianLine.X1,
                    Y1 = medianLine.Y1,
                    Z1 = 0,
                    X2 = medianLine.X2,
                    Y2 = medianLine.Y2,
                    Z2 = 0,
                    StrokeThickness = medianLine.StrokeThickness
                });
                ++id;
                median = false;
            }
            else if (height)
            {
                var line = shape.GetLastShape() as Line;
                var heightLine = interactor.CreateHeight(e.GetPosition(canvas), line, id);
                canvas.Children.Add(heightLine);
                dataLine.Add(new LineData
                {
                    Id = id,
                    X1 = heightLine.X1,
                    Y1 = heightLine.Y1,
                    Z1 = 0,
                    X2 = heightLine.X2,
                    Y2 = heightLine.Y2,
                    Z2 = 0,
                    StrokeThickness = heightLine.StrokeThickness
                });
                ++id;
                height = false;
            }

            if (e.Source is Shape)
            {
                if (!downed)
                {
                    shape.ClearColor();
                    currentLine = new Line();
                    canvas.Children.Add(currentLine);
                    shape = new Composite.MainShape(currentLine, coordinateSystem);
                }

                shape.PickAddShape(e.Source as Shape, dataLine);

                ShowCoordinates();
            }
        }

        private void ShowEquation()
        {
            var equation = shape.GetEquation().Select(x => Math.Round(x)).ToArray();
            lineEquation.Content = "(" + equation[0] + "; " + equation[1] + "; " + equation[2] + ")";
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
                ShowEquation();

                ShowCoordinates();
            }
        }

        private void doZ_Click(object sender, RoutedEventArgs e)
        {
            var z1 = double.Parse(addZ1TextBlock.Text);
            var z2 = double.Parse(addZ2TextBlock.Text);
            shape.AddZ(new double[] { z1, z2 }, dataLine);

            ShowCoordinates();
        }

        private void ShowCoordinates()
        {
            var endCoordinates = shape.GetCoordinates().Select(x => Math.Round(x)).ToArray();
            endsCoord.Content = 
                "(" + endCoordinates[0] + "; " + endCoordinates[1] + "; " + endCoordinates[4] + ")\n" +
                "(" + endCoordinates[2] + "; " + endCoordinates[3] + "; " + endCoordinates[5] + ")";
        }

        private void phiSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            phi = Math.Round(e.NewValue) * 0.1;
            /*var md = new MatrixData(phi, 0, zc);
            var rotateMatrix = new double[,]
            {
                { 1, 0, 0, 0},
                { 0, md.CosPhi, md.SinPhi, 0 },
                { 0, -md.SinPhi, md.CosPhi, 0 },
                { 0, 0, 0, 1 }
            };*/

            var md = new MatrixData(phi, zc);
            var rotateMatrix = new double[,]
            {
                { 1, 0, 0, 0},
                { 0, md.Cos, md.Sin, 0 },
                { 0, -md.Sin, md.Cos, 0 },
                { 0, 0, 0, 1 }
            };

            shape.ComputeReal3D(rotateMatrix);

            shape.ProjectReal3D(md.Zc);

            SetValuesDataGrid(rotateMatrix);

            ShowCoordinates();

            shape.SetDataLineCoordinates(dataLine);
        }

        private void thetaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            theta = Math.Round(e.NewValue) * 0.1;
            /*var md = new MatrixData(0, theta, zc);
            var rotateMatrix = new double[,]
            {
                { md.CosTheta, 0, -md.SinTheta, 0},
                { 0, 1, 0, 0 },
                { md.SinTheta, 0,  md.CosTheta, 0 },
                { 0, 0, 0, 1 }
            };*/
            var md = new MatrixData(theta, zc);
            var rotateMatrix = new double[,]
            {
                { md.Cos, 0, -md.Sin, 0},
                { 0, 1, 0, 0 },
                { md.Sin, 0,  md.Cos, 0 },
                { 0, 0, 0, 1 }
            };

             shape.ComputeReal3D(rotateMatrix);

            shape.ProjectReal3D(md.Zc);

            SetValuesDataGrid(rotateMatrix);

            ShowCoordinates();

            shape.SetDataLineCoordinates(dataLine);
        }

        private void zcSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            zc = Math.Round(e.NewValue) * 10;
            MatrixData md = new MatrixData(0, zc);

            shape.ProjectReal3D(md.Zc);
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

        private void createMedian_Click(object sender, RoutedEventArgs e)
        {
            median = true;
            height = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            height = true;
            median = false;
        }

        private void gammaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            gamma = Math.Round(e.NewValue) * 0.1;
            /*var md = new MatrixData(phi, 0, zc);
            var rotateMatrix = new double[,]
            {
                { 1, 0, 0, 0},
                { 0, md.CosPhi, md.SinPhi, 0 },
                { 0, -md.SinPhi, md.CosPhi, 0 },
                { 0, 0, 0, 1 }
            };*/
            var md = new MatrixData(gamma, zc);
            var rotateMatrix = new double[,]
            {
                { md.Cos, md.Sin, 0, 0},
                { -md.Sin, md.Cos, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };

            shape.ComputeReal3D(rotateMatrix);

            shape.ProjectReal3D(md.Zc);

            SetValuesDataGrid(rotateMatrix);

            ShowCoordinates();

            shape.SetDataLineCoordinates(dataLine);
        }
    }
}
