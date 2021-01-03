using System.Collections.Immutable;
using piCommon;

namespace immutableSsd
{
    public interface ISelector<T>
    {
        ImmutableList<T> GetSelected();
        ISelector<T> ReceiveInterrupt(object caller);
    }
}