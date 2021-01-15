using System;
using System.Collections.Immutable;

namespace deskpi
{
    public class DummyMode : DeskPiMode
    {
        public DummyMode(Func<DeskPiMode> buildSelector, Song song) : base(buildSelector)
        {
            this.song = song;
        }

        public override ImmutableList<(string, uint)> Text => DeskPiUtils.StringToText(song.Name);

        protected override DeskPiMode ReceiveKeyImpl(KeyId key) => this;

        public override uint? NextTick(uint currentTime) => null;

        public override DeskPiMode Tick(uint currentTicks) => this;

        private readonly Song song;
    }
}
