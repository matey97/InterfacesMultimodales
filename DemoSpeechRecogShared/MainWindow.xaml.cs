using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
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

namespace DemoSpeechRecogShared
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpeechRecognizer sharedRecognizer = new SpeechRecognizer() { Enabled = true };
        Grammar grammar = new DictationGrammar();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sharedRecognizer.LoadGrammar(grammar);
            sharedRecognizer.SpeechRecognized += SpeechRecognized;
            sharedRecognizer.SpeechRecognitionRejected += SpeechRecognitionRejected;
            sharedRecognizer.SpeechDetected += SpeechDetected;
        }

        void SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            labelTextoReconocido.Content = "<Voz detectada>";
            labelProbabilidad.Content = "";
        }
        void SpeechRecognitionRejected(object s, SpeechRecognitionRejectedEventArgs e)
        {
            labelTextoReconocido.Content = "<No le he oído bien. Repita por favor>";
            labelProbabilidad.Content = "";
        }
        void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            labelTextoReconocido.Content = e.Result.Text;
            labelProbabilidad.Content = e.Result.Confidence.ToString();
        }
    }
}
