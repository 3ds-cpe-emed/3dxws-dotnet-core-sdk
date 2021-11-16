using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ds.enovia.dsdo.mask
{
    public enum DerivedOutputsMask
    {
        [DefaultValue("dsmvdo:DerivedOutputsMask.Details")]
        Details,
        [DefaultValue("dsmvdo:DerivedOutputsMask.AllDetails")]
        AllDetails
    }
}
