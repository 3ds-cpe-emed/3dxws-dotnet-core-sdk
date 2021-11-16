using ds.enovia.common;
using ds.enovia.common.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ds.enovia.dsdo.model
{
    public class DerivedOutputsReferences : SerializableJsonObject
    {
        public List<BusinessObjectId> referencedObject { get; set; }
        
        public DerivedOutputsReferences()
        {
            referencedObject = new List<BusinessObjectId>();
        }
    }
}
