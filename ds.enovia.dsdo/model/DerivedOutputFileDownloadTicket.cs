using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ds.enovia.dsdo.model
{
    public class DerivedOutputFileDownloadTicket
    {
        public string doid { get; set; } //e.g. "3101B3B9140B412D8754CD8A92189ED5",
        public FileDownloadTicket dataelements { get; set; }
    }
}
