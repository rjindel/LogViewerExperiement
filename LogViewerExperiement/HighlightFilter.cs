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
    class HighlightFilter : FilterBase
    {
        string m_SearchPattern;

        Run m_Style;

        public HighlightFilter(string searchPattern, Run HighlightStyle)
        {
            m_SearchPattern = searchPattern;
            m_Parser = new Regex(m_SearchPattern, RegexOptions.Singleline | RegexOptions.Compiled);
            m_Style = HighlightStyle;
        }

        public override TextBlock Process(string Text)
        {
            //TODO: rename
            TextBlock textBlock = new TextBlock();

            Match match = m_Parser.Match(Text);
            if (match.Success)
            {
                Run HighlightedText = new Run();
                HighlightedText.Background = m_Style.Background;
                HighlightedText.Foreground = m_Style.Foreground;
                HighlightedText.FontStyle  = m_Style.FontStyle;
                HighlightedText.FontWeight = m_Style.FontWeight;

                HighlightedText.Text = match.Value;
                textBlock.Inlines.Add(HighlightedText);
                textBlock.Inlines.Add(Text.Substring(match.Index + match.Length));
            }
            else
            {
                textBlock.Inlines.Add(Text);
            }
            return textBlock;
        }
    }
}
