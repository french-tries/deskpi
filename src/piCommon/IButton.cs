namespace piCommon
{
    public interface IButton<T> : ITickable<IButton<T>>
    {
        T Id { get; }
        bool Pressed { get; }

        IButton<T> OnPinValueChange();
    }
}