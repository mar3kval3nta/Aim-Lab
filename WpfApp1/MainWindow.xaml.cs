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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random random;
        private int score;
        private bool isGameRunning;
        private Label scoreLabel;
        public MainWindow()
        {
            InitializeComponent();
            random = new Random();
            score = 0;
            isGameRunning = false;
            scoreLabel = ScoreLabel;
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (isGameRunning)
                return;

            isGameRunning = true;
            StartButton.IsEnabled = false;
            ClearGameCanvas();
            score = 0;
            UpdateScoreLabel();

            // Start generating targets
            GenerateTarget();
        }

        private void GameCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isGameRunning)
                return;

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                var clickedTarget = e.Source as Ellipse;
                if (clickedTarget != null)
                {
                    GameCanvas.Children.Remove(clickedTarget);
                    score++;
                    UpdateScoreLabel();
                }
                else
                {
                    // Hráč se netrefil, odečíst skóre pouze pokud je větší než 0
                    if (score > 0)
                    {
                        score--;
                        UpdateScoreLabel();
                    }
                }
            }
        }



        private void GenerateTarget()
        {
            if (!isGameRunning)
                return;

            // Pokud je již na plátně nějaký cíl, nebudeme generovat další
            if (GameCanvas.Children.OfType<Ellipse>().Any())
                return;

            var target = new Ellipse
            {
                Width = 50,
                Height = 50,
                Fill = Brushes.Red
            };

            // Nastavit náhodnou pozici
            Canvas.SetLeft(target, random.Next(0, (int)GameCanvas.ActualWidth - 50));
            Canvas.SetTop(target, random.Next(0, (int)GameCanvas.ActualHeight - 50));

            // Přidat cíl do plátna
            GameCanvas.Children.Add(target);

            // Naplánovat generování dalšího cíle
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += (s, e) =>
            {
                GameCanvas.Children.Remove(target);
                GenerateTarget();
            };
            timer.Interval = TimeSpan.FromSeconds(random.Next(1, 2));
            timer.Start();
        }




        private void ClearGameCanvas()
        {
            List<UIElement> targetsToRemove = new List<UIElement>();
            foreach (UIElement element in GameCanvas.Children)
            {
                if (element is Ellipse)
                    targetsToRemove.Add(element);
            }

            foreach (UIElement element in targetsToRemove)
            {
                GameCanvas.Children.Remove(element);
            }
        }

        private void UpdateScoreLabel()
        {
            scoreLabel.Content = "Score: " + score.ToString();
        }
    }
}

