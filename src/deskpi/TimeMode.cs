using System;
using System.Collections.Immutable;
using Optional;
using piCommon;

namespace deskpi
{
    public class TimeMode : IDeskPiMode
    {
        // todo getTicker should use millis received in deskpi tick, check elsewhere?
        public TimeMode(Func<DateTime> now, Func<uint, ITicker> getTicker)
        {
            this.now = now;
            this.displayTime = now();
        }

        private TimeMode(TimeMode source, Func<DateTime> now = null,
            DateTime? displayTime = null)
        {
            this.now = now ?? source.now;
            this.displayTime = displayTime ?? source.displayTime;
        }

        public ImmutableList<(string, uint)> Text => DeskPiUtils.StringToText(
            $"{displayTime:MM.ddHH.mm}");

        public IDeskPiMode ReceiveKey(KeyId key) => this;

        public uint? NextTick(uint currentTime) =>
            (uint?)(now().Minute == displayTime.Minute ? 1000 : 0);

        // todo called 2 times per minute
        public IDeskPiMode Tick(uint currentTicks)
        {
            var currentTime = now();

            if (currentTime.Minute != displayTime.Minute)
            {
                Console.WriteLine($"{displayTime:MM.ddHH.mm}");
                Console.WriteLine($"{currentTime:MM.ddHH.mm}");

                return new TimeMode(this, displayTime: currentTime);
            }
            return this;
        }

        private readonly Func<DateTime> now;
        private readonly DateTime displayTime;
    }
}
