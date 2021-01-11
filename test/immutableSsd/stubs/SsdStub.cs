using System.Collections.Immutable;

namespace immutableSsd.test.stubs
{
    public class SsdStub : ISsd
    {
        public SsdStub()
        {
            this.Next = this;
        }

        public uint AvailableDigits => AvailableDigitsVal;

        public uint? NextTick(uint currentTime)
        {
            ReceivedCurrentTime = currentTime;
            return NextTickVal;
        }

        public ISsd Tick(uint currentTime)
        {
            ReceivedCurrentTime = currentTime;
            return Next;
        }

        public ISsd Write(string text)
        {
            ReceivedText = text;
            return Next;
        }

        public ISsd Write(ImmutableList<(string, uint)> texts)
        {
            ReceivedTexts = texts;
            return Next;
        }

        public uint AvailableDigitsVal { get; set; }
        public uint? NextTickVal { get; set; }
        public ISsd Next { get; set; }

        public uint ReceivedCurrentTime { get; set; }
        public string ReceivedText { get; set; }
        public ImmutableList<(string, uint)> ReceivedTexts { get; set; }
    }
}
