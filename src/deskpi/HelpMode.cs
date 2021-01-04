using System;
using System.Collections.Immutable;

namespace deskpi
{
    public class HelpMode : IDeskPiMode
    {
        public HelpMode()
        {
        }

        TextValue IDeskPiMode.Text => new ComplexTextValue(
            ImmutableList<(string, uint)>.Empty
            .Add(("Bye", 4)).Add(("Bye", 4)));

        public IDeskPiMode ReceiveKey(Key key)
        {
            return this;
        }
    }
}
