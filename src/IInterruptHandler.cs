using System;
namespace immutableSsd.src
{
    // TODO unrequest interrupt?
    public interface IInterruptHandler
    {
        void RequestInterrupt(object caller, uint delay);
    }
}
