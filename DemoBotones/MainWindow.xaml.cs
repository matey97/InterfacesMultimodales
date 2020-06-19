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

namespace DemoBotones
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
                new Action<bool, bool>((bool stateA, bool stateB) =>
                    {
                        A_label.Foreground = stateA ? Brushes.White : Brushes.Black;
                        B_label.Foreground = stateB ? Brushes.White : Brushes.Black;

                        A_label.Background = stateA ? Brushes.Green : Brushes.Transparent;
                        B_label.Background = stateB ? Brushes.Red : Brushes.Transparent;
                    }), 
                obj.ButtonState.A, 
                obj.ButtonState.B);
        }

        private void ChangeButtonState(bool stateA, bool stateB)
        {
            A_label.Foreground = stateA ? Brushes.White : Brushes.Black;
            B_label.Foreground = stateB ? Brushes.White : Brushes.Black;

            A_label.Background = stateA ? Brushes.Green : Brushes.Transparent;
            B_label.Background = stateB ? Brushes.Red : Brushes.Transparent;
        }
    }
}
