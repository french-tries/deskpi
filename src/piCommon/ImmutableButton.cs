using System;
namespace piCommon
{
    public class ImmutableButton
    {
        public ImmutableButton(Func<object, uint, Action> requestInterrupt, Func<bool> read, Action<bool> onUpdate,
            uint debounceDelay = 50) :
            this(requestInterrupt, read, onUpdate, debounceDelay, read())
        {
        }

        private ImmutableButton(Func<object, uint, Action> requestInterrupt, Func<bool> read, Action<bool> onUpdate,
            uint debounceDelay, bool currentValue, bool interrupt = false)
        {
            this.requestInterrupt = requestInterrupt;
            this.read = read;
            this.onUpdate = onUpdate;
            this.debounceDelay = debounceDelay;
            this.currentValue = currentValue;

            if (interrupt)
            {
                cancelInterrupt = requestInterrupt(this, debounceDelay);
            }
        }

        public ImmutableButton OnPinValueChange()
        {
            cancelInterrupt?.Invoke();
            return new ImmutableButton(requestInterrupt, read, onUpdate, debounceDelay, currentValue, true);
        }

        public ImmutableButton ReceiveInterrupt(object caller)
        {
            var newValue = read();
            if (caller == this && currentValue != newValue)
            {
                onUpdate(newValue);
                return new ImmutableButton(requestInterrupt, read, onUpdate, debounceDelay, newValue);
            }
            return this;
        }

        private readonly Func<object, uint, Action> requestInterrupt;
        private readonly Func<bool> read;
        private readonly Action<bool> onUpdate;
        private readonly uint debounceDelay;

        private readonly bool currentValue;

        private readonly Action cancelInterrupt;
    }
}
