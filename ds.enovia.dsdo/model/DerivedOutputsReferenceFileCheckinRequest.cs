using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ds.enovia.dsdo.model
{
    public class DerivedOutputsReferenceFileCheckinRequest : DerivedOutputsReference
    {
        public string fileCount; //weird that this is a string rather than a long...

        public DerivedOutputsReferenceFileCheckinRequest()
        { }

        public DerivedOutputsReferenceFileCheckinRequest(long fileCount, string _ownerId)
        {
            this.fileCount = fileCount.ToString();

          //  this.referencedObject = new DerivedOutputRefObject("VPMReference", _ownerId);
        }
    }
}
