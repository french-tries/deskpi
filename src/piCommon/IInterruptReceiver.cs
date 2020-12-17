using System;
namespace piCommon
{
    public interface IInterruptReceiver<T>
    {
        T ReceiveInterrupt(object caller, uint currentTime);
    }
}
