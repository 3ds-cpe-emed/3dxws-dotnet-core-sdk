using ds.enovia.common.interfaces.attributes;

namespace ds.enovia.common.interfaces
{
    public interface IItem : IUniqueIdentifier, IType, ITitle, IDescription, IModificationDates, IName, ILifecycle, IOwnership, IConcurrentEngineering
    {

    }
}
