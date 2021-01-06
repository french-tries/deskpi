namespace piCommon
{
    public interface ITickable<T>
    {
        uint? NextTick(uint currentTime);
        T Tick(uint currentTime);
    }

    public interface ITicker
    {
        bool Ticked(uint currentTime);
        uint? Remaining(uint currentTime);
        uint Interval { get; }
    }
}