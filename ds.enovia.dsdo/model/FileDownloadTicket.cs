using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ds.enovia.dsdo.model
{
    public class FileDownloadTicket : FileUploadTicket
    {
        public string filename { get; set; } //e.g. "PDF_xcadmodel-75995075-00000001(Default).pdf"
    }
}
