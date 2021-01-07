using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace deskpi.ocarinaSelector
{
    public enum Note { Left, Right, Bottom, Top, A }

    public class Song
    {
        public Song(string Name, ImmutableList<Note> Notes)
        {
            this.Name = Name;
            this.Notes = Notes;
        }

        public string Name { get; }
        public ImmutableList<Note> Notes { get; }

        public static Song ZeldasLullaby = new Song("Zelda's Lullaby", new List<Note>
        {Note.Left, Note.Top, Note.Right, Note.Left, Note.Top, Note.Right}
        .ToImmutableList());

        public static Song EponasSong = new Song("Epona's Song", new List<Note>
        {Note.Top, Note.Left, Note.Right, Note.Top, Note.Left, Note.Right}
        .ToImmutableList());

        public static Song SariasSong = new Song("Saria's Song", new List<Note>
        {Note.Bottom, Note.Right, Note.Left, Note.Bottom, Note.Right, Note.Left}
        .ToImmutableList());

        public static Song SunsSong = new Song("Sun's Song", new List<Note>
        {Note.Right, Note.Bottom, Note.Top, Note.Right, Note.Bottom, Note.Top}
        .ToImmutableList());

        public static Song SongOfTime = new Song("Song Of Time", new List<Note>
        {Note.Right, Note.A, Note.Bottom, Note.Right, Note.A, Note.Bottom}
        .ToImmutableList());

        public static Song SongOfStorms = new Song("Song Of Storms", new List<Note>
        {Note.A, Note.Bottom, Note.Top, Note.A, Note.Bottom, Note.Top}
        .ToImmutableList());

        public static Song MinuetOfForest = new Song("Minuet Of Forest", new List<Note>
        {Note.A, Note.Top, Note.Left, Note.Right, Note.Left, Note.Right}
        .ToImmutableList());

        public static Song BoleroOfFire = new Song("Bolero Of Fire", new List<Note>
        {Note.Bottom, Note.A, Note.Bottom, Note.A, Note.Right, Note.Bottom, Note.Right, Note.Bottom}
        .ToImmutableList());

        public static Song SerenadeOfWater = new Song("Serenade Of Water", new List<Note>
        {Note.A, Note.Bottom, Note.Right, Note.Right, Note.Left}
        .ToImmutableList());

        public static Song NocturneOfShadow = new Song("Nocturne Of Shadow", new List<Note>
        {Note.Left, Note.Right, Note.Right, Note.A, Note.Left, Note.Right, Note.Bottom}
        .ToImmutableList());

        public static Song RequiemOfSpirit = new Song("Requiem Of Spirit", new List<Note>
        {Note.A, Note.Bottom, Note.A, Note.Right, Note.Bottom, Note.A}
        .ToImmutableList());

        public static Song PreludeOfLight = new Song("Prelude Of Light", new List<Note>
        {Note.Top, Note.Right, Note.Top, Note.Right, Note.Left, Note.Top}
        .ToImmutableList());

        public static Song ScarecrowsSong = new Song("Scarecrow's Song", new List<Note>
        {Note.Left, Note.Left, Note.Left, Note.Left, Note.Left, Note.Left, Note.Left, Note.Left}
        .ToImmutableList());

        public static char NoteToChar(Note note)
        {
            switch (note)
            {
                case Note.Left: return '\u02C2';
                case Note.Right: return '\u02C3';
                case Note.Top: return '\u02C4';
                case Note.Bottom: return '\u02C5';
                case Note.A: return 'A';
            }
            return ' ';
        }

        public static string NotesToString(ImmutableList<Note> Notes) => Notes.Aggregate("",
            (text, note) => text + NoteToChar(note));
    }
}
