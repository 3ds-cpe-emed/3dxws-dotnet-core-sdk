using ds.delmia.dsmfg.interfaces;
using ds.enovia.common.serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ds.delmia.dsmfg.model.reference
{
    [MaskSchema(MFGResourceNames.DSMFG_MFGITEM_MASK_DEFAULT)]
    [ItemTypeSchema(MFGResourceNames.PROVIDED_PART_TYPE)]

    public class ProvidedPart : ManufacturingItem, IProvidedPart
    {
    }

   

}