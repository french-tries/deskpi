using System;
using piCommon;

namespace immutableSsd
{
    public interface ISsd : ITickable<ISsd>
    {
        uint AvailableDigits { get; }

        ISsd Write(string text);
        ISsd Write(System.Collections.Immutable.ImmutableList<(string, uint)> texts);
    }
}
