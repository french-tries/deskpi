using System;
namespace piCommon
{
    public class ImmutableButton : IInterruptReceiver<ImmutableButton>
    {
        public ImmutableButton(IInterruptHandler handler, Func<bool> read, Action<bool> onUpdate,
            uint debounceDelay = 50) : 
            this(handler, read, onUpdate, debounceDelay, read())
        {
        }

        private ImmutableButton(IInterruptHandler handler, Func<bool> read, Action<bool> onUpdate,
            uint debounceDelay, bool currentValue)
        {
            this.handler = handler;
            this.read = read;
            this.onUpdate = onUpdate;
            this.debounceDelay = debounceDelay;
            this.currentValue = currentValue;
        }

        public ImmutableButton OnPinValueChange()
        {
            handler.UnrequestInterrupt(this);
            handler.RequestInterrupt(this, debounceDelay);

            return this;
        }

        public ImmutableButton ReceiveInterrupt(object caller, uint currentTime)
        {
            var newValue = read();
            if (caller == this && currentValue != newValue)
            {
                onUpdate(newValue);
                return new ImmutableButton(handler, read, onUpdate, debounceDelay, newValue);
            }
            return this;
        }

        private readonly IInterruptHandler handler;
        private readonly Func<bool> read;
        private readonly Action<bool> onUpdate;
        private readonly uint debounceDelay;

        private readonly bool currentValue;

    }
}
