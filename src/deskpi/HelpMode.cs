using System.Collections.Immutable;
using System.Linq;
using piCommon;

namespace deskpi
{
    public class HelpMode : IDeskPiMode
    {
        private enum Field { Name, Description, HelpText, SongName, SongNotes, Max };

        public HelpMode(ImmutableDictionary<ModeId, ModeData> modes)
        {
            this.modes = ImmutableArray<ModeData>.Empty.AddRange(modes.Values);
        }

        private HelpMode(ImmutableArray<ModeData> modes,
            int currentMode, Field currentField)
        {
            this.modes = modes;
            this.currentMode = currentMode;
            this.currentField = currentField;
        }

        public ImmutableList<(string, uint)> Text {
            get
            {
                var text = "";
                switch (currentField)
                {
                    case Field.Name:
                        text = modes[currentMode].Name;
                        break;
                    case Field.Description:
                        text = modes[currentMode].Description;
                        break;
                    case Field.HelpText:
                        text = modes[currentMode].HelpText;
                        break;
                    case Field.SongName:
                        text = modes[currentMode].Song.Name;
                        break;
                    case Field.SongNotes:
                        text = Song.NotesToString(modes[currentMode].Song.Notes);
                        break;
                }
                return DeskPiUtils.StringToText(text);
            }
        }

        public IDeskPiMode ReceiveKey(KeyId key)
        {
            var result = this;
            switch (key)
            {
                case KeyId.A:
                    result = new HelpMode(modes, 
                        currentMode <= 0 ? modes.Length - 1 : currentMode - 1,
                        Field.Name);
                    break;
                case KeyId.B:
                    result = new HelpMode(modes, currentMode,
                        currentField + 1 >= Field.Max ? Field.Name : currentField + 1);
                    break;
                case KeyId.C:
                    result = new HelpMode(modes,
                        currentMode + 1 >= modes.Length ? 0 : currentMode + 1,
                        Field.Name);
                    break;
            }
            return result;
        }

        public uint? NextTick(uint currentTime) => null;

        public IDeskPiMode Tick(uint currentTime) => this;

        private readonly ImmutableArray<ModeData> modes;
        private readonly int currentMode = 0;
        private readonly Field currentField = Field.Name;
    }
}
