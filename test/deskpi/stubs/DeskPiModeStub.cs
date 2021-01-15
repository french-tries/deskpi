using System;
using System.Collections.Immutable;

namespace deskpi.test.stubs
{
    public class DeskPiModeStub : DeskPiMode
    {
        public DeskPiModeStub() : base(() => null, new ModeData("", "", "", Song.EmptySong))
        {
            Next = this;
        }

        public override ImmutableList<(string, uint)> Text => DeskPiUtils.StringToText(TextVal);

        public override uint? NextTick(uint currentTime) => NextTickVal;

        protected override DeskPiMode ReceiveKeyImpl(KeyId key)
        {
            ReceivedKey = key;
            return Next;
        }

        public override DeskPiMode Tick(uint currentTicks)
        {
            ReceivedTick = currentTicks;
            return Next;
        }

        public string TextVal { get; set; }
        public uint? NextTickVal { get; set; }

        public KeyId ReceivedKey { get; set; }
        public uint ReceivedTick { get; set; }

        public DeskPiModeStub Next { get; set; }
    }
}
