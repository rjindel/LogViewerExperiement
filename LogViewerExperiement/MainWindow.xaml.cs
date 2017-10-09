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
                //MessageBox.Show("NotImplemented! Actually open selected file: " + fileDialog.FileName);
                var stream = new StreamReader(Filename);

                var Log = new List<TextBlock>();
                DateTimeFilter filter = new DateTimeFilter();
                string line;
                while ((line = await stream.ReadLineAsync()) != null)
                {
                    //TextBlock tb = new TextBlock();
                    //tb.Inlines.Add(line);
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
                    item.Select(SearchText.Text);
                    LogText.ScrollIntoView(item);
                    LogText.SelectedItem = item;
                    break;
                }

            }
            //if(LogText.Text.Length > SearchText.Text.Length)
            //LogText.Items.
            //{
            //    int Index = LogText.Text.IndexOf(SearchText.Text);
            //    double ScrollToLocation = (double)Index ;
            //    LogText.ScrollToVerticalOffset(ScrollToLocation);
            //}
        }
    }
}
