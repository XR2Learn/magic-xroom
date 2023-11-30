using DataCollection.Data;

namespace DataCollection.Devices
{
    public interface IDeviceable
    {
        bool IsPresent();
        bool IsAvailable();
        string GetName();
        IDatable GetData();
    }
}