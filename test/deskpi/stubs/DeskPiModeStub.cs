using System;
using System.Collections.Immutable;

namespace deskpi.test.stubs
{
    public class DeskPiModeStub : IDeskPiMode
    {
        public DeskPiModeStub()
        {
            Next = this;
        }

        public ImmutableList<(string, uint)> Text => DeskPiUtils.StringToText(TextVal);

        public uint? NextTick(uint currentTime) => NextTickVal;

        public IDeskPiMode ReceiveKey(KeyId key)
        {
            ReceivedKey = key;
            return Next;
        }

        public IDeskPiMode Tick(uint currentTime)
        {
            ReceivedTick = currentTime;
            return Next;
        }

        public string TextVal { get; set; }
        public uint? NextTickVal { get; set; }

        public KeyId ReceivedKey { get; set; }
        public uint ReceivedTick { get; set; }

        public DeskPiModeStub Next { get; set; }
    }
}
