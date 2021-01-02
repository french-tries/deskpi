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

        public string Text => song.Name;

        public IDeskPiMode ReceiveKey(Key key) => this;

        private readonly Song song;
    }
}
