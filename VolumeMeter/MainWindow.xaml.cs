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

namespace VolumeMeter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int VMHeight = 400, VMWidth = 40, NBoxes = 15, Separation = 2;  
        private readonly double VMGreenToYellow = 0.6, VMYellowToRed = 0.85;

        private List<Rectangle> rectangles = new List<Rectangle>();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CreateVolumeMeter(VMHeight, VMWidth, NBoxes, Separation);

            PreviewKeyDown += MainWindow_KeyDown;
        }

        private void CreateVolumeMeter(int height, int width, int nBoxes, int separation)
        {
            myCanvas.Height = height;
            myCanvas.Width = width;

            int avaliableSpace = height - (nBoxes - 1) * separation;

            double boxSize = (avaliableSpace * 1.0) / nBoxes;

            double start = height;
            for (var i = 0; i < nBoxes; i++)
            {
                Rectangle r = new Rectangle
                {
                    Height = boxSize,
                    Width = width
                };

                start -= boxSize;
                Canvas.SetTop(r, start);
                Canvas.SetLeft(r, 0);

                start -= separation;

                myCanvas.Children.Add(r);
                rectangles.Add(r);
            }            
        }

        private void mySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var activationRange = e.NewValue * NBoxes / 100.0;

            for (var i = 0; i < NBoxes; i++)
            {
                Rectangle r = rectangles[i];
                if (i < activationRange)
                {
                    double range = i * 1.0 / NBoxes;
                    if (range < VMGreenToYellow)
                        r.Fill = Brushes.Green;
                    else if (range < VMYellowToRed)
                        r.Fill = Brushes.Yellow;
                    else
                        r.Fill = Brushes.Red;
                }
                else
                    r.Fill = Brushes.White;
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    mySlider.Value -= 1;
                    break;
                case Key.Right:
                    mySlider.Value += 1;
                    break;
            }
        }
    }
}
