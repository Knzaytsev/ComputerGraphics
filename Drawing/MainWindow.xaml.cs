﻿using Drawing.Composite;
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
        private Composite.Composite shape;
        private bool downed = false;
        private Point oldPoint;

        public MainWindow()
        {
            InitializeComponent();
            coordinateSystem = new CoordinateSystem2DInteractor(canvas.Width, canvas.Height);
            canvas.Children.Add(currentLine);
            shape = new Composite.Composite(currentLine, coordinateSystem);
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
            shape = new Composite.Composite(currentLine, coordinateSystem);
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
                    shape = new Composite.Composite(currentLine, coordinateSystem);
                }

                shape.PickAddShape(e.Source as Shape);

                var equation = shape.GetEquation();
                lineEquation.Content = "(" + equation[0] + "; " + equation[1] + "; " + equation[2] + ")";
            }
            /*if (downed && !(e.Source as Shape is null))
            {
                currentLine = shape.PickAddShape(e.Source as Shape) as Line;
                //currentLine = interactor.PickShape(e.Source as Shape) as Line;
                //shape.Add(new Composite.Composite(currentLine));
            }
            else if (!(e.Source as Shape is null))
            {
                shape.ClearColor();
                //currentLine.Stroke = Brushes.Black;
                currentLine = interactor.PickShape(e.Source as Shape) as Line;
                shape = new Composite.Composite(currentLine);
                var equation = interactor.GetEquation(currentLine, coordinateSystem);
                lineEquation.Content = "(" + equation[0] + "; " + equation[1] + "; " + equation[2] + ")";
                picked = true;
            }*/
        }

        private void dragElement_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                shape.MoveShape(oldPoint, e.GetPosition(canvas));
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
                var createdLines = coordinateSystem.CreateAxes(canvas.Width, canvas.Height, 50);
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

        private void dropElement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Shape)
            {
                var equation = shape.GetEquation();
                lineEquation.Content = "(" + equation[0] + "; " + equation[1] + "; " + equation[2] + ")";
            }
        }
    }
}
