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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Formas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs eArg)
        {
            //Creacion rectangulo y propiedades
            Rectangle r = new Rectangle
            {
                Width = 200,
                Height = 150,
                Stroke = Brushes.Black,
                StrokeThickness = 3,
                Fill = Brushes.Orange
            };
            r.MouseDown += R_MouseDown;

            Ellipse e = new Ellipse
            {
                Width = 200,
                Height = 150,
                Stroke = Brushes.Black,
                StrokeDashArray = new DoubleCollection() { 2, 3, 4 },
                StrokeThickness = 3,
                Fill = Brushes.Orange
            };

            Line l = new Line() 
            { 
                X1 = 0, 
                Y1 = 0,
                X2 = 30, 
                Y2 = 30, 
                StrokeThickness = 4, 
                Stroke = Brushes.Black 
            };
            
            //Posicionamiento de objetos en el canvas al asignarlo
            Canvas.SetTop(r, 100);
            Canvas.SetLeft(r, 100);
            Canvas.SetTop(e, 300);
            Canvas.SetLeft(e, 100);
            Canvas.SetTop(l, 300);
            Canvas.SetLeft(l, 300);

            //Asignacion de objetos al canvas
            canvas.Children.Add(r);
            canvas.Children.Add(e);
            canvas.Children.Add(l);

            Slider slider = new Slider
            {
                Minimum = 0,
                Maximum = canvas.ActualWidth - r.Width,
                Width = 200
            };
            canvas.Children.Add(slider);

            Binding binding = new Binding("Value")
            {
                Source = slider
            };
            BindingOperations.SetBinding(r, Canvas.LeftProperty, binding);


            Slider slider3 = new Slider()
            {
                Minimum = 0,
                Maximum = 360, //Grados
                Width = 200,
                Height = 200
            };
            Canvas.SetTop(slider3, 250);
            canvas.Children.Add(slider3);

            RotateTransform rt = new RotateTransform() 
            { 
                CenterX = r.Width / 2, //Centro de rotación --> centro del rectangulo
                CenterY = r.Height / 2 
            };
            TransformGroup tg = new TransformGroup(); //Puede tener rotación, translación o escalado
            tg.Children.Add(rt);

            //Transformación visual
            r.RenderTransform = tg;

            //LayoutTransform --> transformación completa, afecta a los elementos a su alrededor
            
            Binding binding3 = new Binding("Value") { Source = slider3};
            BindingOperations.SetBinding(rt, RotateTransform.AngleProperty, binding3);



            
            RotateTransform rt2 = new RotateTransform()
            {
                CenterX = e.Width / 2,
                CenterY = e.Height / 2
            };
            TransformGroup tg2 = new TransformGroup();
            tg2.Children.Add(rt2);
            e.RenderTransform = tg2;

            DoubleAnimation an = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(5)),
                From = 0,
                To = 360,
                RepeatBehavior = RepeatBehavior.Forever
            };
            rt2.BeginAnimation(RotateTransform.AngleProperty, an);
        }

        private void R_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Me diste!!");
        }
    }
}
