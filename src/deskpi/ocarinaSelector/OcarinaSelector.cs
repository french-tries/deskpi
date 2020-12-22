using System;
using System.Collections.Immutable;
using Optional;
using static deskpi.src.deskpi.ocarinaSelector.Songs;

namespace deskpi.src.deskpi.ocarinaSelector
{
    public class OcarinaSelector
    {
        public OcarinaSelector(SongsTrie songTrie) : this(songTrie, null)
        {
        }

        private OcarinaSelector(SongsTrie songTrie, ImmutableList<Note> receivedNotes)
        {
            this.songTrie = songTrie;
            this.receivedNotes = receivedNotes ?? ImmutableList<Note>.Empty;
        }

        public OcarinaSelector ReceiveNote(Note note)
        {
            var newNotes = receivedNotes;
            if (receivedNotes.Count >= 8)
            {
                newNotes = receivedNotes.GetRange(0, 7);
            } 
            newNotes = receivedNotes.Insert(0, note);

            Option<Action> action = songTrie.Find(newNotes);

            return action.Match(
                (arg) =>
                {
                    arg();
                    return new OcarinaSelector(songTrie, ImmutableList<Note>.Empty);
                },
                () =>
                {
                    return new OcarinaSelector(songTrie, newNotes);
                });
        }


        private readonly SongsTrie songTrie;
        private readonly ImmutableList<Note> receivedNotes = ImmutableList<Note>.Empty;
    }
}
