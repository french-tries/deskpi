﻿namespace immutableSsd.src
{
    public interface ISsdWriter<TPARAM> : IInterruptReceiver<ISsdWriter<TPARAM>>
    {
        int AvailableDigits { get; }
        ISsdWriter<TPARAM> Write(TPARAM values);
    }
}