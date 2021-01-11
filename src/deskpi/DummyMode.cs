using System;
using System.Collections.Immutable;

namespace deskpi
{
    public class DummyMode : IDeskPiMode
    {
        public DummyMode(Song song)
        {
            this.song = song;
        }

        public ImmutableList<(string, uint)> Text => DeskPiUtils.StringToText(song.Name);

        public IDeskPiMode ReceiveKey(KeyId key) => this;

        public uint? NextTick(uint currentTime) => null;

        public IDeskPiMode Tick(uint currentTime) => this;

        private readonly Song song;
    }
}
