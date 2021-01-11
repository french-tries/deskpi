using System;

namespace deskpi
{
    public class ModeData
    {
        public ModeData(string Name, string Description,
            string HelpText, Song Song)
        {
            this.Name = Name;
            this.Description = Description;
            this.HelpText = HelpText;
            this.Song = Song;
        }

        public string Name { get; }
        public string Description { get; }
        public string HelpText { get; }
        public Song Song { get; }
    }
}
