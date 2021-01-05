using System;
namespace piCommon
{
    public class ImmutableButton<T>
    {
        public ImmutableButton(Func<object, uint, Action> requestInterrupt, Func<bool> read,
            Action<bool> onUpdate, T Id, uint debounceDelay = 50)
        {
            this.requestInterrupt = requestInterrupt;
            this.read = read;
            this.onUpdate = onUpdate;
            this.debounceDelay = debounceDelay;
            this.Id = Id;
            this.Pressed = read();
        }

        private ImmutableButton(ImmutableButton<T> source, T Id, 
            Func<object, uint, Action> requestInterrupt = null, 
            Func<bool> read = null, Action<bool> onUpdate = null,
            uint? debounceDelay = null, bool? Pressed = null, bool interrupt = false)
        {
            this.requestInterrupt = requestInterrupt ?? source.requestInterrupt;
            this.read = read ?? source.read;
            this.onUpdate = onUpdate ?? source.onUpdate;
            this.debounceDelay = debounceDelay ?? source.debounceDelay;
            this.Id = Id;
            this.Pressed = Pressed ?? source.Pressed;

            if (interrupt == true)
            {
                cancelInterrupt = this.requestInterrupt(this, this.debounceDelay);
            }
        }

        public ImmutableButton<T> OnPinValueChange()
        {
            cancelInterrupt?.Invoke();
            return new ImmutableButton<T>(this, Id, interrupt: true);
        }

        public ImmutableButton<T> ReceiveInterrupt(object caller)
        {
            if (caller != this) return this;
            var newValue = read();
            if (Pressed != newValue)
            {
                onUpdate(newValue);
                return new ImmutableButton<T>(this, Id, Pressed: newValue);
            }
            return this;
        }

        public T Id { get; }
        public bool Pressed { get; }

        private readonly Func<object, uint, Action> requestInterrupt;
        private readonly Func<bool> read;
        private readonly Action<bool> onUpdate;
        private readonly uint debounceDelay;

        private readonly Action cancelInterrupt;
    }
}
