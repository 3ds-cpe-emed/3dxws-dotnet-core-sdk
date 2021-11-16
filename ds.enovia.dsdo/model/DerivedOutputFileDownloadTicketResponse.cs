using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ds.enovia.dsdo.model
{
    public class DerivedOutputFileDownloadTicketResponse
    {
        public bool success { get; set; }
        public int statusCode { get; set; }
        public DerivedOutputFileDownloadTicket data { get; set; }
    }
}
