using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using Microsoft.Win32;

namespace WPF_BGPU
{
    public partial class MultiEdit : Window
    {
        private string currentFilePath;

        public MultiEdit()
        {
            InitializeComponent();

            // Подписка на события изменения текста
            TextEditor1.TextChanged += TextEditor_TextChanged;
            TextEditor2.TextChanged += TextEditor_TextChanged;
            TextEditor3.TextChanged += TextEditor_TextChanged;
        }

        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            TextBox activeEditor = GetActiveTextBox();
            if (activeEditor != null)
            {
                string text = activeEditor.Text;

                // Подсчет символов
                int charCount = text.Length;
                CharCountText.Text = $"Символов: {charCount}";

                // Подсчет слов
                int wordCount = string.IsNullOrWhiteSpace(text) ? 0 :
                    Regex.Matches(text.Trim(), @"\S+").Count;
                WordCountText.Text = $"Слов: {wordCount}";
            }
        }

        private TextBox GetActiveTextBox()
        {
            // Определяем активную вкладку и возвращаем соответствующий TextBox
            var tabControl = (TabControl)Grid.GetRow(1) != null ?
                FindName("TextEditor1") as TabControl : null;

            // Простой способ - проверяем видимость/фокус
            if (TextEditor1.IsFocused || TextEditor1.IsVisible)
                return TextEditor1;
            else if (TextEditor2.IsFocused || TextEditor2.IsVisible)
                return TextEditor2;
            else
                return TextEditor3;
        }

        private void ShowAnimatedMessage(string message, bool isError = false)
        {
            MessageText.Text = message;

            // Меняем цвет фона в зависимости от типа сообщения
            if (isError)
                FloatingMessage.Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Colors.Red);
            else
                FloatingMessage.Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Colors.Green);

            // Анимация появления
            DoubleAnimation fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3)
            };

            DoubleAnimation slideIn = new DoubleAnimation
            {
                From = -50,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new QuadraticEase()
            };

            FloatingMessage.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            FloatingMessage.BeginAnimation(UIElement.RenderTransformProperty,
                new TranslateTransform());

            var transform = new TranslateTransform();
            FloatingMessage.RenderTransform = transform;
            transform.BeginAnimation(TranslateTransform.YProperty, slideIn);

            // Анимация исчезновения через 2 секунды
            DoubleAnimation fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.3),
                BeginTime = TimeSpan.FromSeconds(2)
            };

            fadeOut.Completed += (s, e) =>
            {
                FloatingMessage.Visibility = Visibility.Collapsed;
            };

            FloatingMessage.Visibility = Visibility.Visible;
            FloatingMessage.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        private void AnimateStatusChange(string message)
        {
            StatusText.Text = message;

            // Анимация мигания статуса
            DoubleAnimation blink = new DoubleAnimation
            {
                From = 1,
                To = 0.3,
                Duration = TimeSpan.FromSeconds(0.2),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(2)
            };

            StatusText.BeginAnimation(UIElement.OpacityProperty, blink);
        }

        // Методы для работы с документами
        private void NewDocument_Click(object sender, RoutedEventArgs e)
        {
            TextBox activeEditor = GetActiveTextBox();
            if (activeEditor != null)
            {
                if (!string.IsNullOrWhiteSpace(activeEditor.Text))
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Создать новый документ? Несохраненные изменения будут потеряны.",
                        "Новый документ",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        activeEditor.Clear();
                        currentFilePath = null;
                        ShowAnimatedMessage("Создан новый документ");
                        AnimateStatusChange("Новый документ создан");
                    }
                }
                else
                {
                    activeEditor.Clear();
                    ShowAnimatedMessage("Создан новый документ");
                    AnimateStatusChange("Новый документ создан");
                }
            }
        }

        private void OpenDocument_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                Title = "Открыть документ"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string content = File.ReadAllText(openFileDialog.FileName);
                    TextBox activeEditor = GetActiveTextBox();
                    activeEditor.Text = content;
                    currentFilePath = openFileDialog.FileName;

                    ShowAnimatedMessage($"Документ загружен: {Path.GetFileName(currentFilePath)}");
                    AnimateStatusChange($"Открыт файл: {Path.GetFileName(currentFilePath)}");
                }
                catch (Exception ex)
                {
                    ShowAnimatedMessage($"Ошибка загрузки: {ex.Message}", true);
                }
            }
        }

        private void SaveDocument_Click(object sender, RoutedEventArgs e)
        {
            TextBox activeEditor = GetActiveTextBox();

            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                    Title = "Сохранить документ",
                    DefaultExt = "txt",
                    FileName = "document.txt"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    currentFilePath = saveFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            try
            {
                File.WriteAllText(currentFilePath, activeEditor.Text);
                ShowAnimatedMessage($"Документ сохранен: {Path.GetFileName(currentFilePath)}");
                AnimateStatusChange($"Сохранен файл: {Path.GetFileName(currentFilePath)}");
            }
            catch (Exception ex)
            {
                ShowAnimatedMessage($"Ошибка сохранения: {ex.Message}", true);
            }
        }

        // Операции редактирования
        private void CopyText_Click(object sender, RoutedEventArgs e)
        {
            TextBox activeEditor = GetActiveTextBox();
            if (!string.IsNullOrEmpty(activeEditor.SelectedText))
            {
                Clipboard.SetText(activeEditor.SelectedText);
                ShowAnimatedMessage("Текст скопирован");
                AnimateStatusChange("Текст скопирован в буфер обмена");
            }
            else
            {
                ShowAnimatedMessage("Нет выделенного текста", true);
            }
        }

        private void CutText_Click(object sender, RoutedEventArgs e)
        {
            TextBox activeEditor = GetActiveTextBox();
            if (!string.IsNullOrEmpty(activeEditor.SelectedText))
            {
                Clipboard.SetText(activeEditor.SelectedText);
                activeEditor.SelectedText = "";
                ShowAnimatedMessage("Текст вырезан");
                AnimateStatusChange("Текст вырезан в буфер обмена");
            }
            else
            {
                ShowAnimatedMessage("Нет выделенного текста", true);
            }
        }

        private void PasteText_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                TextBox activeEditor = GetActiveTextBox();
                activeEditor.SelectedText = Clipboard.GetText();
                ShowAnimatedMessage("Текст вставлен");
                AnimateStatusChange("Текст вставлен из буфера обмена");
            }
            else
            {
                ShowAnimatedMessage("Буфер обмена пуст", true);
            }
        }

        private void ClearText_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Очистить весь текст?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                TextBox activeEditor = GetActiveTextBox();
                activeEditor.Clear();
                ShowAnimatedMessage("Текст очищен");
                AnimateStatusChange("Документ очищен");
            }
        }
    }
}