using System.Collections.Immutable;
using System.Linq;
using Optional;

namespace deskpi.ocarinaSelector
{
    public enum ModeId
    {
        Selector, Dummy1, Dummy2, Help, Dummy4, Dummy5, Dummy6, Dummy7,
        Dummy8, Dummy9, Dummy10, Dummy11, Dummy12, Dummy13
    }

    public class OcarinaSelector : IDeskPiMode
    {
        public OcarinaSelector(Trie<Note, ModeId> songTrie,
            ImmutableDictionary<Key, Note> keyToNote,
            ModeId mode, ImmutableDictionary<ModeId, IDeskPiMode> innerModes)
        {
            this.songTrie = songTrie;
            this.keyToNote = keyToNote;
            this.receivedNotes = ImmutableList<Note>.Empty;
            this.mode = mode;
            this.innerModes = innerModes;
        }

        private OcarinaSelector(OcarinaSelector source, ModeId? mode = null,
            Trie<Note, ModeId> songTrie = null, ImmutableDictionary<Key, Note> keyToNote = null,
            ImmutableList<Note> receivedNotes = null,
            ImmutableDictionary<ModeId, IDeskPiMode> innerModes = null)
        {
            this.songTrie = songTrie ?? source.songTrie;
            this.keyToNote = keyToNote ?? source.keyToNote;
            this.receivedNotes = receivedNotes ?? source.receivedNotes;
            this.mode = mode ?? source.mode;
            this.innerModes = innerModes ?? source.innerModes;
        }

        public IDeskPiMode ReceiveKey(Key key)
        {
            if (mode == ModeId.Selector)
            {
                return ReceiveSelectionKey(key);
            }
            if (key == Key.F)
            {
                return new OcarinaSelector(this, ModeId.Selector);
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
            if (newNotes.Count >= 8)
            {
                newNotes = newNotes.GetRange(0, 7);
            }
            newNotes = newNotes.Insert(0, keyToNote[key]);

            Option<ModeId> modeN = songTrie.Find(newNotes);

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

        public TextValue Text {
            get {
                if (mode == ModeId.Selector)
                {
                    return new SimpleTextValue(receivedNotes.Aggregate("",
            (text, note) =>  Song.NoteToChar(note) + text));
                }
                return innerModes[mode].Text;
            }
        }

        private readonly Trie<Note, ModeId> songTrie;
        private readonly ImmutableDictionary<Key, Note> keyToNote;
        private readonly ImmutableList<Note> receivedNotes = ImmutableList<Note>.Empty;

        private readonly ModeId mode;
        private readonly ImmutableDictionary<ModeId, IDeskPiMode> innerModes;


    }
}
