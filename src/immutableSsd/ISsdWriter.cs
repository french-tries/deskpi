using piCommon;

namespace immutableSsd
{
    public interface ISsdWriter<TPARAM>
    {
        int AvailableDigits { get; }
        ISsdWriter<TPARAM> Write(TPARAM values);
        ISsdWriter<TPARAM> ReceiveInterrupt(object caller);
    }
}