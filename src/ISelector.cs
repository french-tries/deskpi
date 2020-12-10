using System.Collections.Immutable;

namespace immutableSsd.src
{
    public interface ISelector<T> : IInterruptReceiver<ISelector<T>>
    {
        ImmutableList<T> GetSelected();

        ISelector<T> UpdateValues(ImmutableList<T> newValues);
    }
}