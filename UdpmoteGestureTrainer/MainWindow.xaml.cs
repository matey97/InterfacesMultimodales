using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using UdpmoteGestureLib;
using UdpmoteLib;

namespace UdpmoteGestureTrainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Udpmote udpmote;
        private GestureCapturer gestureCapturer;

        private List<Gesture> gesturesCaptured;

        private int gestureBeingTrained;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            udpmote = new Udpmote();
            gestureCapturer = new GestureCapturer();
            gesturesCaptured = new List<Gesture>();

            udpmote.UdpmoteConnected += Udpmote_UdpmoteConnected;
            udpmote.UdpmoteDisconnected += Udpmote_UdpmoteDisconnected;
            gestureCapturer.GestureCaptured += GestureCapturer_GestureCaptured;
            udpmote.Connect();
        }

        private void Udpmote_UdpmoteConnected(UdpmoteInfo obj)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action(() => TrainGestures.IsEnabled = true));
        }

        private void Udpmote_UdpmoteDisconnected(UdpmoteInfo obj)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action(() => TrainGestures.IsEnabled = false));
        }

        private void GestureCapturer_GestureCaptured(Gesture g)
        {
            g.Name = gesturesToTrain.Items[gestureBeingTrained].ToString();
            gesturesCaptured.Add(g);

            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action(() => TrainNextGesture()));
        }

        private void TrainNextGesture()
        {
            // Cambia el color del elemento en la lista para indicar que ya esta entrenado
            if (gestureBeingTrained >= 0) 
                (gesturesToTrain.ItemContainerGenerator.ContainerFromIndex(gestureBeingTrained) as ListViewItem).Background = Brushes.LightGreen;

            gestureBeingTrained++;

            // Cuando ya se han entrenado todos los gestos se desregistra el listener
            if (gestureBeingTrained == gesturesToTrain.Items.Count)
            {
                udpmote.UdpmoteChanged -= gestureCapturer.OnUdpmoteChanged;
                TrainGestures.IsEnabled = true;
                TB_GestureTraining.Text = "";
                TB_GestureTraining.Background = Brushes.LightGray;
                return;
            }
            
            // Se indica el gesto que se va a entrenar
            TB_GestureTraining.Text = gesturesToTrain.Items[gestureBeingTrained].ToString();
            TB_GestureTraining.Background = Brushes.LightYellow;
        }

        #region ClickHandlers
        private void AddGesture_Click(object sender, RoutedEventArgs e)
        {
            gesturesToTrain.Items.Add(TB_NewGesture.Text);
            TB_NewGesture.Text = "";
            ClearList.IsEnabled = true;
        }

        // Registra el listener del capturador al evento del udpmote
        private void TrainGestures_Click(object sender, RoutedEventArgs e)
        {
            TrainGestures.IsEnabled = false;

            gestureBeingTrained = -1;
            gesturesCaptured.Clear();

            udpmote.UdpmoteChanged += gestureCapturer.OnUdpmoteChanged;
            TrainNextGesture();
        }

        private void ClearList_Click(object sender, RoutedEventArgs e)
        {
            gesturesToTrain.Items.Clear();
            ClearList.IsEnabled = false;
        }
        #endregion

        #region Persistence
        private void MenuCargar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool result = ShowFileDialog(openFileDialog);

            if (result)
            {
                using (Stream stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    gesturesCaptured = (List<Gesture>) bf.Deserialize(stream);

                    gesturesToTrain.Items.Clear();
                    foreach (Gesture g in gesturesCaptured)
                    {
                        gesturesToTrain.Items.Add(g.Name);
                    }
                }
            }
        }

        private void MenuGuardar_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            bool result = ShowFileDialog(saveFileDialog);

            if (result)
            {
                using (Stream stream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(stream, gesturesCaptured);
                }
            }
        }

        private bool ShowFileDialog(FileDialog dialog)
        {
            dialog.FileName = "prototypes";
            dialog.DefaultExt = ".bin";
            dialog.Filter = "Binary files (.bin)|*.bin";

            return dialog.ShowDialog() == true;
        }
        #endregion
    }
}
