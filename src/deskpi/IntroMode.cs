using System;
using System.Collections.Immutable;
using Optional;
using piCommon;

namespace deskpi
{
    public class IntroMode : DeskPiMode
    {
        private readonly DeskPiMode next;
        private readonly Option<ITicker> ticker = Option.None<ITicker>();

        public IntroMode(DeskPiMode next, Func<ITicker> getTicker) : base(next)
        {
            this.next = next;
            this.ticker = getTicker().SomeNotNull();
        }

        public override ImmutableList<(string, uint)> Text =>
            ImmutableList<(string,uint)>.Empty.Add((next.Data.Name, 0));

        public override uint? NextTick(uint currentTime)
        {
            uint? result = 0;
            ticker.MatchSome(
                (arg) => { result = arg.Remaining(currentTime); });
            return result;
        }

        public override DeskPiMode Tick(uint currentTicks)
        {
            DeskPiMode result = this;
            ticker.Match(
                (arg) => { 
                    if (arg.Ticked(currentTicks))
                    {
                        result = next;
                    }
                },
                () => {
                    result = next;
                });
            return result;
        }

        protected override DeskPiMode ReceiveKeyImpl(KeyId key) => this;
    }
}
