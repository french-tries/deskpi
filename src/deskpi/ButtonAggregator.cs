using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Optional;
using piCommon;

namespace deskpi
{
    public class ButtonAggregator : IButtonAggregator
    {
        public ButtonAggregator(Func<ITicker> getTicker, IEnumerable<IButton<ButtonId>> buttons)
        {
            this.getTicker = getTicker;
            this.buttons = ImmutableDictionary<ButtonId, IButton<ButtonId>>.Empty.AddRange(
                from button in buttons 
                select new KeyValuePair<ButtonId, IButton<ButtonId>>(button.Id, button));
            this.KeyState = KeyId.None;
    }

        private ButtonAggregator(ButtonAggregator source, Func<ITicker> getTicker = null,
            ImmutableDictionary<ButtonId, IButton<ButtonId>> buttons = null,
            KeyId? Pressed = null, Option<ITicker>? ticker = null)
        {
            this.getTicker = getTicker ?? source.getTicker;
            this.buttons = buttons ?? source.buttons;
            this.KeyState = Pressed ?? source.KeyState;
            this.ticker = ticker ?? source.ticker;
        }

        public IButtonAggregator Tick(uint currentTime)
        {
            var updated = from entry in buttons
                          let newVal = entry.Value.Tick(currentTime)
                          where newVal != entry.Value
                          select new KeyValuePair<ButtonId, IButton<ButtonId>>(entry.Key, newVal);

            if (!updated.Any())
            {
                var result = this;
                ticker.MatchSome((tck) => {
                    if (tck.Ticked(currentTime))
                    {
                        result = new ButtonAggregator(this, Pressed: ButtonsToKey(), 
                            ticker: Option.None<ITicker>());
                    }
                });
                return result;
            }
            return new ButtonAggregator(this, buttons: buttons.SetItems(updated), 
                ticker: getTicker().SomeNotNull());
        }

        public uint? NextTick(uint currentTime) =>
            PiUtils.Min(PiUtils.NextTick(ticker, currentTime), 
                buttons.Values.Min((button) => button.NextTick(currentTime)));


        public IButtonAggregator OnPinValueChange(ButtonId button)
        {
            if (!buttons.ContainsKey(button))
            {
                Console.WriteLine($"Unrecognized button {button}");
                return this;
            }
            return new ButtonAggregator(this, 
                buttons: buttons.SetItem(button, buttons[button].OnPinValueChange()));
        }

        public KeyId KeyState { get; }

        private KeyId ButtonsToKey()
        {
            var pressed = from button in buttons.Values where button.Pressed select (int)button.Id;
            var sum = pressed.Sum();

            if (sum < 0 || sum > (int)KeyId.G)
            {
                Console.WriteLine($"Error in ButtonsToKey");
                return KeyId.None;
            }
            return (KeyId)sum;
        }

        private static int CountPressed(ImmutableDictionary<ButtonId, IButton<ButtonId>> buttons) =>
            buttons.Values.Aggregate(0, (count, button) => button.Pressed ? count + 1 : count);

        private readonly Func<ITicker> getTicker;
        private readonly ImmutableDictionary<ButtonId, IButton<ButtonId>> buttons;
        private readonly Option<ITicker> ticker = Option.None<ITicker>();
    }
}
