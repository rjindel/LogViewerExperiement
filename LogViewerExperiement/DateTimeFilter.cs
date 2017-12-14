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
    class DateTimeFilter
    {
        //const string LOG_LINE_REGEX = @"^\[(?<year>\d*)\.(?<month>\d*)\.(?<day>\d*)-(?<hour>\d*)\.(?<minute>\d*)\.(?<second>\d*):(?<milisecond>\d*)\]\[(?<thing>\s*\d*)\](?<category>\w+):(?<message>.*)";
        //const string LOG_LINE_REGEX = @"^\[(?<year>\d+)\.(?<month>\d+)\.(?<day>\d+)-(?<hour>\d+)\.(?<minute>\d+)\.(?<second>\d+)\:(?<milisecond>\d+)\]\[(?<frame>\s*d*)\]";
        //const string LOG_LINE_REGEX = @"^\[(?<year>\d+)\.(?<month>\d+)\.(?<day>\d+)-(?<hour>\d+)\.(?<minute>\d+)\.(?<second>\d+)\:(?<milisecond>\d+)\]";
        const string LOG_LINE_REGEX = @"^\[(\d+)\.(\d+)\:(?<year>\d+)\.(?<month>\d+)\.(?<day>\d+)-(?<hour>\d+)\.(?<minute>\d+)\.(?<second>\d+)]";

        Regex m_parser;
        public DateTimeFilter() {
            m_parser = new Regex(LOG_LINE_REGEX, RegexOptions.Singleline | RegexOptions.Compiled);
        }
        public TextBlock Process(String Text)
        {
            TextBlock tb = new TextBlock();
            Match match = m_parser.Match(Text);
            if (match.Success)
            {
                tb.Inlines.Add(new Run(match.Value) { FontWeight = FontWeights.Bold });
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
