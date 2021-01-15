using System;
using NUnit.Framework;
using piCommon;

namespace deskpi.test
{
    [TestFixture]
    public class TimeModeTests
    {
        [TestCase]
        public void Text_ReturnsFormattedTime()
        {
            var now = DateTime.Now;
            uint ticks = 0;
            var mode = new TimeMode(()=> null, () => now, (arg) => new Ticker(arg, ticks));

            Assert.AreEqual($"{now:MM.ddHH.mm}", mode.Text[0].Item1);
        }
        /*
        [TestCase]
        public void NextTick_AtStart_ReturnsLeftUntilNextMinute()
        {
            var now = new DateTime(1999, 1, 2, 3, 4, 5);
            uint ticks = 0;
            var mode = new TimeMode(() => now, (arg) => new Ticker(arg, ticks));

            var next = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0)
                .AddMinutes(1);
            var until = (next - now).Ticks / TimeSpan.TicksPerMillisecond;

            Assert.AreEqual(until, mode.NextTick(ticks));
        }

        [TestCase]
        public void Tick_NewMin_UpdatesTime()
        {
            var now = new DateTime(1999, 1, 2, 3, 4, 5);
            uint ticks = 0;
            IDeskPiMode mode = new TimeMode(() => now, (arg) => new Ticker(arg, ticks));

            now = now.AddMinutes(1);
            ticks = 60000;

            mode = mode.Tick(ticks);
            Assert.AreEqual($"{now:MM.ddHH.mm}", mode.Text[0].Item1);
        }*/
    }
}
