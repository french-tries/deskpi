using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace deskpi
{
    public class ButtonAggregator
    {
        public enum Button { Bottom, Middle, Top }
        public enum Key { A, B, C, D, E, F, G }

        public ButtonAggregator(Action<Key> onKeyPressed) : 
            this(onKeyPressed, ImmutableSortedSet<Button>.Empty)
        {
        }

        private ButtonAggregator(Action<Key> onKeyPressed, ImmutableSortedSet<Button> pressed,
            bool previouslyReleased = false)
        {
            this.onKeyPressed = onKeyPressed;
            this.pressed = pressed;
            this.previouslyReleased = previouslyReleased;
        }


        public ButtonAggregator OnButtonUpdate(Button id, bool pressing)
        {
            if (pressing)
            {
                if (pressed.Contains(id)) return this;

                return new ButtonAggregator(onKeyPressed, pressed.Add(id), false);
            }
            if (!pressed.Contains(id))
            {
                return this;
            }
            if (!previouslyReleased)
            {
                onKeyPressed(buttonsToKey[(pressed.Contains(Button.Bottom), 
                    pressed.Contains(Button.Middle), pressed.Contains(Button.Top))]);
            }

            return new ButtonAggregator(onKeyPressed, pressed.Remove(id), true);
        }

        private readonly Action<Key> onKeyPressed;

        private readonly bool previouslyReleased;
        private readonly ImmutableSortedSet<Button> pressed = ImmutableSortedSet<Button>.Empty;

        private static readonly ImmutableDictionary<(bool, bool, bool), Key> buttonsToKey =
            new Dictionary<(bool, bool, bool), Key>{
                { (true, false, false), Key.A },
                { (false, true, false), Key.B },
                { (false, false, true), Key.C },
                { (true, true, false), Key.D },
                { (false, true, true), Key.E },
                { (true, false, true), Key.F },
                { (true, true, true), Key.G }
            }.ToImmutableDictionary();
    }
}
