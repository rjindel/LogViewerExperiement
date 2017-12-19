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
        List<FontStyle> fontStyles;
        List<FontWeight> fontWeights;
        List<Brush> brushes;
        public CreateFilter()
        {
            InitializeComponent();
            InitDialog();

            CurrentFilter.SelectionChanged += new SelectionChangedEventHandler( Combo_FilterChanged );
        }

        private void InitDialog()
        {
            //Need a better way. Reflection.
            fontStyles = new List<FontStyle>() { FontStyles.Italic, FontStyles.Normal, FontStyles.Oblique };
            FontStyleCombo.ItemsSource = fontStyles;

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

            //Try reflection for brushes (could be used for FontWeight + Style)
            brushes = new List<Brush>();

            Type brushesType = typeof(Brushes);
            var properties = brushesType.GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach(var prop in properties)
            {
                string name = prop.Name;
                Brush brush = prop.GetValue(null, null) as Brush;
                brushes.Add(brush);
            }

            ForegroundCombo.ItemsSource = brushes;
            BackgroundCombo.ItemsSource = typeof(Colors).GetProperties();

            BackgroundCombo.SelectedIndex = 0;
            BackgroundCombo.SelectionChanged += new SelectionChangedEventHandler(Combo_BackgroundChanged);
            ForegroundCombo.SelectedIndex = 0;
            ForegroundCombo.SelectionChanged += new SelectionChangedEventHandler(Combo_ForegroundChanged);
            FontStyleCombo.SelectedIndex = 0;
            FontStyleCombo.SelectionChanged += new SelectionChangedEventHandler(Combo_FontStyleChanged);
            FontWeightCombo.SelectedIndex = 0;
            FontWeightCombo.SelectionChanged += new SelectionChangedEventHandler(Combo_FontWeightChanged);
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
            StreamWriter writer = new StreamWriter("Filter.cfg");

            foreach(FilterData data in CurrentFilter.Items)
            {
                writer.WriteLine(data.m_Name);
                writer.WriteLine(data.m_FilterPattern);
                writer.WriteLine(data.m_Style.ToString());
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
        private void Combo_FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                FilterData Selected = e.AddedItems[0] as FilterData;
                SetCurrentFilter(Selected);
            }
        }

        private void Combo_BackgroundChanged(object sender, SelectionChangedEventArgs e)
        {
            var style = viewer.Inlines.FirstInline;
            if(e.AddedItems.Count == 1)
            {
                var colour = e.AddedItems[0] as PropertyInfo;
                var value = (Color)colour.GetValue(null, null);
                style.Background = new SolidColorBrush( value );
            }
        }
        private void Combo_ForegroundChanged(object sender, SelectionChangedEventArgs e)
        {
            var style = viewer.Inlines.FirstInline;
            if(e.AddedItems.Count == 1)
            {
                Brush colour = e.AddedItems[0] as Brush;
                style.Foreground = colour;
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
