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
using System.Reflection;

namespace LogViewerExperiement
{
    /// <summary>
    /// Interaction logic for CreateFilter.xaml
    /// </summary>
    public partial class CreateFilter : Window
    {
        const string DefaultFilterName = "Filter Name";
        const string DefaultSampleText = "Sample Text";
        List<FontStyle> fontStyles;
        List<FontWeight> fontWeights;
        List<TextBlock> brushes;
        List<FilterData> items;
        public CreateFilter()
        {
            InitializeComponent();
            InitDialog();

            CurrentFilter.SelectionChanged += Combo_FilterChanged;
            LoadFilter();
        }

        private void InitDialog()
        {
            //Need a better way. Reflection.
            fontStyles = new List<FontStyle>() { FontStyles.Italic, FontStyles.Normal, FontStyles.Oblique };
            FontStyleCombo.ItemsSource = fontStyles;
            FontStyleCombo.SelectedItem = FontStyles.Normal;

            fontWeights = new List<FontWeight>() {
                FontWeights.Black,
                FontWeights.Bold,
                FontWeights.DemiBold,
                FontWeights.ExtraBlack,
                FontWeights.ExtraBold,
                FontWeights.ExtraLight,
                FontWeights.Heavy,
                FontWeights.Light,
                FontWeights.Medium,
                FontWeights.Normal,
                FontWeights.Regular,
                FontWeights.SemiBold,
                FontWeights.Thin,
                FontWeights.UltraLight,
                FontWeights.UltraBlack,
                FontWeights.UltraBold,
            };
            FontWeightCombo.ItemsSource = fontWeights;
            FontWeightCombo.SelectedItem = FontWeights.Normal;

            //Try reflection for brushes (could be used for FontWeight + Style)
            brushes = new List<TextBlock>();

            Type brushesType = typeof(Brushes);
            var properties = brushesType.GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach (var prop in properties)
            {
                Brush brush = prop.GetValue(null, null) as Brush;
                Run backgroundRun = new Run(prop.Name) { Background = brush };
                TextBlock tb = new TextBlock(backgroundRun);

                brushes.Add(tb);
            }

            BackgroundCombo.ItemsSource = new List<TextBlock>(brushes);
            ForegroundCombo.ItemsSource = brushes;

            BackgroundCombo.SelectionChanged += Combo_ColourChanged;
            ForegroundCombo.SelectionChanged += Combo_ColourChanged;
            FontStyleCombo.SelectionChanged += Combo_FontStyleChanged;
            FontWeightCombo.SelectionChanged += Combo_FontWeightChanged;
        }

        private void LoadDefaultFilter(object sender, RoutedEventArgs e)
        {
            const string UE3_TIMESTAMP = @"^\[(\d+)\.(\d+)\:(?<year>\d+)\.(?<month>\d+)\.(?<day>\d+)-(?<hour>\d+)\.(?<minute>\d+)\.(?<second>\d+)]";

            FilterData data = new FilterData();
            data.m_Name = "UE3_TIMESTAMP";
            data.m_FilterPattern = UE3_TIMESTAMP;
            data.m_Style = new Run() { FontWeight = FontWeights.Bold };

            items =  new List<FilterData>();
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
            LoadFilter();
        }
        private void LoadFilter()
        {
            StreamReader reader = new StreamReader("Filter.cfg");
            items = new List<FilterData>();

            while (!reader.EndOfStream)
            {
                FilterData filter = new FilterData();
                filter.m_Name = reader.ReadLine();
                filter.m_FilterPattern = reader.ReadLine();
                string filterStyle = reader.ReadLine();
                try
                {
                    filter.m_Style = XamlReader.Parse(filterStyle) as Run;
                }
                catch(XamlParseException e)
                {
                    Console.Write("Error loading filters: {0} ", e.Message);
                }
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
            StreamWriter writer = new StreamWriter("Filter.cfg");

            foreach(FilterData data in CurrentFilter.Items)
            {
                var style = data.m_Style;
                string filterStyle = XamlWriter.Save(style);

                writer.WriteLine(data.m_Name);
                writer.WriteLine(data.m_FilterPattern);
                writer.WriteLine(filterStyle);
            }

            FilterData firstItem = new FilterData();
            if(CurrentFilter.Items.Count > 0)
            {
                firstItem = CurrentFilter.Items[0] as FilterData;
            }

            if (FilterName.Text != firstItem.m_Name)
            {
                var style = viewer.Inlines.FirstInline;
                string filterStyle = XamlWriter.Save(style);

                writer.WriteLine(FilterName.Text);
                writer.WriteLine(SearchFilter.Text);
                writer.WriteLine(filterStyle);
            }
            writer.Close();
        }
        void ResetFilter()
        {
            FilterName.Text = DefaultFilterName;
            SearchFilter.Text = SearchFilterLabel.Content.ToString();
            viewer.Inlines.Clear();
            var defaultRun = new Run(DefaultSampleText);
            viewer.Inlines.Add(defaultRun);

            //foreach(var brush in brushes)
            //{
            //    Run currentRun = brush.Inlines.FirstInline as Run;
            //    if((currentRun.Background as SolidColorBrush).Color == (defaultRun.Background as SolidColorBrush).Color)
            //    {
            //        defaultRun.Text = currentRun.Text;
            //    }
            //}

            var tb = new TextBlock(defaultRun);
            BackgroundCombo.SelectedItem = tb;
            ForegroundCombo.SelectedItem = tb;
        }
        private void DeleteFilter(object sender, RoutedEventArgs e)
        {
            var Filter = CurrentFilter.FindName(FilterName.Text) as FilterData;
            items.Remove(Filter);
            CurrentFilter.ItemsSource = items;

            ResetFilter();
        }
        private void AddFilter(object sender, RoutedEventArgs e)
        {
            FilterData data = new FilterData();
            data.m_Name = FilterName.Text;
            data.m_FilterPattern = SearchFilter.Text;
            data.m_Style = viewer.Inlines.FirstInline as Run;
            items.Add(data);

            ResetFilter();
        }
        private void Combo_FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                FilterData Selected = e.AddedItems[0] as FilterData;
                SetCurrentFilter(Selected);
            }
        }

        private void Combo_ColourChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.Write("ColourChanged by {0} ", (sender as FrameworkElement).Name);
            var style = viewer.Inlines.FirstInline;
            if(e.AddedItems.Count == 1)
            {
                var tb = e.AddedItems[0] as TextBlock;
                var colour = tb.Inlines.First() as Run;
                Console.Write("to {0}\n", (colour.Background as SolidColorBrush).Color);
                if(sender == BackgroundCombo)
                {
                    style.Background = colour.Background;
                }
                else if(sender == ForegroundCombo)
                {
                    style.Foreground = colour.Background;
                }
            }
        }
        private void Combo_FontStyleChanged(object sender, SelectionChangedEventArgs e)
        {
            var style = viewer.Inlines.FirstInline;
            if(e.AddedItems.Count == 1)
            {
                FontStyle fontStyle = (FontStyle) e.AddedItems[0];
                style.FontStyle = fontStyle;
            }
        }
        private void Combo_FontWeightChanged(object sender, SelectionChangedEventArgs e)
        {
            var style = viewer.Inlines.FirstInline;
            if(e.AddedItems.Count == 1)
            {
                FontWeight fontWeight = (FontWeight) e.AddedItems[0];
                style.FontWeight = fontWeight;
            }
        }

    }
}
