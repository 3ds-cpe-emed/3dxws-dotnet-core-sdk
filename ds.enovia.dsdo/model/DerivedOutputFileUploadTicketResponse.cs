using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ds.enovia.dsdo.model
{
    public class DerivedOutputFileUploadTicketResponse
    {
        public string success;    //this was defined as string and it should be boolean...
        public string statusCode; //this was defined as string and it should be integer ...
        public DerivedOutputFileUploadTicket data;
    }
}
