using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace ReservaVuelos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpeechRecognitionEngine speechRecognizer;
        private string[] destinationCities = new string[] { "Madrid", "Barcelona", "Londres", "París", "Berlín" };
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Grammar grammar = new Grammar("flight_booking_grammar.srgs");

            speechRecognizer = new SpeechRecognitionEngine();
            speechRecognizer.LoadGrammar(grammar);
            speechRecognizer.SpeechRecognized += SpeechRecognized;
            speechRecognizer.SpeechRecognitionRejected += SpeechRecognitionRejected;
            speechRecognizer.SpeechDetected += SpeechDetected;
            speechRecognizer.SetInputToDefaultAudioDevice();
            
            foreach (string city in destinationCities)
            {
                LV_destinationCities.Items.Add(city);
            }
        }

        private void SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            TB_recognizedText.Background = Brushes.LightCyan;
            TB_recognizedText.Text = "<Voz detectada>";
        }
        private void SpeechRecognitionRejected(object s, SpeechRecognitionRejectedEventArgs e)
        {
            TB_recognizedText.Background = Brushes.LightYellow;
            TB_recognizedText.Text = "<No le he oido bien. Repita por favor>";
            B_speak.IsEnabled = true;
        }
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            TB_recognizedText.Background = Brushes.LightGreen;
            TB_recognizedText.Text = Capitalize(e.Result.Text);

            SemanticValue semantics = e.Result.Semantics;

            // La frase ¿Hay algun vuelo para mañana? añade una variable a la semantica para indicar que se ha reconocido esa frase
            // Si dicha variable se encuentra en la gramatica se muestran los destinos disponibles y si no se extrae más información de la semantica
            if (semantics.ContainsKey("TomorrowFlights"))
            {
                SP_destinationsAvaliable.Visibility = Visibility.Visible;
            } else
            {
                ChangeStackPanelsVisibility(true);
                // Por defecto Castellón como origen, una mejora seria indicarlo en la gramatica
                TB_origin.Text = "Castellón";
                TB_destination.Text = semantics["Destination"].Value.ToString();
                TB_type.Text = semantics["Type"].Value.ToString();

                if (semantics.ContainsKey("Price"))
                    TB_price.Text = semantics["Price"].Value.ToString();
            }

            B_speak.IsEnabled = true;
        }

        private void B_speak_Click(object sender, RoutedEventArgs e)
        {
            B_speak.IsEnabled = false;
            TB_recognizedText.Background = Brushes.LightGray;
            SP_destinationsAvaliable.Visibility = Visibility.Collapsed;
            ClearTextBoxes();
            ChangeStackPanelsVisibility(false);
            // El modo single detiene el reconocedor tras realizar el reconocimiento
            // Otra opcion es emplear el modo multiple y parar manualmente con RecognizeAsyncStop
            speechRecognizer.RecognizeAsync(RecognizeMode.Single);
        }

        // Pone en mayuscula la primera letra de la frase reconocida
        private string Capitalize(string s)
        {
            char[] letters = s.ToCharArray();
            int firstLetter = 0;
            if (letters[0].Equals('¿'))
                firstLetter = 1;
            
            letters[firstLetter] = char.ToUpper(letters[firstLetter]);
            return new string(letters);
        }

        private void ChangeStackPanelsVisibility(bool visible)
        {
            var visibility = visible ? Visibility.Visible : Visibility.Hidden;
            SP_origin.Visibility = visibility;
            SP_typeAndPrice.Visibility = visibility;
            SP_destination.Visibility = visibility;
        }

        private void ClearTextBoxes()
        {
            TB_recognizedText.Text = "";
            TB_origin.Text = "";
            TB_destination.Text = "";
            TB_price.Text = "";
            TB_type.Text = "";
        }
    }
}
