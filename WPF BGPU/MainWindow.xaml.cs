using System.Windows;
using System.Windows.Controls;

namespace WPF_BGPU
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                MessageBox.Show($"Вы нажали кнопку: {btn.Content}",
                                "Событие Click",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }
    }
}
