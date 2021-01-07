using System;
using deskpi.ocarinaSelector;

namespace deskpi
{
    public class ModeEntry<ID>
    {
        public ModeEntry(ID Mode, string Name, string Description,
            string HelpText, Song Song)
        {
            this.Mode = Mode;
            this.Name = Name;
            this.Description = Description;
            this.HelpText = HelpText;
            this.Song = Song;
        }

        public ID Mode { get; }
        public string Name { get; }
        public string Description { get; }
        public string HelpText { get; }
        public Song Song { get; }
    }
}
