using System;
using System.Collections.Immutable;
using Optional;
using static deskpi.src.deskpi.ocarinaSelector.Songs;

namespace deskpi.src.deskpi.ocarinaSelector
{
    public class SongsTrie
    {
        private class Node
        {
            public Node() : this(Option.None<Action>(), null)
            {
            }

            private Node(Option<Action> action, ImmutableDictionary<Note, Node> children)
            {
                this.action = action;
                this.children = children ?? ImmutableDictionary<Note, Node>.Empty;            }

            public Node Insert(ImmutableList<Note> song, Action newAction)
            {
                if (song == ImmutableList<Note>.Empty)
                {
                    return new Node(newAction.SomeNotNull(), children);
                }
                if (action.HasValue)
                {
                    return this;
                }

                var newNode = children.ContainsKey(song[0]) ?
                    children[song[0]] : new Node();

                newNode = newNode.Insert(song.RemoveAt(0), newAction);

                if (children.ContainsKey(song[0]) && newNode == children[song[0]])
                {
                    return this;
                }
                return new Node(action, children.SetItem(song[0], newNode));
            }

            public Option<Action> Find(ImmutableList<Note> song)
            {
                if (song == ImmutableList<Note>.Empty || action.HasValue)
                {
                    return action;
                }
                if (!children.ContainsKey(song[0]))
                {
                    return Option.None<Action>();
                }
                return children[song[0]].Find(song.RemoveAt(0));
            }

            private readonly Option<Action> action;
            private readonly ImmutableDictionary<Note, Node> children = 
                ImmutableDictionary<Note, Node>.Empty;
        }

        public SongsTrie() : this(new Node())
        {
        }

        private SongsTrie(Node top)
        {
            this.top = top;
        }

        public SongsTrie Insert(ImmutableList<Note> song, Action newAction)
        {
            var newTop = top.Insert(song.Reverse(), newAction);
            return newTop == top ? this : new SongsTrie(newTop); ;
        }

        public Option<Action> Find(ImmutableList<Note> song)
        {
            return top.Find(song);
        }

        private readonly Node top = new Node();
    }
}
