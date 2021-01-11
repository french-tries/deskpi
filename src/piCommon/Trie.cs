using System.Collections.Immutable;
using Optional;

namespace piCommon
{
    public class Trie<K, T>
    {
        private class Node
        {
            public Node() : this(Option.None<T>(), null)
            {
            }

            private Node(Option<T> item, ImmutableDictionary<K, Node> children)
            {
                this.item = item;
                this.children = children ?? ImmutableDictionary<K, Node>.Empty;            }

            public Node Insert(ImmutableList<K> name, T itemN)
            {
                if (name == ImmutableList<K>.Empty)
                {
                    return new Node(itemN.SomeNotNull(), children);
                }
                if (item.HasValue)
                {
                    return this;
                }

                var newNode = children.ContainsKey(name[0]) ?
                    children[name[0]] : new Node();

                newNode = newNode.Insert(name.RemoveAt(0), itemN);

                if (children.ContainsKey(name[0]) && newNode == children[name[0]])
                {
                    return this;
                }
                return new Node(item, children.SetItem(name[0], newNode));
            }

            public Option<T> Find(ImmutableList<K> name)
            {
                if (name == ImmutableList<K>.Empty || item.HasValue)
                {
                    return item;
                }
                if (!children.ContainsKey(name[0]))
                {
                    return Option.None<T>();
                }
                return children[name[0]].Find(name.RemoveAt(0));
            }

            private readonly Option<T> item;
            private readonly ImmutableDictionary<K, Node> children = 
                ImmutableDictionary<K, Node>.Empty;
        }

        public Trie() : this(new Node())
        {
        }

        private Trie(Node top)
        {
            this.top = top;
        }

        public Trie<K,T> Insert(ImmutableList<K> name, T itemN)
        {
            var newTop = top.Insert(name.Reverse(), itemN);
            return newTop == top ? this : new Trie<K,T>(newTop); ;
        }

        public Option<T> Find(ImmutableList<K> name)
        {
            return top.Find(name);
        }

        private readonly Node top = new Node();
    }
}
