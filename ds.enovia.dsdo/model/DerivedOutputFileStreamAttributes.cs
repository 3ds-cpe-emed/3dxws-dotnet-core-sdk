using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ds.enovia.dsdo.model
{
    public class DerivedOutputFileStreamAttributes
    {
        public string persistencyName { get; set; } //e.g. "PDF_xcadmodel-75995075-00000001(Default)"
        public string persistencyType { get; set; } //e.g. "pdf"
    }
}
