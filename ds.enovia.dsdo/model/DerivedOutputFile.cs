using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ds.enovia.dsdo.model
{
    public class DerivedOutputFile
    {
        public string id { get; set; } //e.g. "PDF_PDF_xcadmodel-75995075-00000001(Default).pdf"
        public bool isSync { get; set; }
        public string converterName { get; set; } //e.g. "SOLIDWORKS"
        public string format { get; set; } //e.g. "PDF"
        public string filename { get; set; } //e.g. "PDF_xcadmodel-75995075-00000001(Default).pdf"
        public string synchroStamp { get; set; } //e.g. "{MD5}9fc104bb368d75abaae1faa72d66d329",
        public string checksum { get; set; } //e.g. 2d972d94665f6a8c0b99e7deb259c5c0
        public bool isExternal { get; set; }
        public bool downloadable { get; set; }
        public DerivedOutputFileStreamAttributes streamAttributes { get; set; }
    }
}
