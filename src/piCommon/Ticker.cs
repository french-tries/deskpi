using System.Diagnostics;
using Optional;

namespace piCommon
{
    public class Ticker : ITicker
    {
        public Ticker(uint interval, uint startTime)
        {
            Interval = interval;
            this.startTime = startTime;

            Debug.Assert(interval < uint.MaxValue / 4);
        }

        public bool Ticked(uint currentTime) =>
            currentTime - startTime >= Interval;

        public uint? Remaining(uint currentTime)
        {
            var next = startTime + Interval;

            if (next - currentTime > currentTime - next)
            {
                return 0;
            }
            return next - currentTime;
        }

        public uint Interval { get; }
        private readonly uint startTime;
    }
}
