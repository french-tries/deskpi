using System.Collections.Immutable;

namespace immutableSsd.test.stubs
{
    public class SelectorStub<T> : ISelector<T>
    {
        public SelectorStub()
            : this(ImmutableList<T>.Empty)
        {
        }

        public SelectorStub(ImmutableList<T> text)
        {
            this.Text = text;
        }

        public ImmutableList<T> GetSelected() => Text;

        public ISelector<T> ReceiveInterrupt(object caller)
        {
            if (CreateNew)
            {
                newInstance.Text = Text;
                return newInstance;
            }
            return this;
        }

        public bool CreateNew { get; set; }

        public ImmutableList<T> Text { get; set; }

        private static SelectorStub<T> newInstance = new SelectorStub<T>();
    }
}
