using System;
using System.Collections.Immutable;

namespace deskpi
{
    public enum ModeId
    {
        Selector, Dummy1, Dummy2, Help, Dummy4, Time, Dummy6, Dummy7,
        Dummy8, Dummy9, Dummy10, Dummy11, Dummy12, Dummy13
    }

    public abstract class DeskPiMode
    {
        protected DeskPiMode(Func<DeskPiMode> buildSelector)
        {
            this.buildSelector = buildSelector;
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

        public ModeId Id { get; }

        private readonly Func<DeskPiMode> buildSelector;
    }
}
