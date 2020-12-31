using System.Collections.Immutable;

namespace immutableSsd.test.stubs
{
    public class SelectorStub<T> : ISelector<T>
    {
        public ImmutableList<T> GetSelected() => text;

        public ISelector<T> UpdateValues(ImmutableList<T> newValues)
        {
            this.text = newValues;
            return this;
        }

        public ISelector<T> ReceiveInterrupt(object caller)
        {
            if (CreateNew)
            {
                newInstance.text = text;
                return newInstance;
            }
            return this;
        }

        public bool CreateNew { get; set; }

        private ImmutableList<T> text;

        private static SelectorStub<T> newInstance = new SelectorStub<T>();
    }
}
