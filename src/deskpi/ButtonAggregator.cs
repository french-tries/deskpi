using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using piCommon;

namespace deskpi
{
    [Flags]
    public enum Button { Bottom = 1, Middle = 2, Top = 4 }
    public enum Key { None, A = 1, B = 2, C = 4, D = 3, E = 6, F = 5, G = 7 }

    public class ButtonAggregator : ITickable<ButtonAggregator>
    {
        public ButtonAggregator(IEnumerable<ImmutableButton<Button>> buttons)
        {
            this.buttons = ImmutableDictionary<Button, ImmutableButton<Button>>.Empty.AddRange(
                from button in buttons 
                select new KeyValuePair<Button, ImmutableButton<Button>>(button.Id, button));
            this.KeyState = Key.None;
    }

        private ButtonAggregator(ButtonAggregator source, 
            ImmutableDictionary<Button, ImmutableButton<Button>> buttons = null,
            Key? Pressed = null, bool? previouslyReleased = null)
        {
            this.buttons = buttons ?? source.buttons;
            this.KeyState = Pressed ?? source.KeyState;
            this.previouslyReleased = previouslyReleased ?? source.previouslyReleased;
        }

        private static int CountPressed(ImmutableDictionary<Button, ImmutableButton<Button>> buttons) => 
            buttons.Values.Aggregate(0, (count, button) => button.Pressed ? count + 1 : count);

        public ButtonAggregator Tick(uint currentTime)
        {
            var updated = from entry in buttons
                          let newVal = entry.Value.Tick(currentTime)
                          where newVal != entry.Value
                          select new KeyValuePair<Button, ImmutableButton<Button>>(entry.Key, newVal);

            if (!updated.Any())
            {
                return this;
            }
            var buttonsN = buttons.SetItems(updated);

            if (CountPressed(buttons) <= CountPressed(buttonsN))
            {
                return new ButtonAggregator(this, buttonsN, Key.None, false);
            }
            if (!previouslyReleased)
            {
                return new ButtonAggregator(this, buttonsN, ButtonsToKey(), true);
            }
            return new ButtonAggregator(this, buttonsN, Key.None, true);
        }

        public uint? NextTick(uint currentTime) =>
            buttons.Values.Min((button) => button.NextTick(currentTime));


        public ButtonAggregator OnPinValueChange(Button button)
        {
            if (!buttons.ContainsKey(button))
            {
                Console.WriteLine($"Unrecognized button {button}");
                return this;
            }
            return new ButtonAggregator(this, 
                buttons.SetItem(button, buttons[button].OnPinValueChange()));
        }

        public Key KeyState { get; }

        private readonly ImmutableDictionary<Button, ImmutableButton<Button>> buttons;

        private readonly bool previouslyReleased;

        private Key ButtonsToKey()
        {
            var pressed = from button in buttons.Values where button.Pressed select (int)button.Id;
            var sum = pressed.Sum();

            if (sum < 0 || sum > (int)Key.G)
            {
                Console.WriteLine($"Error in ButtonsToKey");
                return Key.None;
            }
            return (Key)sum;
        }
    }
}
