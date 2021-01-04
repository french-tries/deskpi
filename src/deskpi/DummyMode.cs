using System;
using deskpi.ocarinaSelector;

namespace deskpi
{
    public class DummyMode : IDeskPiMode
    {
        public DummyMode(Song song)
        {
            this.song = song;
        }

        public TextValue Text => new SimpleTextValue(song.Name);

        public IDeskPiMode ReceiveKey(Key key) => this;

        private readonly Song song;
    }
}
