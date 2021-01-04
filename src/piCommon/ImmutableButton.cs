using System;
namespace piCommon
{
    public class ImmutableButton
    {
        public ImmutableButton(Func<object, uint, Action> requestInterrupt, Func<bool> read,
            Action<bool> onUpdate, uint debounceDelay = 50)
        {
            this.requestInterrupt = requestInterrupt;
            this.read = read;
            this.onUpdate = onUpdate;
            this.debounceDelay = debounceDelay;
            this.Pressed = read();
        }

        private ImmutableButton(ImmutableButton source, 
            Func<object, uint, Action> requestInterrupt = null, 
            Func<bool> read = null, Action<bool> onUpdate = null,
            uint? debounceDelay = null, bool? Pressed = null, bool interrupt = false)
        {
            this.requestInterrupt = requestInterrupt ?? source.requestInterrupt;
            this.read = read ?? source.read;
            this.onUpdate = onUpdate ?? source.onUpdate;
            this.debounceDelay = debounceDelay ?? source.debounceDelay;
            this.Pressed = Pressed ?? source.Pressed;

            if (interrupt == true)
            {
                cancelInterrupt = this.requestInterrupt(this, this.debounceDelay);
            }
        }

        public ImmutableButton OnPinValueChange()
        {
            cancelInterrupt?.Invoke();
            return new ImmutableButton(this, interrupt: true);
        }

        public ImmutableButton ReceiveInterrupt(object caller)
        {
            if (caller != this) return this;

            var newValue = read();
            if (Pressed != newValue)
            {
                onUpdate(newValue);
                return new ImmutableButton(this, Pressed: newValue);
            }
            return this;
        }

        public bool Pressed { get; }

        private readonly Func<object, uint, Action> requestInterrupt;
        private readonly Func<bool> read;
        private readonly Action<bool> onUpdate;
        private readonly uint debounceDelay;

        private readonly Action cancelInterrupt;
    }
}
