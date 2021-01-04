using System.Collections.Immutable;
using piCommon;

namespace immutableSsd
{
    // todo ishh...
    public interface ISsdWriter<TPARAM>
    {
        uint AvailableDigits { get; }
        ISsdWriter<TPARAM> Write(TPARAM values);
        ISsdWriter<TPARAM> ReceiveInterrupt(object caller);
    }
}