using System.Collections.Immutable;
using piCommon;

namespace deskpi
{
    public interface IDeskPiMode : ITickable<IDeskPiMode>
    {
        IDeskPiMode ReceiveKey(KeyId key);
        ImmutableList<(string, uint)> Text { get; }
    }
}
