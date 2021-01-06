using System.Collections.Immutable;
using piCommon;

namespace immutableSsd
{
    // todo ishh...
    public interface ISsdWriter<T> : ITickable<ISsdWriter<T>>
    {
        uint AvailableDigits { get; }
        ISsdWriter<T> Write(T values);
    }
}