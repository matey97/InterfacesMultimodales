using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
using System.Xml.Serialization;
using UdpmoteLib;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    // Enumeración para representar las posibles direcciones de la cabeza de la serpiente
    public enum Heading { LEFT, UP, RIGHT, DOWN };
    public partial class MainWindow : Window
    {
        private const int CANVAS_HEIGHT = 400, CANVAS_WIDTH = 400;
        private const int ITEM_SIZE = 20;
        private const int SNAKE_INITIAL_SIZE = 3;
        private const int POINTS_PER_FRUIT = 50;
        private const int POINTS_PER_LEVEL = 150;
        private const int SPEEDUP = 50;
        private int FRUIT_VALUE = 2;

        private const string NEW_GAME_TEXT = "Nueva partida [ENTER]";
        private const string RESUME_GAME_TEXT = "Continuar [ENTER]";
        private const string PAUSE_GAME_TEXT = "Pause [ESC]";
        private const string RECORD_TEXT = "Record: {0}";
        private const string SCORE_TEXT = "Puntuación: {0}";
        private const string LEVEL_TEXT = "Nivel {0}";

        private readonly List<Heading> headings = new List<Heading>() { Heading.LEFT, Heading.UP, Heading.RIGHT, Heading.DOWN };
        private readonly LinkedList<Rectangle> snake = new LinkedList<Rectangle>(); // LinkedList para poder añadir al principio de la lista

        private Ellipse fruit;
        private Rectangle overlay;

        private bool alreadyChangedHeading;
        private bool isGameOngoing;
        private bool isGamePaused;

        private int partsToGrow = 0;
        private int maxScore;
        private int score;
        private int level;
        private int currentHeading;
        // Indice sobre la lista headings que indica la dirección actual de la serpiente.
        // La lista headings sirve como una lista circular. 
        // Cuando se haga click a izquierda, el indice se decrementara. Ej: Si RIGHT --> UP, Si LEFT --> DOWN
        // Cuando se haga click a derecha, el indice se incrementara. Ej: Si UP --> RIGHT, Si DOWN --> LEFT
        public int CurrentHeading {
            get {
                return currentHeading;
            }

            set {
                // Check de seguridad: Si se hace click lo suficientemente rapido dos veces a izquierda (o derecha)
                // la serpiente puede "darse la vuelta sobre si misma" y se acaba el juego. 
                // Esta condición permite solo un cambio de dirección por tick
                if (alreadyChangedHeading)
                    return;

                currentHeading = value < 0 ? headings.Count - 1 : value % headings.Count;
                alreadyChangedHeading = true;
            } 
        }

        private Timer timer;
        private Random random;
        private SoundPlayer startAndPause, eatFruit;

        // Conjuntos para guardar las coordenadas posibles y las que estan en uso
        private HashSet<(double, double)> coordinates = new HashSet<(double, double)>();
        private HashSet<(double, double)> usedCoordinates = new HashSet<(double, double)>();

        private Udpmote udpmote = new Udpmote();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            alreadyChangedHeading = false;
            random = new Random();
            SetupCoordinates();
            SetupSounds();
            SetupOverlay();
            SetupInfoUI();
            PreviewKeyDown += MainWindow_PreviewKeyDown;

            udpmote.UdpmoteChanged += Udpmote_UdpmoteChanged;
            udpmote.Connect();
        }

        private void SetupCoordinates()
        {
            for (var i = 0; i < CANVAS_HEIGHT; i += ITEM_SIZE)
            {
                for (var j = 0; j < CANVAS_WIDTH; j+= ITEM_SIZE)
                {
                    coordinates.Add((i, j));
                }
            }
        }

        private void SetupSnake()
        {
            // Se elimina la serpiente del tablero, util al empezar un juego despues de finalizar el anterior
            foreach (var part in snake)
            {
                gameBoard.Children.Remove(part);
            }
            snake.Clear();

            Rectangle snakePart;
            for (var i = 0; i < SNAKE_INITIAL_SIZE; i++)
            {
                snakePart = CreateSnakePart();

                var top = CANVAS_HEIGHT / 2 + ITEM_SIZE * i;
                var left = CANVAS_WIDTH / 2;
                Canvas.SetTop(snakePart, top);
                Canvas.SetLeft(snakePart, left);

                gameBoard.Children.Add(snakePart);
                snake.AddLast(snakePart);
                usedCoordinates.Add((top, left));
            }
            
            snake.First.Value.Fill = Brushes.DarkGreen;

            // Por defecto, la serpiente empieza en direccion UP
            CurrentHeading = 1;
        }

        private Rectangle CreateSnakePart()
        {
            Rectangle newPart = new Rectangle
            {
                Height = ITEM_SIZE,
                Width = ITEM_SIZE,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = Brushes.LimeGreen
            };
            Canvas.SetZIndex(newPart, 2);

            return newPart;
        }

        private void SetupTimer()
        {
            timer = new Timer(500);
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // En cada tick...
            Dispatcher.Invoke(
                new Action(() =>
                {
                    Rectangle head = snake.First.Value;
                 
                    // Se calculan las coordenadas donde se tiene que poner la cabeza de la serpiente
                    (double top, double left) = CalculateNewCoordinates(head);

                    // Se comprueba si las nuevas coordenadas incumplen las reglas del juego:
                    //      - Salen fuera del tablero
                    //      - Situan la cabeza en el mismo sitio que otra parte del cuerpo
                    if (AreInvalidCoordinates(top, left) || IsGoingToEatHimself(top, left))
                    {
                        GameOver();
                        return;
                    }

                    // Si la serpiente tiene que crecer porque ha comido una fruta se crea una nueva parte de la serpiente,
                    // si no se quita del final la ultima parte para añadirla al principio
                    Rectangle snakePart;
                    if (partsToGrow == 0)
                    {
                        snakePart = snake.Last.Value;
                        snake.RemoveLast();
                        usedCoordinates.Remove((Canvas.GetTop(snakePart), Canvas.GetLeft(snakePart)));
                    } else
                    {
                        snakePart = CreateSnakePart();
                        gameBoard.Children.Add(snakePart);
                        partsToGrow--;
                    }
                        
                    head.Fill = Brushes.LimeGreen;

                    Canvas.SetTop(snakePart, top);
                    Canvas.SetLeft(snakePart, left);
                    snakePart.Fill = Brushes.DarkGreen;
                    snake.AddFirst(snakePart);
                    usedCoordinates.Add((top, left));

                    // Si tras realizar el movimiento ha llegado a una fruta se la come
                    if (HasCollidedWithFood(snakePart))
                    {
                        EatFoodAndRespawn();
                        partsToGrow += FRUIT_VALUE;
                    }

                    alreadyChangedHeading = false;
                }),
                DispatcherPriority.Normal
            );
        }

        // Calcula nuevas coordenadas dependiendo de la dirección actual de la serpiente
        private (double, double) CalculateNewCoordinates(Rectangle head)
        {
            double top = Canvas.GetTop(head);
            double left = Canvas.GetLeft(head);

            switch (headings[CurrentHeading])
            {
                case Heading.LEFT:
                    left -= ITEM_SIZE;
                    break;
                case Heading.UP:
                    top -= ITEM_SIZE;
                    break;
                case Heading.RIGHT:
                    left += ITEM_SIZE;
                    break;
                case Heading.DOWN:
                    top += ITEM_SIZE;
                    break;
            }

            return (top, left);
        }

        private bool AreInvalidCoordinates(double top, double left)
        {
            return !coordinates.Contains((top, left));
        }

        // Comprueba si las coordenadas corresponden con las de otra parte de la serpiente.
        // En caso de que las nuevas coordenadas sean las de la cola estamos en un caso valido ya que la parte que es cola se convierte en cabeza
        private bool IsGoingToEatHimself(double top, double left)
        {
            Rectangle last = snake.Last.Value;
            if (Canvas.GetTop(last) == top && Canvas.GetLeft(last) == left)
                return false;

            return usedCoordinates.Contains((top, left));
        }

        // Comprueba si las coordenadas de la cabeza y la fruta coinciden
        private bool HasCollidedWithFood(Rectangle head)
        {
            return Canvas.GetTop(head) == Canvas.GetTop(fruit) && Canvas.GetLeft(head) == Canvas.GetLeft(fruit);
        }

        // Come la fruta y reproduce un sonido acorde
        private void EatFoodAndRespawn()
        {
            eatFruit.Play();
            gameBoard.Children.Remove(fruit);
            IncrementScore();
            SpawnFood();
        }

        private void SpawnFood()
        {
            // Se elimina la fruta generada en el ultimo juego
            if (fruit != null)
            {
                gameBoard.Children.Remove(fruit);
            }

            // Obtiene las coordenadas que estan libres y selecciona una al azar donde generar el fruto
            var unusedCoordinates = coordinates.Except(usedCoordinates);
            (double top, double left) = unusedCoordinates.ElementAt(random.Next(unusedCoordinates.Count()));

            fruit = new Ellipse
            {
                Height = ITEM_SIZE,
                Width = ITEM_SIZE,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = Brushes.Red,
            };

            Canvas.SetTop(fruit, top);
            Canvas.SetLeft(fruit, left);
            Canvas.SetZIndex(fruit, 1);

            gameBoard.Children.Add(fruit);
        }

        private void SetupSounds()
        {
            startAndPause = new SoundPlayer(Snake.Resources.start_pause);
            eatFruit = new SoundPlayer(Snake.Resources.bite);          
        }

        private void SetupOverlay()
        {
            overlay = new Rectangle
            {
                Height = CANVAS_HEIGHT,
                Width = CANVAS_WIDTH,
                Fill = Brushes.LightGray,
                Opacity = 0.8
            };

            Canvas.SetTop(overlay, 0);
            Canvas.SetLeft(overlay, 0);
            Canvas.SetZIndex(overlay, 3);

            gameBoard.Children.Add(overlay);
        }

        private void SetupInfoUI()
        {
            maxScore = GetMaxScore();
            maxScoreLabel.Content = String.Format(RECORD_TEXT, maxScore);
            actionLabel.Content = NEW_GAME_TEXT;
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (isGameOngoing && !isGamePaused)
                        CurrentHeading--;
                    break;
                case Key.Right:
                    if (isGameOngoing && !isGamePaused)
                        CurrentHeading++;
                    break;
                case Key.Enter:
                    if (!isGameOngoing || isGamePaused)
                        StartGame();
                    break;
                case Key.Escape:
                    if (isGameOngoing && !isGamePaused)
                        PauseGame();
                    break;
            }
        }

        private void StartGame()
        {
            
            if (!isGameOngoing)
            {
                usedCoordinates.Clear();
                SetupSnake();
                SetupTimer();
                SpawnFood();
                score = 0;
                level = 1;
                UpdateScoreAndLevelLabels();
                isGameOngoing = true;
            }         

            startAndPause.Play();

            actionLabel.Content = PAUSE_GAME_TEXT;

            gameBoard.Children.Remove(overlay);
            timer.Start();
            isGamePaused = false;
        }

        // Incrementa la puntuacion del juego:
        //      - Cada POINTS_PER_LEVEL se aumenta de nivel
        //      - Cada nivel, el intervalo del tick disminuye 50ms
        //      - Cada dos niveles, se incrementa el valor de aumento que proporcionan las frutas
        private void IncrementScore()
        {
            score += POINTS_PER_FRUIT;
            if (score % POINTS_PER_LEVEL == 0)
            {
                level++;
                FRUIT_VALUE = level % 2 != 0 ? FRUIT_VALUE++ : FRUIT_VALUE;
                if (timer.Interval > 50)
                    timer.Interval -= SPEEDUP;
            }
            UpdateScoreAndLevelLabels();
        }

        private void UpdateScoreAndLevelLabels()
        {
            scoreLabel.Content = String.Format(SCORE_TEXT, score);
            levelLabel.Content = String.Format(LEVEL_TEXT, level);
        }

        private void PauseGame()
        {
            startAndPause.Play();
            gameBoard.Children.Add(overlay);
            actionLabel.Content = RESUME_GAME_TEXT;
            timer.Stop();
            isGamePaused = true;
        }

        private void GameOver()
        {
            new SoundPlayer(Snake.Resources.die).Play();
            timer.Stop();
            gameBoard.Children.Add(overlay);

            actionLabel.Content = NEW_GAME_TEXT;

            if (score > maxScore)
            {
                maxScoreLabel.Content = String.Format(RECORD_TEXT, score);
                SaveMaxScore(score);
            }
                
            isGameOngoing = false;
        }

        // Metodos de persistencia de puntuacion maxima
        private int GetMaxScore()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(int));
            int persistedScore;
            try
            {
                using (Stream reader = new FileStream(Directory.GetCurrentDirectory() + "\\maxScore.xml", FileMode.Open, FileAccess.Read))
                {
                    persistedScore = (int)serializer.Deserialize(reader);
                }
            } catch (FileNotFoundException)
            {
                persistedScore = 0;
            }
            
            return persistedScore;
        }

        private void SaveMaxScore(int scoreToSave)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(int));
            using (Stream writer = new FileStream(Directory.GetCurrentDirectory() + "\\maxScore.xml", FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(writer, scoreToSave);
            }
        }

        // UDPmote control
        private void Udpmote_UdpmoteChanged(UdpmoteState state)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action<UdpmoteState>((UdpmoteState s) =>
                {
                    if (s.ButtonState.A && (!isGameOngoing || isGamePaused))
                    {
                        StartGame();
                    }
                    else if (s.ButtonState.B && (isGameOngoing && !isGamePaused))
                    {
                        PauseGame();
                    }

                    double x = s.AccelState.X;
                    if (x > 1.0)
                        CurrentHeading--;
                    else if (x < -1.0)
                        CurrentHeading++;

                }), state);
        }
    }
}
