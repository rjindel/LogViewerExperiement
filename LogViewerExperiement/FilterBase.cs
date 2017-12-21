using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LogViewerExperiement
{
    abstract class FilterBase
    {
        protected Regex m_Parser;

        //FilterBase(string SearchPattern)

        abstract public TextBlock Process(string Text);
    }
}
