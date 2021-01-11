using System;
using Optional;

namespace piCommon
{
    public class Button<T> : IButton<T>
    {
        public Button(Func<ITicker> getTicker, Func<bool> read,
            Action<bool> onUpdate, T Id)
        {
            this.getTicker = getTicker;
            this.read = read;
            this.onUpdate = onUpdate;
            this.Id = Id;
            this.Pressed = read();
        }

        private Button(Button<T> source, T Id,
            Func<ITicker> getTicker = null, Func<bool> read = null,
            Action<bool> onUpdate = null, bool? Pressed = null,
            Option<ITicker>? ticker = null)
        {
            this.getTicker = getTicker ?? source.getTicker;
            this.read = read ?? source.read;
            this.onUpdate = onUpdate ?? source.onUpdate;
            this.Id = Id;
            this.Pressed = Pressed ?? source.Pressed;
            this.ticker = ticker ?? source.ticker;
        }

        public IButton<T> OnPinValueChange() =>
            new Button<T>(this, Id, ticker: getTicker().Some());

        public IButton<T> Tick(uint currentTime)
        {
            var result = this;
            ticker.MatchSome((tck) => {
                if (tck.Ticked(currentTime))
                {
                    var newValue = read();
                    if (Pressed != newValue)
                    {
                        onUpdate(newValue);
                        result = new Button<T>(this, Id,
                            Pressed: newValue, ticker: Option.None<ITicker>());
                    }
                }
            } );
            return result;
        }

        public uint? NextTick(uint currentTime) => PiUtils.NextTick(ticker, currentTime);

        public T Id { get; }
        public bool Pressed { get; }

        private readonly Func<ITicker> getTicker;
        private readonly Func<bool> read;
        private readonly Action<bool> onUpdate;

        private readonly Option<ITicker> ticker = Option.None<ITicker>();
    }
}
