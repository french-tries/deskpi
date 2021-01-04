using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using piCommon;

namespace deskpi
{
    public enum Button { Bottom, Middle, Top }
    public enum Key { None, A, B, C, D, E, F, G }

    public class ButtonAggregator
    {
        public ButtonAggregator(ImmutableButton top, ImmutableButton middle,
            ImmutableButton bottom)
        {
            this.buttons = new Dictionary<Button, ImmutableButton> {
                {Button.Top, top},
                {Button.Middle, middle},
                {Button.Bottom, bottom}
            }.ToImmutableDictionary();

            this.KeyState = Key.None;
    }

        private ButtonAggregator(ButtonAggregator source, 
            ImmutableDictionary<Button, ImmutableButton> buttons = null,
            Key? Pressed = null, bool? previouslyReleased = null)
        {
            this.buttons = buttons ?? source.buttons;
            this.KeyState = Pressed ?? source.KeyState;
            this.previouslyReleased = previouslyReleased ?? source.previouslyReleased;
        }

        private static int CountPressed(ImmutableDictionary<Button, ImmutableButton> buttons) => 
            buttons.Values.Aggregate(0, (count, button) => button.Pressed ? count + 1 : count);

        public ButtonAggregator ReceiveInterrupt(object caller)
        {
            var updated = from entry in buttons 
                let newVal = entry.Value.ReceiveInterrupt(caller)
                where newVal != entry.Value
                select new KeyValuePair<Button, ImmutableButton>(entry.Key, newVal);

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
                return new ButtonAggregator(this, buttonsN,
                    buttonsToKey[(buttons[Button.Top].Pressed, 
                    buttons[Button.Middle].Pressed, buttons[Button.Bottom].Pressed)], true);
            }
            return new ButtonAggregator(this, buttonsN, Key.None, true);
        }

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

        private readonly ImmutableDictionary<Button, ImmutableButton> buttons;

        private readonly bool previouslyReleased;

        // todo change this to make independent of the button id type or count
        private static readonly ImmutableDictionary<(bool, bool, bool), Key> buttonsToKey =
            new Dictionary<(bool, bool, bool), Key>{
                { (false, false, false), Key.None },
                { (false, false, true), Key.A },
                { (false, true, false), Key.B },
                { (true, false, false), Key.C },
                { (false, true, true), Key.D },
                { (true, true, false), Key.E },
                { (true, false, true), Key.F },
                { (true, true, true), Key.G }
            }.ToImmutableDictionary();
    }
}
