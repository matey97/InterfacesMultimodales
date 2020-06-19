using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
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

namespace DemoSynthesis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpeechSynthesizer synth = new SpeechSynthesizer();
        private IList<InstalledVoice> voices;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TB_text.Focus();
            voices = synth.GetInstalledVoices();

            CB_voices.ItemsSource = voices;
            CB_voices.DisplayMemberPath = "VoiceInfo.Description";
            //CB_voices.SelectedValuePath = "VoiceInfo.Id";
            if (voices.Count > 0)
                CB_voices.SelectedIndex = 0;

            Button_speak.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            synth.Speak(TB_text.Text);
        }

        private void CB_voices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            synth.SelectVoice(voices[CB_voices.SelectedIndex].VoiceInfo.Name);
        }

        private void TB_text_TextChanged(object sender, TextChangedEventArgs e)
        {
            Button_speak.IsEnabled = TB_text.Text.Length != 0;
        }
    }
}
