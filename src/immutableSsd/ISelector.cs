using System.Collections.Immutable;
using piCommon;

namespace immutableSsd
{
    public interface ISelector<T>
    {
        ImmutableList<T> GetSelected();
        ISelector<T> UpdateValues(ImmutableList<T> newValues);
        ISelector<T> ReceiveInterrupt(object caller);
    }
}