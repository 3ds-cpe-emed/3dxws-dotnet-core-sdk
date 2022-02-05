using System;
using System.Collections.Generic;
using System.Text;

namespace ds.delmia.dsmfg.interfaces
{
    public interface IOutsourced
    {     
        /// <summary>
        /// Indicates Yes(2) or No(1) value
        /// </summary>
        string Outsourced { get; set; }
    }
}
