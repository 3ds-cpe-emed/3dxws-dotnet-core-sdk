using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ds.enovia.dsdo.model
{
    public class DerivedOutputs : DerivedOutputsReference
    {
        public string id { get; set; }
        public List<DerivedOutputFile> derivedOutputfiles { get; set; }
    }
}
