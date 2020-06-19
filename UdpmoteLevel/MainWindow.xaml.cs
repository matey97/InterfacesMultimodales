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
using System.Windows.Threading;
using UdpmoteLib;

namespace UdpmoteLevel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Udpmote udpmote = new Udpmote();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            udpmote.UdpmoteChanged += Udpmote_UdpmoteChanged;
            udpmote.Connect();
        }

        private void Udpmote_UdpmoteChanged(UdpmoteState obj)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action<double>((double y) =>
                {
                    y = Math.Min(Math.Max(0.0, y), 1.0);
                    double angle = 90 - 180.0 * Math.Acos(y) / Math.PI;

                    L_Degrees.Content = string.Format("{0}º", angle);
                }), obj.AccelState.Y);
        }
    }
}
