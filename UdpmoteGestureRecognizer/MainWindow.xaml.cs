using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace UdpmoteGestureRecognizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Udpmote udpmote;
        private GestureCapturer gestureCapturer;
        private GestureRecognizer gestureRecognizer;

        private List<Gesture> prototypes = new List<Gesture>();
        private bool isRecognizing = false;
        private bool udpmoteIsConnected = false;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            udpmote = new Udpmote();
            gestureCapturer = new GestureCapturer();
            gestureRecognizer = new GestureRecognizer();

            // Enlaza el capturador con el reconocedor
            gestureCapturer.GestureCaptured += gestureRecognizer.OnGestureCaptured;
            gestureRecognizer.GestureRecognized += GestureRecognizer_GestureRecognized;

            udpmote.UdpmoteConnected += Udpmote_UdpmoteConnected;
            udpmote.UdpmoteDisconnected += Udpmote_UdpmoteDisconnected;
            udpmote.Connect();
        }

        private void GestureRecognizer_GestureRecognized(string gesture)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<string>((g) => {
                RecognizedGesture.Text = g;
                RecognizedGesture.Background = Brushes.LightGreen;
            }), gesture);
        }

        private void Udpmote_UdpmoteConnected(UdpmoteInfo obj)
        {
            udpmoteIsConnected = true;
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => TryEnableStartButton()));
        }

        private void Udpmote_UdpmoteDisconnected(UdpmoteInfo obj)
        {
            udpmoteIsConnected = false;
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                isRecognizing = false;
                StartStopButton.IsEnabled = false;
                RecognizedGesture.Text = "";
                RecognizedGesture.Background = Brushes.LightGray;
                StartStopButton.Content = "Empezar reconocimiento";
                }));
        }

        // Registra/desregistra el listener del evento de udpmote dependiendo de si va a empezar el reconocimiento o va a finalizar
        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRecognizing)
            {
                udpmote.UdpmoteChanged -= gestureCapturer.OnUdpmoteChanged;
                RecognizedGesture.Text = "";
                RecognizedGesture.Background = Brushes.LightGray;
                StartStopButton.Content = "Empezar reconocimiento";
            } else
            {
                udpmote.UdpmoteChanged += gestureCapturer.OnUdpmoteChanged;
                StartStopButton.Content = "Detener reconocimiento";
            }

            isRecognizing = !isRecognizing;
        }

        private void MenuCargar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".bin";
            openFileDialog.Filter = "Binary files (.bin)|*.bin";

            bool result = openFileDialog.ShowDialog() == true;

            if (result)
            {
                using (Stream stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    prototypes =(List<Gesture>)bf.Deserialize(stream);
                    foreach (Gesture prototype in prototypes)
                        gestureRecognizer.AddPrototypes(prototype);

                    TryEnableStartButton();
                }
            }
        }

        // Intenta habilitar el boton de inicio de reconocimiento
        private void TryEnableStartButton()
        {
            if (prototypes.Count > 0 && udpmoteIsConnected)
                StartStopButton.IsEnabled = true;
        }
    }
}
