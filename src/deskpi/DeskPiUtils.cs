using System;
using System.Collections.Immutable;

namespace deskpi
{
    public static class DeskPiUtils
    {
        public static ImmutableList<(string, uint)> StringToText(string text) =>
            ImmutableList<(string, uint)>.Empty.Add((text, 0));
    }
}
