using ds.delmia.dsmfg.interfaces;
using ds.enovia.common.serialization;

namespace ds.delmia.dsmfg.model.reference
{
    [MaskSchema(MFGResourceNames.DSMFG_MFGITEM_MASK_DEFAULT)]
    [ItemTypeSchema(MFGResourceNames.MANUFACTURING_ASSEMBLY_TYPE)]
    public class ManufacturingAssembly : ManufacturingItem, IManufacturingAssembly
    {
       
    }
}
