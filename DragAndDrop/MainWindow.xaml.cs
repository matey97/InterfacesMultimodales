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

namespace DragAndDrop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Ellipse focusedCircle = null;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            canvas.MouseDown += Canvas_PreviewMouseDown;
        }

        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var c = new Ellipse { Width = 40, Height = 40, Stroke = Brushes.Black, Fill = Brushes.Yellow };
            c.MouseDown += C_MouseDown;
            c.MouseUp += C_MouseUp;
            c.MouseMove += C_MouseMove;

            var p = e.GetPosition(canvas);
            Canvas.SetTop(c, p.Y - c.Height/2.0);
            Canvas.SetLeft(c, p.X - c.Width/2.0);

            canvas.Children.Add(c);
        }

        private void C_MouseDown(object sender, MouseButtonEventArgs e)
        {
            focusedCircle = (Ellipse)sender;
            focusedCircle.CaptureMouse();
            e.Handled = true;
        }

        private void C_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (focusedCircle == null)
                return;
            focusedCircle.ReleaseMouseCapture();
            focusedCircle = null;
            e.Handled = true;
        }

        private void C_MouseMove(object sender, MouseEventArgs e)
        {
            if (focusedCircle == null)
                return;

            var p = e.GetPosition(canvas);
            Canvas.SetTop(focusedCircle, p.Y - focusedCircle.Height/2.0);
            Canvas.SetLeft(focusedCircle, p.X - focusedCircle.Width/2.0);
        }
    }
}
