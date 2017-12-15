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
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace LogViewerExperiement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void ShowOpenFileDialog(Object sender, RoutedEventArgs args)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Filter = "Log files (*.log)|*.log";

            if(fileDialog.ShowDialog() == true)
            {
                OpenFile(fileDialog.FileName);
            }
        }
        async void OpenFile(string Filename)
        {
            {
                var stream = new StreamReader(Filename);

                var Log = new List<TextBlock>();
                //DateTimeFilter filter = new DateTimeFilter 

                //const string UE4_TIMESTAMP= @"^\[(?<year>\d+)\.(?<month>\d+)\.(?<day>\d+)-(?<hour>\d+)\.(?<minute>\d+)\.(?<second>\d+)\:(?<milisecond>\d+)\]";
                const string UE3_TIMESTAMP = @"^\[(\d+)\.(\d+)\:(?<year>\d+)\.(?<month>\d+)\.(?<day>\d+)-(?<hour>\d+)\.(?<minute>\d+)\.(?<second>\d+)]";
                HighlightFilter filter = new HighlightFilter(UE3_TIMESTAMP, new Run() { FontWeight = FontWeights.Bold });

                string line;
                while ((line = await stream.ReadLineAsync()) != null)
                {
                    TextBlock tb = filter.Process(line);
                    Log.Add(tb);
                }
                LogText.ItemsSource = Log;
            }

        }

        private void Select(TextBlock item)
        {
            LogText.ScrollIntoView(item);
            LogText.SelectedItem = item;
        }
        private void Deselect()
        {
            RemoveHighlight(LogText.SelectedItem as TextBlock, Brushes.Yellow);
            LogText.SelectedItem = null;
        }

        private void HighlightText(TextBlock item, string text, Brush brush)
        {
            foreach (Run line in item.Inlines)
            {
                int index = line.Text.IndexOf(SearchText.Text);
                if (index != -1)
                {
                    var newItem = new Run(line.Text.Substring(0, index));
                    item.Inlines.InsertBefore(line, newItem);
                    newItem = new Run(SearchText.Text) { Background = brush };
                    item.Inlines.InsertBefore(line, newItem);
                    newItem = new Run(line.Text.Substring(index + SearchText.Text.Length));
                    item.Inlines.InsertBefore(line, newItem);
                    item.Inlines.Remove(line);
                    break;
                }
            }
        }

        private void RemoveHighlight(TextBlock item, Brush brush)
        {
            Run PreviousLine = null;
            for(int i = 0; i < item.Inlines.Count; i++)
            {
                Run line = item.Inlines.ElementAt(i) as Run;
                if (line.Background == brush)
                {
                    string text = string.Empty;
                    if(PreviousLine != null)
                    {
                        text = PreviousLine.Text;
                        item.Inlines.Remove(PreviousLine);
                    }
                    text += line.Text;
                    if (i + 1 < item.Inlines.Count)
                    {
                        Run next = item.Inlines.ElementAt(i + 1) as Run;
                        text += next.Text;
                        item.Inlines.Remove(next);
                    }
                    item.Inlines.InsertBefore(line, new Run(text));
                    item.Inlines.Remove(line);
                    break;
                }
                PreviousLine = line;
            }
        }


        private void FindClicked(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock item in LogText.ItemsSource)
            {
                if(item == LogText.SelectedItem)
                {
                    Deselect();
                    continue;
                }
                if(LogText.SelectedItem != null)
                {
                    continue;
                }

                //if (item.Text.ToString().Contains(SearchText.Text))
                {
                    foreach(Run r in item.Inlines)
                    {
                        int index = r.Text.IndexOf(SearchText.Text);
                        if(index != -1)
                        {
                            HighlightText(item, SearchText.Text, Brushes.Yellow);
                            Select(item);
                            break;
                        }
                    }
                }

            }
        }

        private void FindPrevious(object sender, RoutedEventArgs e)
        {
            int i;
            if (LogText.SelectedItem != null)
            {
                i = LogText.SelectedIndex - 1;
                Deselect();
            }
            else
            {
                i = LogText.Items.Count - 1;
            }
            for (; i >= 0 ; --i)
            {
                TextBlock item = LogText.Items.GetItemAt(i) as TextBlock;
                foreach (Run r in item.Inlines)
                {
                    int index = r.Text.IndexOf(SearchText.Text);
                    if (index != -1)
                    {
                        HighlightText(item, SearchText.Text, Brushes.Yellow);
                        Select(item);
                        return;
                    }
                }
            }
        }

        private void NotImplemented(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature Not Implemented!");
        }
    }
}
