using ds.enovia.common.model;

namespace ds.enovia.dsdo.model
{
    public class PartDerivedOutputRefObject : BusinessObjectId
    {
        public PartDerivedOutputRefObject(string _ownerId)
        {
            source = "$3DSpace";
            type = "VPMReference";
            id = _ownerId;
            relativePath = $"/resources/v1/modeler/dsxcad/dsxcad:Part/{_ownerId}";
        }
    }

    public class DrawingDerivedOutputRefObject : BusinessObjectId
    {
        public DrawingDerivedOutputRefObject(string _ownerId)
        {
            source = "$3DSpace";
            type = "Drawing";
            id = _ownerId;
            relativePath = $"/resources/v1/modeler/dsxcad/dsxcad:Drawing/{_ownerId}";
        }
    }
}
