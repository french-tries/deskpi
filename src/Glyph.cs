using System;
using System.Collections.Immutable;

// TODO this vs segments char directly?
namespace immutableSsd.src
{
    public class Glyph : IEquatable<Glyph>
    {
        public Glyph(char character, bool dot)
        {
            Character = character;
            Dot = dot;
        }

        public char Character { get; }

        public bool Dot { get; }

        public static Glyph Empty { get; } = new Glyph(' ', false);

        public bool Equals(Glyph other)
        {
            if (other == null) return false;
            return Character == other.Character &&
                Dot == other.Dot;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Glyph);
        }

        public override int GetHashCode() =>
            (Character, Dot).GetHashCode();

        public static ImmutableList<Glyph> FromString(string text)
        {
            var result = ImmutableList<Glyph>.Empty;

            int pos = 0;

            while(pos < text.Length)
            {
                if (text[pos] == '.')
                {
                    result = result.Add(new Glyph(' ', true));
                    pos++;
                }
                else
                {
                    if (pos+1 >= text.Length || text[pos+1] != '.')
                    {
                        result = result.Add(new Glyph(text[pos], false));
                        pos++;
                    }
                    else
                    {
                        result = result.Add(new Glyph(text[pos], true));
                        pos += 2;
                    }
                }
            }

            return result;
        }
    }
}
