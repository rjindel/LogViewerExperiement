using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LogViewerExperiement
{
    class LogCategoriesFilter : FilterBase
    {
        Window windoww;
        ListView CategoryList;
        List<string> m_Categories;
        string m_SearchPattern;
        string m_CategoryName = "categ";

        public LogCategoriesFilter(string searchPattern)
        {
            m_SearchPattern = searchPattern;
            m_Parser = new Regex(m_SearchPattern, RegexOptions.Singleline | RegexOptions.Compiled);
            m_Categories = new List<string>();
        }
        public override TextBlock Process(string Text)
        {
            Match match = m_Parser.Match(Text);
            if(match.Success)
            {
                string category = match.Groups[m_CategoryName].Value.Trim();
                m_Categories.Add(category);
            }
            return null;
        }
        public void Finish()
        {
            m_Categories = m_Categories.Distinct().ToList();
            List<TextBlock> items = new List<TextBlock>();
            foreach(string cat in m_Categories)
            {
                TextBlock textBox = new TextBlock( new Run(cat));
                items.Add(textBox);
            }
            windoww = new Window();
            CategoryList = new ListView();
            CategoryList.ItemsSource = items;
            windoww.Content = CategoryList;
            windoww.Show();
        }
    }
}
