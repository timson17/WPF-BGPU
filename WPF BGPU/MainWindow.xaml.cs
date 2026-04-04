using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace WPF_BGPU
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Подписываемся на событие загрузки окна
            this.Loaded += MainWindow_Loaded;
        }

        // Событие, которое срабатывает после полной загрузки окна
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCloseButtonState();
        }

        // Обновление состояния кнопки "Закрыть"
        private void UpdateCloseButtonState()
        {
            bool hasText = !string.IsNullOrWhiteSpace(TextBox1?.Text) ||
                          !string.IsNullOrWhiteSpace(TextBox2?.Text);

            if (hasText)
            {
                // Если есть текст - кнопка недоступна
                CloseButton.Style = (Style)FindResource("CloseButtonDisabledStyle");
            }
            else
            {
                // Если нет текста - кнопка доступна
                CloseButton.Style = (Style)FindResource("CloseButtonActiveStyle");
            }
        }

        // Обработчик изменения текста
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateCloseButtonState();
        }

        // Кнопка "Открыть" - загрузка текста из файла
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            openFileDialog.Title = "Выберите текстовый файл";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string content = File.ReadAllText(openFileDialog.FileName);

                    // Разделяем текст между двумя полями
                    int halfLength = content.Length / 2;
                    if (halfLength > 0)
                    {
                        TextBox1.Text = content.Substring(0, halfLength);
                        TextBox2.Text = content.Substring(halfLength);
                    }
                    else
                    {
                        TextBox1.Text = content;
                        TextBox2.Text = "";
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии файла: {ex.Message}",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
        }

        // Кнопка "Очистить"
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            TextBox1.Clear();
            TextBox2.Clear();
        }

        // Кнопка "Закрыть"
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что кнопка активна (в полях нет текста)
            if (string.IsNullOrWhiteSpace(TextBox1?.Text) &&
                string.IsNullOrWhiteSpace(TextBox2?.Text))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Невозможно закрыть приложение, пока в текстовых полях есть содержимое!",
                              "Предупреждение",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
            }
        }

        // Изменение стиля текстовых полей при выборе в ComboBox
        private void StyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StyleComboBox.SelectedItem == null) return;

            ComboBoxItem selectedItem = (ComboBoxItem)StyleComboBox.SelectedItem;

            switch (selectedItem.Content.ToString())
            {
                case "Стиль 1 (Arial, 12, Черный)":
                    ApplyTextStyle(new FontFamily("Arial"), 12, Brushes.Black);
                    break;
                case "Стиль 2 (Times New Roman, 14, Синий)":
                    ApplyTextStyle(new FontFamily("Times New Roman"), 14, Brushes.Blue);
                    break;
                case "Стиль 3 (Courier New, 11, Красный)":
                    ApplyTextStyle(new FontFamily("Courier New"), 11, Brushes.Red);
                    break;
                case "Стиль 4 (Verdana, 16, Темно-зеленый)":
                    ApplyTextStyle(new FontFamily("Verdana"), 16, Brushes.DarkGreen);
                    break;
                case "Стиль 5 (Comic Sans MS, 13, Фиолетовый)":
                    ApplyTextStyle(new FontFamily("Comic Sans MS"), 13, Brushes.Purple);
                    break;
            }
        }

        // Применение стиля к обоим текстовым полям
        private void ApplyTextStyle(FontFamily fontFamily, double fontSize, Brush foreground)
        {
            if (TextBox1 != null && TextBox2 != null)
            {
                TextBox1.FontFamily = fontFamily;
                TextBox1.FontSize = fontSize;
                TextBox1.Foreground = foreground;

                TextBox2.FontFamily = fontFamily;
                TextBox2.FontSize = fontSize;
                TextBox2.Foreground = foreground;
            }
        }
    }
}