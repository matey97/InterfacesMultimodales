using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DemoInkCanvas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DrawingAttributes da = new DrawingAttributes();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window1_Loaded;
        }
        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            wSlider.Minimum = DrawingAttributes.MinWidth;
            wSlider.Maximum = 10;
            hSlider.Minimum = DrawingAttributes.MinHeight;
            hSlider.Maximum = 10;
            wSlider.Value = 1.0; hSlider.Value = 1.0;
            CanvasDeTinta.DefaultDrawingAttributes = da;

            StrokeCollection strokes = CanvasDeTinta.Strokes;
            Stroke s = strokes[0];
            StylusPointCollection puntos = s.StylusPoints;
            StylusPoint punto = puntos[0];

            double x = punto.X;
            double y = punto.Y;
            double presion = punto.PressureFactor;

        }
        private void wSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            da.Width = wSlider.Value;
        }

        private void hSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            da.Height = hSlider.Value;
        }

        private void ComboBox_SelectionChanged(object sender,
        SelectionChangedEventArgs e)
        {
            switch ((e.Source as ComboBox).SelectedIndex)
            {
                case 0:
                    da.Color = Colors.Black;
                    break;
                case 1:
                    da.Color = Colors.Red;
                    break;
                case 2:
                    da.Color = Colors.Blue;
                    break;
            }
        }
    }
}
