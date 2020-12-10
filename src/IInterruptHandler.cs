using System;
namespace immutableSsd.src
{
    // @todo unrequest interrupt?
    public interface IInterruptHandler
    {
        void RequestInterrupt(object caller, uint delay);
    }
}
