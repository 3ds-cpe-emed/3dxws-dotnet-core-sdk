using System;
using System.Collections.Generic;
using System.Text;

namespace ds.enovia.common.serialization
{
    public abstract class MaskJson3DXSchemaConverter<T> : Json3DXSchemaConverter<T>
    {
        public abstract string MaskName { get; protected set; }
    }
}
