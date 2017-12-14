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
                DateTimeFilter filter = new DateTimeFilter();
                string line;
                while ((line = await stream.ReadLineAsync()) != null)
                {
                    TextBlock tb = filter.Process(line);
                    Log.Add(tb);
                }
                LogText.ItemsSource = Log;
            }

        }

        private void FindClicked(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock item in LogText.ItemsSource)
            {
                if (item.Text.ToString().Contains(SearchText.Text))
                {
                    foreach(Run r in item.Inlines)
                    {
                        int index = r.Text.IndexOf(SearchText.Text);
                        if(index != -1)
                        {
                            var newItem = new Run(r.Text.Substring(0, index));
                            item.Inlines.InsertAfter(r, newItem);
                            newItem = new Run(SearchText.Text) { Background = Brushes.Yellow };
                            item.Inlines.InsertAfter(r, newItem);
                            newItem = new Run(r.Text.Substring(index + SearchText.Text.Length));
                            item.Inlines.InsertAfter(r, newItem);
                            item.Inlines.Remove(r);
                            break;
                        }
                    }
                    LogText.ScrollIntoView(item);
                    LogText.SelectedItem = item;
                    break;
                }

            }
        }

        private void NotImplemented(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature Not Implemented!");
        }
    }
}
