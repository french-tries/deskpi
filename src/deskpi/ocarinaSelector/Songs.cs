using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace deskpi.src.deskpi.ocarinaSelector
{
    public static class Songs
    {
        public enum Note { Left, Right, Bottom, Top, A }

        public static ImmutableList<Note> ZeldasLullaby = new List<Note>
        {Note.Left, Note.Top, Note.Right, Note.Left, Note.Top, Note.Right}
        .ToImmutableList();

        public static ImmutableList<Note> EponasSong = new List<Note>
        {Note.Top, Note.Left, Note.Right, Note.Top, Note.Left, Note.Right}
        .ToImmutableList();

        public static ImmutableList<Note> SariasSong = new List<Note>
        {Note.Bottom, Note.Right, Note.Left, Note.Bottom, Note.Right, Note.Left}
        .ToImmutableList();

        public static ImmutableList<Note> SunsSong = new List<Note>
        {Note.Right, Note.Bottom, Note.Top, Note.Right, Note.Bottom, Note.Top}
        .ToImmutableList();

        public static ImmutableList<Note> SongOfTime = new List<Note>
        {Note.Right, Note.A, Note.Bottom, Note.Right, Note.A, Note.Bottom}
        .ToImmutableList();

        public static ImmutableList<Note> SongOfStorms = new List<Note>
        {Note.A, Note.Bottom, Note.Top, Note.A, Note.Bottom, Note.Top}
        .ToImmutableList();

        public static ImmutableList<Note> MinuetOfForest = new List<Note>
        {Note.A, Note.Top, Note.Left, Note.Right, Note.Left, Note.Right}
        .ToImmutableList();

        public static ImmutableList<Note> BoleroOfFire = new List<Note>
        {Note.Bottom, Note.A, Note.Bottom, Note.A, Note.Right, Note.Bottom, Note.Right, Note.Bottom}
        .ToImmutableList();

        public static ImmutableList<Note> SerenadeOfWater = new List<Note>
        {Note.A, Note.Bottom, Note.Right, Note.Right, Note.Left}
        .ToImmutableList();

        public static ImmutableList<Note> NocturneOfShadow = new List<Note>
        {Note.Left, Note.Right, Note.Right, Note.A, Note.Left, Note.Right, Note.Bottom}
        .ToImmutableList();

        public static ImmutableList<Note> RequiemOfSpirit = new List<Note>
        {Note.A, Note.Bottom, Note.A, Note.Right, Note.Bottom, Note.A}
        .ToImmutableList();

        public static ImmutableList<Note> PreludeOfLight = new List<Note>
        {Note.Top, Note.Right, Note.Top, Note.Right, Note.Left, Note.Top}
        .ToImmutableList();

        public static ImmutableList<Note> ScarecrowsSong = new List<Note>
        {Note.Left, Note.Left, Note.Left, Note.Left, Note.Left, Note.Left, Note.Left, Note.Left}
        .ToImmutableList();
    }
}
