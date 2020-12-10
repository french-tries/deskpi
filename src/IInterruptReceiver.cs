using System;
namespace immutableSsd.src
{
    public interface IInterruptReceiver<T>
    {
        T ReceiveInterrupt(object caller, uint currentTime);
    }
}
