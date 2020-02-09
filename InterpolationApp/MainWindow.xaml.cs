using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using Spline_Test;

namespace InterpolationApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Point> points;
        List<Point> resultPoints;
        public MainWindow()
        {
            InitializeComponent();

            points = File.LoadFile();
            //points = new List<Point>
            //{
            //    new Point(0,1),
            //    new Point(1,2),
            //    new Point(2,3),
            //    new Point(3,7),
            //    new Point(4,6),
            //    new Point(5,5),
            //    new Point(6,1),
            //    new Point(7,2),
            //    new Point(8,3),
            //    new Point(9,1),
            //    new Point(10, 2)
            //};
            PointsGrid.ItemsSource = points;
            points = points.OrderBy( i => i.x ).ToList();

            
        }

        private void ShowCubicSpline()
        {
            wpfPlot1.plt.Clear();
            wpfPlot1.plt.Title("Cubic Spline");
            wpfPlot1.plt.PlotScatter(resultPoints.ConvertAll(new Converter<Point, double>(Point.PointToX)).ToArray(),
                resultPoints.ConvertAll(new Converter<Point, double>(Point.PointToY)).ToArray(), lineWidth:0);
            wpfPlot1.plt.PlotScatter( points.ConvertAll( new Converter<Point, double>( Point.PointToX ) ).ToArray(),
                points.ConvertAll( new Converter<Point, double>( Point.PointToY ) ).ToArray(), lineWidth:0, markerSize:10 );
            wpfPlot1.Render();
            CompositionTarget.Rendering += OnRendering;
        }

        private bool CalculateCubicSpline()
        {
            SplineInterpolation splineInterpolation = new SplineInterpolation(
                points.ConvertAll(new Converter<Point, double>(Point.PointToX)),
                points.ConvertAll(new Converter<Point, double>(Point.PointToY)));
            resultPoints = new List<Point>();
            double step;
            if( Double.TryParse( TextBoxStep.Text.Replace(".",","), out step))
            {
                for (double x = points.First().x; x < points.Last().x; x = x + step)
                    resultPoints.Add(new Point(x, splineInterpolation.Interpolate(x)));
                if (resultPoints.Last().x < points.Last().x)
                    resultPoints.Add(points.Last());
                return true;
            }
            else
            {
                MessageBox.Show( "Ошибка: Шаг указан неверно!" );
                return false;
            }
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            points = PointsGrid.Items.OfType<Point>().ToList().OrderBy(i => i.x).ToList();
            if(CalculateCubicSpline()) 
                ShowCubicSpline();
        }

        private void Button_Click_1( object sender, RoutedEventArgs e )
        {
            
            Point newPoint = new Point( points.Last().x + 1 , 0);
            points.Add(newPoint);
            PointsGrid.ItemsSource = points;
            Button_Click(sender, e);
        }

        private void Button_Click_2( object sender, RoutedEventArgs e )
        {
            points.RemoveAt(points.Count-1);
            PointsGrid.ItemsSource = points;
            Button_Click(sender, e);
        }

        private void WindowClosing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            File.SaveFile(points);
        }

        private void OnRendering(object sender, EventArgs e)
        {
            ShowCoordinatesOnHover();
        }

        private void ShowCoordinatesOnHover()
        {
            //Координаты координатной сетки
            var mouse = wpfPlot1.mouseCoordinates;
            mouse.X = Math.Round(mouse.X, 2);
            mouse.Y = Math.Round(mouse.Y, 2);

            foreach (var point in resultPoints)
            {
                if ((Math.Abs(point.x - mouse.X) < 0.05) && (Math.Abs(point.y - mouse.Y) < 0.05))
                {
                    CoordinatesTextBlock.Text = point.ToString();
                    //Координаты окне
                    CoordinatesTextBlock.Margin =
                        new Thickness(Mouse.GetPosition(wpfPlot1).X + 15, Mouse.GetPosition(wpfPlot1).Y, 0, 0);
                    break;
                }
                else
                    CoordinatesTextBlock.Text = "";
            }
        }
    }
}
