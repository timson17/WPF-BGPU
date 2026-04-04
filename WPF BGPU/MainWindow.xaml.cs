using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace WPF_BGPU
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer animationTimer;
        private int currentState = 0;
        private double currentValue = 0;
        private bool isAnimating = false;

        private Color[] centerColors;
        private Color[] edgeColors;
        private string[] stateNames;
        private double[] centerPoints;
        private double[] radiuses;

        public MainWindow()
        {
            InitializeComponent();

            centerColors = new Color[]
            {
                Color.FromRgb(0, 0, 139),    // Покой - темно-синий
                Color.FromRgb(255, 165, 0),  // Расширение - оранжевый
                Color.FromRgb(255, 0, 0),    // Пик - красный
                Color.FromRgb(255, 165, 0),  // Сжатие - оранжевый
                Color.FromRgb(0, 0, 139)     // Минимум - темно-синий
            };

            edgeColors = new Color[]
            {
                Color.FromRgb(0, 0, 0),       // Покой - черный
                Color.FromRgb(139, 0, 0),     // Расширение - темно-красный
                Color.FromRgb(255, 255, 0),   // Пик - желтый
                Color.FromRgb(139, 0, 0),     // Сжатие - темно-красный
                Color.FromRgb(0, 0, 0)        // Минимум - черный
            };

            stateNames = new string[]
            {
                "Покой",
                "Расширение",
                "Пик",
                "Сжатие",
                "Минимум"
            };

            centerPoints = new double[] { 0.3, 0.4, 0.5, 0.4, 0.2 };
            radiuses = new double[] { 0.5, 0.6, 0.8, 0.6, 0.3 };

            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(50);
            animationTimer.Tick += AnimationTimer_Tick;

            this.Loaded += MainWindow_Loaded;

            ApplyGradient(centerColors[0], edgeColors[0], centerPoints[0], radiuses[0]);

            if (stateText != null)
            {
                stateText.Text = $"Состояние: {stateNames[0]}";
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (speedValue != null && speedSlider != null)
            {
                speedValue.Text = speedSlider.Value.ToString("F1");
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (!isAnimating) return;

            double speed = speedSlider.Value * 0.03;
            currentValue += speed;

            if (currentValue >= 1)
            {
                currentValue = 0;
                currentState++;

                if (currentState >= 5)
                {
                    currentState = 0;
                }

                if (stateText != null)
                {
                    stateText.Text = $"Состояние: {stateNames[currentState]}";
                }
            }

            int nextState = (currentState + 1) % 5;
            double t = EaseInOut(currentValue);

            Color centerColor = InterpolateColor(centerColors[currentState], centerColors[nextState], t);
            Color edgeColor = InterpolateColor(edgeColors[currentState], edgeColors[nextState], t);
            double centerPoint = centerPoints[currentState] + (centerPoints[nextState] - centerPoints[currentState]) * t;
            double radius = radiuses[currentState] + (radiuses[nextState] - radiuses[currentState]) * t;

            ApplyGradient(centerColor, edgeColor, centerPoint, radius);
        }

        private double EaseInOut(double t)
        {
            return t < 0.5 ? 2 * t * t : 1 - Math.Pow(-2 * t + 2, 2) / 2;
        }

        private Color InterpolateColor(Color c1, Color c2, double t)
        {
            return Color.FromArgb(
                255,
                (byte)(c1.R + (c2.R - c1.R) * t),
                (byte)(c1.G + (c2.G - c1.G) * t),
                (byte)(c1.B + (c2.B - c1.B) * t)
            );
        }

        private void ApplyGradient(Color centerColor, Color edgeColor, double centerPoint, double radius)
        {
            if (pulsarEllipse == null) return;

            RadialGradientBrush gradient = new RadialGradientBrush();
            gradient.GradientOrigin = new Point(0.5, 0.5);
            gradient.Center = new Point(0.5, 0.5);
            gradient.RadiusX = radius;
            gradient.RadiusY = radius;

            gradient.GradientStops.Clear();
            gradient.GradientStops.Add(new GradientStop(centerColor, 0.0));
            gradient.GradientStops.Add(new GradientStop(centerColor, Math.Max(0, centerPoint - 0.05)));
            gradient.GradientStops.Add(new GradientStop(edgeColor, Math.Min(1, centerPoint + 0.05)));
            gradient.GradientStops.Add(new GradientStop(edgeColor, 1.0));

            pulsarEllipse.Fill = gradient;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            isAnimating = true;
            animationTimer.Start();
            if (startButton != null)
                startButton.Background = new SolidColorBrush(Color.FromRgb(0, 100, 0));
            if (stopButton != null)
                stopButton.Background = new SolidColorBrush(Color.FromRgb(231, 76, 60));
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            isAnimating = false;
            animationTimer.Stop();
            if (startButton != null)
                startButton.Background = new SolidColorBrush(Color.FromRgb(46, 204, 113));
            if (stopButton != null)
                stopButton.Background = new SolidColorBrush(Color.FromRgb(192, 57, 43));
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            isAnimating = false;
            animationTimer.Stop();
            currentState = 0;
            currentValue = 0;
            ApplyGradient(centerColors[0], edgeColors[0], centerPoints[0], radiuses[0]);
            if (stateText != null)
                stateText.Text = $"Состояние: {stateNames[0]}";
            if (startButton != null)
                startButton.Background = new SolidColorBrush(Color.FromRgb(46, 204, 113));
            if (stopButton != null)
                stopButton.Background = new SolidColorBrush(Color.FromRgb(231, 76, 60));
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (speedValue != null)
            {
                speedValue.Text = speedSlider.Value.ToString("F1");
            }
        }
    }
}