using System.Collections.Immutable;

namespace immutableSsd.test.stubs
{
    public class SelectorStub<T> : ISelector<T>
    {
        public ImmutableList<T> GetSelected() => Text;

        public uint? NextTick(uint currentTime) => NextTickVal;

        public ISelector<T> Tick(uint currentTime) =>
            NewText == ImmutableList<T>.Empty ? this : new SelectorStub<T> { Text = NewText };

        public uint? NextTickVal { get; set; }

        public ImmutableList<T> Text { get; set; } = ImmutableList<T>.Empty;
        public ImmutableList<T> NewText { get; set; } = ImmutableList<T>.Empty;
    }
}
