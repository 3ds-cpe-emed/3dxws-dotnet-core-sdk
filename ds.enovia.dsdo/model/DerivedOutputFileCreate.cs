using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ds.enovia.dsdo.model
{
    public class DerivedOutputFileCreate
    {
        public string receipt; //e.g. "PDF_PDF_xcadmodel-75995075-00000001(Default).pdf"
        public string format; //e.g. "PDF"
        public string filename; //e.g. "PDF_xcadmodel-75995075-00000001(Default).pdf"
        public string checksum; //e.g. 2d972d94665f6a8c0b99e7deb259c5c0

    }
}
