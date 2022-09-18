using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Wpf__Task3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int CurrentFontSize = 11;
        public const int DefaultFontSize = 11;
        bool AutoSave = false;
        bool FileSaved = false;
        public MainWindow()
        {
            InitializeComponent();
            Zoom_ComboBox.SelectedIndex = 4;
            Fonts_ComboBox.SelectedIndex = 0;
            FontSize_TextBox.Text = DefaultFontSize.ToString();

            Brush brush = Brushes.Black;
            Color colorFromBrush;
            colorFromBrush = (brush as SolidColorBrush).Color;


            ColorPicker.SelectedColor = colorFromBrush;

            var families = Fonts.SystemFontFamilies;

            foreach (var fontfamily in families)
            {
                Fonts_ComboBox.Items.Add(fontfamily);
            }
        }

        private void Doc_name_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Doc_name.Text == String.Empty)
                Doc_name.Text = "Untitled Document";
        }

        private void Undo_Btn_Click(object sender, RoutedEventArgs e)
        {
            Writings.Undo();
        }

        private void Redo_Btn_Click(object sender, RoutedEventArgs e)
        {
            Writings.Redo();
        }

        private void Print_Btn_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(Paper, "My First Print Job");
            }
        }

        private void Zoom_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var scaler = Paper.LayoutTransform as ScaleTransform;
            var selectedItem = (Zoom_ComboBox.SelectedItem as ComboBoxItem).Content.ToString();

            dynamic to;

            if (selectedItem == "Fit")
            {
                to = 2.10;
            }
            else
            {
                selectedItem = selectedItem.Remove(selectedItem.Length - 1, 1);
                to = double.Parse(selectedItem) / 100;
            }

            if (scaler == null)
            {
                scaler = new ScaleTransform(1.0, 1.0);
                Paper.LayoutTransform = scaler;
            }

            DoubleAnimation animator = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(600)),
            };

            animator.To = to;
            scaler.BeginAnimation(ScaleTransform.ScaleXProperty, animator);
            scaler.BeginAnimation(ScaleTransform.ScaleYProperty, animator);
        }

        private void Fonts_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedFont = Fonts_ComboBox.SelectedItem;
            Writings.FontFamily = selectedFont as System.Windows.Media.FontFamily;
        }

        private void DecreaseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentFontSize > 1)
            {
                CurrentFontSize--;
                FontSize_TextBox.Text = CurrentFontSize.ToString();
            }
        }

        private void IncreaseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentFontSize < 99)
            {
                CurrentFontSize++;
                FontSize_TextBox.Text = CurrentFontSize.ToString();
            }
        }

        private void FontSize_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FontSize_TextBox.Text != String.Empty)
            {
                bool converted = int.TryParse(FontSize_TextBox.Text, out CurrentFontSize);
                if (converted)
                    Writings.FontSize = CurrentFontSize;
            }
        }

        public static bool IsTextNumeric(string str)
        {
            Regex reg = new Regex("[^0-9]");
            return reg.IsMatch(str);
        }

        private void FontSize_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
        }

        private void Bold_TBtn_Checked(object sender, RoutedEventArgs e)
        {
            Writings.FontWeight = FontWeights.Bold;
        }

        private void Bold_TBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            Writings.FontWeight = FontWeights.Normal;
        }

        private void Italic_TBtn_Checked(object sender, RoutedEventArgs e)
        {
            Writings.FontStyle = FontStyles.Italic;
        }

        private void Italic_TBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            Writings.FontStyle = FontStyles.Normal;
        }
        private void Underline_TBtn_Checked(object sender, RoutedEventArgs e)
        {
            Writings.TextDecorations = TextDecorations.Underline;
        }

        private void Underline_TBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            Writings.TextDecorations = null;
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            var brush = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString(ColorPicker.SelectedColorText);

            Writings.Foreground = brush;
        }

        private void LeftAlign_Btn_Click(object sender, RoutedEventArgs e)
        {
            Writings.TextAlignment = TextAlignment.Left;
        }

        private void CenterAlign_Btn_Click(object sender, RoutedEventArgs e)
        {
            Writings.TextAlignment = TextAlignment.Center;
        }

        private void RightAlign_Btn_Click(object sender, RoutedEventArgs e)
        {
            Writings.TextAlignment = TextAlignment.Right;
        }

        private void AutoSave_Cb_Checked(object sender, RoutedEventArgs e)
        {
            if (!FileSaved)
            {
                Save_Button_Click(sender, e);
                FileSaved = true;
            }
            AutoSave = true;
        }

        private void AutoSave_Cb_Unchecked(object sender, RoutedEventArgs e)
        {
            AutoSave = false;
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!FileSaved)
            {
                string content = Writings.Text;

                var openFileDialog = new OpenFileDialog();

                if (openFileDialog.ShowDialog() == true)
                {
                    FileName = openFileDialog.FileName;
                    StreamWriter writer = new StreamWriter(File.OpenWrite(FileName));

                    writer.Write(content);
                    writer.Close();

                    FileSaved = true;
                    FileInfo fileInfo = new FileInfo(FileName);
                    Doc_name.Text = fileInfo.Name;
                }
            }
        }


        public string FileName { get; set; }
        private void Writings_KeyDown(object sender, KeyEventArgs e)
        {
            if (AutoSave)
            {
                var lastLetter = e.Key.ToString();
                if (e.IsDown)
                {
                    lastLetter = lastLetter.ToLower();
                }

                string content = Writings.Text;

                if (char.IsDigit(lastLetter, 0) || char.IsLetter(lastLetter, 0) || e.Key == Key.Space || e.Key == Key.Enter)
                {
                    content += lastLetter;
                }

                StreamWriter writer = new StreamWriter(File.OpenWrite(FileName));

                writer.Write(content);

                writer.Close();
            }
        }
    }
}