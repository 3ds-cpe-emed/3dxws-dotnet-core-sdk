using System;
using System.Collections.Generic;
using System.Text;

namespace ds.enovia.common.interfaces.attributes
{
    public interface ICustomerAttributes
    {
        IDictionary<string, object> CustomerAttributes { get; set; }
    }
}
