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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.IO;

namespace LogViewerExperiement
{
    /// <summary>
    /// Interaction logic for CreateFilter.xaml
    /// </summary>
    public partial class CreateFilter : Window
    {
        public CreateFilter()
        {
            InitializeComponent();

            CurrentFilter.SelectionChanged += new SelectionChangedEventHandler( Combo_SelectionChanged );
        }

        private void LoadDefaultFilter(object sender, RoutedEventArgs e)
        {
            const string UE3_TIMESTAMP = @"^\[(\d+)\.(\d+)\:(?<year>\d+)\.(?<month>\d+)\.(?<day>\d+)-(?<hour>\d+)\.(?<minute>\d+)\.(?<second>\d+)]";

            FilterData data = new FilterData();
            data.m_Name = "UE3_TIMESTAMP";
            data.m_FilterPattern = UE3_TIMESTAMP;
            data.m_Style = new Run() { FontWeight = FontWeights.Bold };

            List<FilterData> items =  new List<FilterData>();
            items.Add(data);
            CurrentFilter.ItemsSource = items;
            CurrentFilter.SelectedIndex = 0;
        }

        private void SetCurrentFilter(FilterData data)
        {

            FilterName.Text = data.m_Name;
            SearchFilter.Text = data.m_FilterPattern;

            data.m_Style.Text = viewer.Text;
            viewer.Inlines.Clear();
            viewer.Inlines.Add(data.m_Style);
        }

        private void LoadFilter(object sender, RoutedEventArgs e)
        {
            StreamReader reader = new StreamReader("Filter.cfg");
            List<FilterData> items = new List<FilterData>();

            while (!reader.EndOfStream)
            {
                FilterData filter = new FilterData();
                filter.m_Name = reader.ReadLine();
                filter.m_FilterPattern = reader.ReadLine();
                string filterStyle = reader.ReadLine();
                filter.m_Style = XamlReader.Parse(filterStyle) as Run;
                items.Add(filter);
            }
            reader.Close();

            if (items.Count > 0)
            {
                CurrentFilter.ItemsSource = items;
                CurrentFilter.SelectedIndex = 0;
            }
        }

        private void SaveFilter(object sender, RoutedEventArgs e)
        {
            var style = viewer.Inlines.FirstInline;
            string filterStyle = XamlWriter.Save(style);

            StreamWriter writer = new StreamWriter("Filter.cfg");
            writer.WriteLine(FilterName.Text);

            writer.WriteLine(SearchFilter.Text);
            writer.WriteLine(filterStyle);
            writer.Close();
        }
        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                FilterData Selected = e.AddedItems[0] as FilterData;
                SetCurrentFilter(Selected);
            }
        }
    }
}
