using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WPF_RunningButton
{
    public partial class MainWindow : Window
    {
        private Random rand = new Random();
        private int attempts = 0;
        private int successes = 0;
        private bool isMoving = false;

        public MainWindow()
        {
            InitializeComponent();

            // Устанавливаем обработчик изменения скорости
            speedSlider.ValueChanged += (s, e) => speedText.Text = ((int)speedSlider.Value).ToString();

            // Устанавливаем начальную позицию кнопки
            Loaded += (s, e) => SetButtonPosition();
            gameCanvas.SizeChanged += (s, e) => SetButtonPosition();
        }

        private void SetButtonPosition()
        {
            if (gameCanvas.ActualWidth == 0) return;

            double x = (gameCanvas.ActualWidth - runButton.Width) / 2;
            double y = (gameCanvas.ActualHeight - runButton.Height) / 2;

            Canvas.SetLeft(runButton, x);
            Canvas.SetTop(runButton, y);
        }

        private void runButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isMoving) return;

            isMoving = true;
            attempts++;
            UpdateStats();

            // Получаем позицию мыши
            Point mouse = e.GetPosition(gameCanvas);
            double left = Canvas.GetLeft(runButton);
            double top = Canvas.GetTop(runButton);

            // Вычисляем направление убегания
            double dx = left - mouse.X;
            double dy = top - mouse.Y;

            if (Math.Abs(dx) < 1 && Math.Abs(dy) < 1)
            {
                dx = rand.Next(-50, 50);
                dy = rand.Next(-50, 50);
            }

            // Нормализуем вектор
            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len > 0)
            {
                dx = dx / len;
                dy = dy / len;
            }

            // Расстояние убегания
            double distance = 100 * (speedSlider.Value / 5);

            // Новая позиция
            double newX = left + dx * distance;
            double newY = top + dy * distance;

            // Проверка границ
            double maxX = gameCanvas.ActualWidth - runButton.Width;
            double maxY = gameCanvas.ActualHeight - runButton.Height;

            if (newX < 0) newX = 0;
            if (newX > maxX) newX = maxX;
            if (newY < 0) newY = 0;
            if (newY > maxY) newY = maxY;

            // Анимация движения
            double duration = 80 / speedSlider.Value;
            DoubleAnimation animX = new DoubleAnimation(left, newX, TimeSpan.FromMilliseconds(duration));
            DoubleAnimation animY = new DoubleAnimation(top, newY, TimeSpan.FromMilliseconds(duration));

            animX.Completed += (s, e) => isMoving = false;

            runButton.BeginAnimation(Canvas.LeftProperty, animX);
            runButton.BeginAnimation(Canvas.TopProperty, animY);
        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            successes++;
            UpdateStats();

            // Эффект при клике
            runButton.Content = "Поймал!";
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += (s, args) => { runButton.Content = "Нажми меня"; timer.Stop(); };
            timer.Start();

            // Возвращаем кнопку в центр
            SetButtonPosition();
        }

        private void UpdateStats()
        {
            attemptsText.Text = $"Попыток: {attempts}";
            successText.Text = $"Успехов: {successes}";
        }
    }
}