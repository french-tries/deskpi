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
            .Add(("Help mode", 4)).Add(("Add content...", 4)));

        public IDeskPiMode ReceiveKey(Key key)
        {
            return this;
        }
    }
}
