using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF_BGPU
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrushColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyInkCanvas == null) return;

            var comboBox = sender as ComboBox;
            if (comboBox?.SelectedItem is ComboBoxItem item)
            {
                switch (item.Content.ToString())
                {
                    case "Красный":
                        MyInkCanvas.DefaultDrawingAttributes.Color = Colors.Red;
                        break;
                    case "Синий":
                        MyInkCanvas.DefaultDrawingAttributes.Color = Colors.Blue;
                        break;
                    case "Зелёный":
                        MyInkCanvas.DefaultDrawingAttributes.Color = Colors.Green;
                        break;
                    case "Жёлтый":
                        MyInkCanvas.DefaultDrawingAttributes.Color = Colors.Yellow;
                        break;
                }
                StatusText.Text = $"Цвет кисти: {item.Content}";
            }
        }

        private void BrushSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (BrushSizeText != null && MyInkCanvas != null)
            {
                int size = (int)BrushSizeSlider.Value;
                BrushSizeText.Text = size.ToString();
                StatusText.Text = $"Размер кисти: {size}";

                MyInkCanvas.DefaultDrawingAttributes.Height = size;
                MyInkCanvas.DefaultDrawingAttributes.Width = size;
            }
        }


        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Автор: Каримов Тимур\nВерсия: 1.0",
                            "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AboutMenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Открыто";
        }

        private void AboutMenuItem_Click_Save(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Сохранено";
        }

        private void AboutMenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Выход";
            this.Close();
        }

        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyInkCanvas == null) return;

            var comboBox = sender as ComboBox;
            if (comboBox?.SelectedItem is ComboBoxItem item)
            {
                switch (item.Content.ToString())
                {
                    case "Рисование":
                        MyInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                        break;
                    case "Редактирование":
                        MyInkCanvas.EditingMode = InkCanvasEditingMode.Select;
                        break;
                    case "Удаление":
                        MyInkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
                        break;
                }
                StatusText.Text = $"Режим: {item.Content}";
            }
        }


    }
}
