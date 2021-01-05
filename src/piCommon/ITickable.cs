namespace piCommon
{
    public interface ITickable<T>
    {
        uint? NextTick(uint currentTime);
        T Tick(uint currentTime);
    }
}