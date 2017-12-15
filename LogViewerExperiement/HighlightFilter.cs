using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Controls;

namespace LogViewerExperiement
{
    class HighlightFilter
    {
        string m_SearchPattern;

        Run m_Style;
        Regex m_Parser;

        public HighlightFilter(string searchPattern, Run HighlightStyle)
        {
            m_SearchPattern = searchPattern;
            m_Parser = new Regex(m_SearchPattern, RegexOptions.Singleline | RegexOptions.Compiled);
            m_Style = HighlightStyle;
        }

        public TextBlock Process(string Text)
        {
            //TODO: rename
            TextBlock tb = new TextBlock();

            Match match = m_Parser.Match(Text);
            if (match.Success)
            {
                Run HighlightedText = new Run();
                HighlightedText.Foreground = m_Style.Foreground;
                HighlightedText.FontWeight = m_Style.FontWeight;
                HighlightedText.Background = m_Style.Background;
                //HighlightedText. = m_Style.;
                HighlightedText.Text = match.Value;
                tb.Inlines.Add(HighlightedText);
                tb.Inlines.Add(Text.Substring(match.Index + match.Length));
            }
            else
            {
                tb.Inlines.Add(Text);
            }
            return tb;
        }
    }
}
