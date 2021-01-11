namespace deskpi.test.stubs
{
    public class ButtonAggregatorStub : IButtonAggregator
    {
        public ButtonAggregatorStub()
        {
            this.Next = this;
        }

        public KeyId KeyState => KeyStateVal;

        public uint? NextTick(uint currentTime)
        {
            ReceivedTick = currentTime;
            return NextTickVal;
        }

        public IButtonAggregator OnPinValueChange(ButtonId button)
        {
            ReceivedButton = button;
            return Next;
        }

        public IButtonAggregator Tick(uint currentTime)
        {
            ReceivedTick = currentTime;
            return Next;
        }

        public KeyId KeyStateVal { get; set; }
        public uint? NextTickVal { get; set; }

        public uint ReceivedTick { get; set; }
        public ButtonId ReceivedButton { get; set; }

        public ButtonAggregatorStub Next { get; set; }
    }
}
