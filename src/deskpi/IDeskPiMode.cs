using System.Collections.Immutable;

namespace deskpi
{
    public interface TextValue
    {
    }

    public class SimpleTextValue : TextValue
    {
        public SimpleTextValue(string Value)
        {
            this.Value = Value;
        }

        public string Value { get; }
    }

    public class ComplexTextValue : TextValue
    {
        public ComplexTextValue(ImmutableList<(string, uint)> Values)
        {
            this.Values = Values;
        }

        public ImmutableList<(string, uint)> Values { get; }
    }

    public interface IDeskPiMode
    {
        IDeskPiMode ReceiveKey(Key key);
        TextValue Text { get; }
    }
}
