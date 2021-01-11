using System;

namespace piCommon.test.stubs
{
    public class ButtonStub<T> : IButton<T>
    {
        public ButtonStub()
        {
            Next = this;
        }

        public T Id => IdVal;
        public bool Pressed => PressedVal;

        public uint? NextTick(uint currentTime) => NextTickVal;

        public IButton<T> OnPinValueChange()
        {
            OnPinValueChanged = true;
            return this;
        }

        public IButton<T> Tick(uint currentTime) => Next;

        public T IdVal { get; set; }
        public bool PressedVal { get; set; }
        public uint? NextTickVal { get; set; }

        public bool OnPinValueChanged { get; set; }

        public ButtonStub<T> Next { get; set; }
    }
}
