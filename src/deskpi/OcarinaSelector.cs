using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Optional;
using piCommon;

namespace deskpi
{
    public enum ModeId
    {
        Selector, Dummy1, Dummy2, Help, Dummy4, Time, Dummy6, Dummy7,
        Dummy8, Dummy9, Dummy10, Dummy11, Dummy12, Dummy13
    }

    public class OcarinaSelector : IDeskPiMode
    {
        private readonly Trie<Note, ModeId> songTrie;
        private readonly ImmutableDictionary<KeyId, Note> keyToNote;
        private readonly ImmutableList<Note> receivedNotes = ImmutableList<Note>.Empty;

        private readonly ModeId currentMode;
        private readonly ImmutableDictionary<ModeId, IDeskPiMode> innerModes;


        public OcarinaSelector(ImmutableDictionary<ModeId, ModeData> modesData,
            ImmutableDictionary<ModeId, IDeskPiMode> modes,
            ImmutableDictionary<KeyId, Note> keyToNote,
            ModeId defaultMode)
        {
            CheckOrphans(modesData, modes);

            this.songTrie = new Trie<Note, ModeId>();

            foreach (var entry in modesData)
            {
                songTrie = songTrie.Insert(entry.Value.Song.Notes, entry.Key);
            }

            this.keyToNote = keyToNote;
            this.receivedNotes = ImmutableList<Note>.Empty;
            this.currentMode = defaultMode;
            this.innerModes = modes;
        }

        private OcarinaSelector(OcarinaSelector source, ModeId? mode = null,
            Trie<Note, ModeId> songTrie = null, ImmutableDictionary<KeyId, Note> keyToNote = null,
            ImmutableList<Note> receivedNotes = null,
            ImmutableDictionary<ModeId, IDeskPiMode> innerModes = null)
        {
            this.songTrie = songTrie ?? source.songTrie;
            this.keyToNote = keyToNote ?? source.keyToNote;
            this.receivedNotes = receivedNotes ?? source.receivedNotes;
            this.currentMode = mode ?? source.currentMode;
            this.innerModes = innerModes ?? source.innerModes;
        }

        public IDeskPiMode ReceiveKey(KeyId key)
        {
            if (currentMode == ModeId.Selector)
            {
                return ReceiveSelectionKey(key);
            }
            if (key == KeyId.F)
            {
                return new OcarinaSelector(this, ModeId.Selector);
            }
            return new OcarinaSelector(this,
                innerModes: innerModes.SetItem(currentMode, innerModes[currentMode].ReceiveKey(key)));
        }

        public uint? NextTick(uint currentTime) =>
            innerModes.Values.Min((button) => button.NextTick(currentTime));

        public IDeskPiMode Tick(uint currentTime)
        {
            var updated = new List<KeyValuePair<ModeId, IDeskPiMode>>();

            foreach (var entry in innerModes)
            {
                var modeN = entry.Value.Tick(currentTime);
                if (modeN != entry.Value)
                {
                    updated.Add(new KeyValuePair<ModeId, IDeskPiMode>(entry.Key, modeN));
                }
            }

            if (!updated.Any())
            {
                return this;
            }
            return new OcarinaSelector(this, innerModes: innerModes.SetItems(updated));
        }

        public ImmutableList<(string, uint)> Text {
            get {
                if (currentMode == ModeId.Selector)
                {
                    return DeskPiUtils.StringToText(receivedNotes.Aggregate("",
            (text, note) =>  Song.NoteToChar(note) + text));
                }
                return innerModes[currentMode].Text;
            }
        }

        private IDeskPiMode ReceiveSelectionKey(KeyId key)
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

        private static void CheckOrphans(ImmutableDictionary<ModeId, ModeData> modesData,
            ImmutableDictionary<ModeId, IDeskPiMode> modes)
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
