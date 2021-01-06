using System.Collections.Immutable;
using piCommon;

namespace immutableSsd
{
    public interface ISelector<T> : ITickable<ISelector<T>>
    {
        ImmutableList<T> GetSelected();
    }
}