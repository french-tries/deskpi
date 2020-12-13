using System;
namespace immutableSsd.src
{
    public interface IInterruptHandler
    {
        void RequestInterrupt(object caller, uint delay);
        void UnrequestInterrupt(object caller);
    }
}
