using System;
namespace immutableSsd
{
    public interface IInterruptReceiver<T>
    {
        T ReceiveInterrupt(object caller, uint currentTime);
    }
}
