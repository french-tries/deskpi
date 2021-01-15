using System;
using System.Collections.Immutable;
using System.Linq;
using Optional;
using piCommon;

namespace deskpi
{
    public class OcarinaSelector : DeskPiMode
    {
        private readonly Trie<Note, Func<DeskPiMode, DeskPiMode>> songTrie;
        private readonly ImmutableDictionary<KeyId, Note> keyToNote;
        private readonly Func<DeskPiMode, DeskPiMode> createIntroMode;
        private readonly ImmutableList<Note> receivedNotes = ImmutableList<Note>.Empty;

        public OcarinaSelector(ModeData data, 
            ImmutableDictionary<ModeId, ModeData> modesData,
            ImmutableDictionary<ModeId, Func<DeskPiMode, DeskPiMode>> modes,
            ImmutableDictionary<KeyId, Note> keyToNote,
            Func<DeskPiMode, DeskPiMode> createIntroMode)
            : base(() => new OcarinaSelector(data, modesData, modes, keyToNote, createIntroMode), data)
        {
            CheckOrphans(modesData, modes);

            this.songTrie = new Trie<Note, Func<DeskPiMode, DeskPiMode>>();

            foreach (var entry in modesData)
            {
                if (modes.ContainsKey(entry.Key))
                {
                    songTrie = songTrie.Insert(entry.Value.Song.Notes, modes[entry.Key]);
                }
            }

            this.keyToNote = keyToNote;
            this.createIntroMode = createIntroMode;
            this.receivedNotes = ImmutableList<Note>.Empty;
        }

        private OcarinaSelector(OcarinaSelector source,
            Trie<Note, Func<DeskPiMode, DeskPiMode>> songTrie = null,
            ImmutableDictionary<KeyId, Note> keyToNote = null,
            Func<DeskPiMode, DeskPiMode> createIntroMode = null,
            ImmutableList<Note> receivedNotes = null) : base(source)
        {
            this.songTrie = songTrie ?? source.songTrie;
            this.keyToNote = keyToNote ?? source.keyToNote;
            this.createIntroMode = createIntroMode ?? source.createIntroMode;
            this.receivedNotes = receivedNotes ?? source.receivedNotes;
        }

        protected override DeskPiMode ReceiveKeyImpl(KeyId key)
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

            Option<Func<DeskPiMode, DeskPiMode>> modeN = songTrie.Find(newNotes);

            return modeN.Match(
                (arg) =>
                {
                    return createIntroMode(arg(
                        new OcarinaSelector(this, receivedNotes: ImmutableList<Note>.Empty)));
                },
                () =>
                {
                    return new OcarinaSelector(this, receivedNotes: newNotes);
                });
        }

        public override uint? NextTick(uint currentTime) => null;

        public override DeskPiMode Tick(uint currentTicks) => this;

        public override ImmutableList<(string, uint)> Text {
            get {
                return DeskPiUtils.StringToText(receivedNotes.Aggregate("",
                    (text, note) =>  Song.NoteToChar(note) + text));
            }
        }

        private static void CheckOrphans(ImmutableDictionary<ModeId, ModeData> modesData,
            ImmutableDictionary<ModeId, Func<DeskPiMode, DeskPiMode>> modes)
        {
            var modesDataId = from data in modesData
                              select data.Key;
            var modesId = from entry in modes
                          select entry.Key;
            var orphaned = modesDataId.Except(modesId);

            foreach (var orphan in orphaned)
            {
                Console.WriteLine($"Orphaned mode with ID {orphan}");
            }
        }
    }
}
