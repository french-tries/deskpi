using System;
namespace immutableSsd
{
    public interface IInterruptHandler
    {
        void RequestInterrupt(object caller, uint delay);
        void UnrequestInterrupt(object caller);
    }
}
