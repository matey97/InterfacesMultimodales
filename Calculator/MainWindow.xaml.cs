using Microsoft.Ink;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Expresiones regulares para validar caracteres individuales y expresión completa
        private const string acceptedCharRegex = @"^(\d+(\.\d+)?|[+-\/*xX>()]){1}$";
        private const string acceptedExpressionRegex = @"^(([-+\/*xX]+\()?[-+\/*xX]?\d+(\.\d+)?\)?)*$";

        private const string infoTextMode1 = "Introduce digitos individualmente. Introduce > para finalizar la expresión.";
        private const string infoTextMode2 = "Introduce la expresión completa.";

        private DispatcherTimer timer = new DispatcherTimer();
        private bool expressionDone = false;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Tick += Timer_Tick;

            calculatorCanvas.MouseMove += CalculatorCanvas_MouseMove;

            RB_Mode1.Checked += RB_Mode1_Checked;
            RB_Mode2.Checked += RB_Mode2_Checked;

            RB_Mode1.IsChecked = true;

            RB_Draw.Checked += RB_Draw_Checked;
            RB_Erase.Checked += RB_Erase_Checked;
        }


        private void CalculatorCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                timer.Stop();
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                calculatorCanvas.Strokes.Save(ms);
                var ink = new Ink();
                ink.Load(ms.ToArray());
                using (RecognizerContext context = new RecognizerContext())
                {
                    if (ink.Strokes.Count > 0)
                    {
                        // Se borra la expresión del text box si se va a evaluar una nueva expresión
                        if (expressionDone)
                        {
                            resultBox.Text = "";
                            expressionDone = false;
                        }
                           
                        context.Strokes = ink.Strokes;
                        RecognitionStatus status;
                        var result = context.Recognize(out status);
                        if (status == RecognitionStatus.NoError)
                        {
                            if (RB_Mode1.IsChecked == true)
                                HandleMode1(result);
                            else
                                HandleMode2(result);                           
                        }                            
                        else
                            MessageBox.Show("Recognition failed");
                    }
                }
            }
        }

        private void HandleMode1(RecognitionResult result)
        {
            // Si no hay un resultado claro al reconocer el caracter solo se limpia el canvas
            if (result == null)
            {
                ResetCanvasAndTimer();
                return;
            }

            var resultString = result.TopString;

            // Si el caracter reconocido no es valido solo se limpia el canvas
            if (!IsAcceptedChar(resultString))
            {
                ResetCanvasAndTimer();
                return;
            }

            // Cuando se reconoce ">" se comprueba la expresión introducida hasta el momento y
            // si es valida se muestra su resultado
            if (resultString.Equals(">"))
            {
                if (!IsAcceptedExpression(resultBox.Text))
                {
                    MessageBox.Show("Expresión no valida", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    expressionDone = true;
                    ResetCanvasAndTimer();
                    return;
                }

                DataTable dt = new DataTable();
                var value = dt.Compute(resultBox.Text, "");
                resultString = "=" + value;

                expressionDone = true;
            }

            resultString = Regex.Replace(resultString, @"[xX]", "*");

            resultBox.Text += resultString;

            ResetCanvasAndTimer();
        }

        private void HandleMode2(RecognitionResult result)
        {
            var resultString = result.TopString;

            // Se reemplazan las x o X por * y se eliminan los posibles blancos
            resultString = Regex.Replace(resultString, @"[xX]", "*");
            resultString = Regex.Replace(resultString, @"\s+", "");

            // Si la expresión es correcta se calcula y se muestra el resultado
            if (!IsAcceptedExpression(resultString))
            {
                return;
            }

            DataTable dt = new DataTable();
            var value = dt.Compute(resultString, "");
            resultString += "=" + value;

            resultBox.Text += resultString;
            expressionDone = true;
        }

        private void ResetCanvasAndTimer()
        {
            calculatorCanvas.Strokes.Clear();
            timer.Stop();
        }

        private bool IsAcceptedChar(string s)
        {
            return Regex.Match(s, acceptedCharRegex).Success;
        }

        // Comprueba la validez de los caracteres de la expresión mediante un expresión regular
        // y que cada parentesis de abertura tenga uno de cierre. No es una comprobación perfecta
        // porque no se comprueba estrictamente que un parentesis abierto tenga su parentesis de cierre
        // Ej: Podría aparecer un parentesis de cierre antes del de apertura.
        private bool IsAcceptedExpression(string s)
        {
            int openingParenthesis = s.Count(c => c == '(');
            int closingParenthesis = s.Count(c => c == ')');
            return openingParenthesis == closingParenthesis &&
                Regex.Match(s, acceptedExpressionRegex).Success;
        }

        private void RB_Mode1_Checked(object sender, RoutedEventArgs e)
        {
            calculatorCanvas.EditingMode = InkCanvasEditingMode.Ink;

            TextInfo.Text = infoTextMode1;
            timer.Interval = TimeSpan.FromMilliseconds(500);
            expressionDone = true;

            SP_DrawErase.Visibility = Visibility.Collapsed;
            clearCanvas.Visibility = Visibility.Collapsed;
            resultBox.Text = "";

            ResetCanvasAndTimer();
        }

        private void RB_Mode2_Checked(object sender, RoutedEventArgs e)
        {
            TextInfo.Text = infoTextMode2;
            timer.Interval = TimeSpan.FromMilliseconds(1500);
            expressionDone = true;

            SP_DrawErase.Visibility = Visibility.Visible;
            clearCanvas.Visibility = Visibility.Visible;
            resultBox.Text = "";
        }

        private void ClearCanvas(object sender, RoutedEventArgs e)
        {
            ResetCanvasAndTimer();
            resultBox.Text = "";
        }

        private void RB_Draw_Checked(object sender, RoutedEventArgs e)
        {
            calculatorCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void RB_Erase_Checked(object sender, RoutedEventArgs e)
        {
            calculatorCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }
    }
}
