using System;
using System.Collections.Immutable;

namespace deskpi
{
    public abstract class DeskPiMode
    {
        private readonly Func<DeskPiMode> buildSelector;

        protected DeskPiMode(Func<DeskPiMode> buildSelector, ModeData data)
        {
            this.buildSelector = buildSelector;
            this.Data = data;
        }

        protected DeskPiMode(DeskPiMode source)
        {
            this.buildSelector = source.buildSelector;
        }

        public abstract uint? NextTick(uint currentTime);
        public abstract ImmutableList<(string, uint)> Text { get; }
        public abstract DeskPiMode Tick(uint currentTicks);

        public DeskPiMode ReceiveKey(KeyId key)
        {
            if (key == KeyId.F)
            {
                return buildSelector();
            }
            return ReceiveKeyImpl(key);
        }

        protected abstract DeskPiMode ReceiveKeyImpl(KeyId key);

        public ModeData Data { get; }
    }
}
