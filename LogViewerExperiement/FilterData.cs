using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;


namespace LogViewerExperiement
{
    class FilterData
    {
        public override string ToString()
        {
                return m_Name;
        }
        public string m_Name;
        public string m_FilterPattern;
        public Run m_Style;
    }
}
