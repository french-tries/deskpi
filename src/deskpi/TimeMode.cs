using System;
using System.Collections.Immutable;
using Optional;
using piCommon;

namespace deskpi
{
    public class TimeMode : DeskPiMode
    {
        // todo getTicker should use millis received in deskpi tick, check elsewhere?
        public TimeMode(Func<DeskPiMode> buildSelector, ModeData data,
            Func<DateTime> now, Func<uint, ITicker> getTicker)
            : base(buildSelector, data)
        {
            this.now = now;
            this.displayTime = now();
        }

        private TimeMode(TimeMode source, Func<DateTime> now = null,
            DateTime? displayTime = null) : base(source)
        {
            this.now = now ?? source.now;
            this.displayTime = displayTime ?? source.displayTime;
        }

        public override ImmutableList<(string, uint)> Text => DeskPiUtils.StringToText(
            $"{displayTime:MM.ddHH.mm}");

        protected override DeskPiMode ReceiveKeyImpl(KeyId key) => this;

        public override uint? NextTick(uint currentTime) =>
            (uint?)(now().Minute == displayTime.Minute ? 1000 : 0);

        public override DeskPiMode Tick(uint currentTicks)
        {
            var currentTime = now();

            if (currentTime.Minute != displayTime.Minute)
            {
                return new TimeMode(this, displayTime: currentTime);
            }
            return this;
        }

        private readonly Func<DateTime> now;
        private readonly DateTime displayTime;
    }
}
