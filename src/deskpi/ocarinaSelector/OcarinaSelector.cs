using System.Collections.Immutable;
using Optional;

namespace deskpi.ocarinaSelector
{
    public enum Mode
    {
        Selector, Dummy1, Dummy2, Dummy3, Dummy4, Dummy5, Dummy6, Dummy7,
        Dummy8, Dummy9, Dummy10, Dummy11, Dummy12, Dummy13
    }

    public class OcarinaSelector : IDeskPiMode
    {
        public OcarinaSelector(Trie<Note, Mode> songTrie,
            ImmutableDictionary<Key, Note> keyToNote,
            Mode mode, ImmutableDictionary<Mode, IDeskPiMode> innerModes)
        {
            this.songTrie = songTrie;
            this.keyToNote = keyToNote;
            this.receivedNotes = ImmutableList<Note>.Empty;
            this.mode = mode;
            this.innerModes = innerModes;
        }

        private OcarinaSelector(OcarinaSelector source, Mode? mode = null,
            Trie<Note, Mode> songTrie = null, ImmutableDictionary<Key, Note> keyToNote = null,
            ImmutableList<Note> receivedNotes = null,
            ImmutableDictionary<Mode, IDeskPiMode> innerModes = null)
        {
            this.songTrie = songTrie ?? source.songTrie;
            this.keyToNote = keyToNote ?? source.keyToNote;
            this.receivedNotes = receivedNotes ?? source.receivedNotes;
            this.mode = mode ?? source.mode;
            this.innerModes = innerModes ?? source.innerModes;
        }

        public IDeskPiMode ReceiveKey(Key key)
        {
            if (mode == Mode.Selector)
            {
                return ReceiveSelectionKey(key);
            }
            if (key == Key.F)
            {
                return new OcarinaSelector(this, Mode.Selector);
            }
            return new OcarinaSelector(this,
                innerModes: innerModes.SetItem(mode, innerModes[mode].ReceiveKey(key)));
        }

        private IDeskPiMode ReceiveSelectionKey(Key key)
        {
            if (!keyToNote.ContainsKey(key))
            {
                return this;
            }
            var newNotes = receivedNotes;
            if (receivedNotes.Count >= 8)
            {
                newNotes = receivedNotes.GetRange(0, 7);
            }
            newNotes = receivedNotes.Insert(0, keyToNote[key]);

            Option<Mode> modeN = songTrie.Find(newNotes);

            return modeN.Match(
                (arg) =>
                {
                    return new OcarinaSelector(this, arg,
                        receivedNotes: ImmutableList<Note>.Empty);
                },
                () =>
                {
                    return new OcarinaSelector(this, receivedNotes: newNotes);
                });
        }

        public string Text {
            get {
                if (mode == Mode.Selector) return "Hello world please work...";
                return innerModes[mode].Text;
            }
        }

        private readonly Trie<Note, Mode> songTrie;
        private readonly ImmutableDictionary<Key, Note> keyToNote;
        private readonly ImmutableList<Note> receivedNotes = ImmutableList<Note>.Empty;

        private readonly Mode mode;
        private readonly ImmutableDictionary<Mode, IDeskPiMode> innerModes;
    }
}
