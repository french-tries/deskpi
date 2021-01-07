using System.Collections.Immutable;
using System.Linq;
using deskpi.ocarinaSelector;

namespace deskpi
{
    public class HelpMode : IDeskPiMode
    {
        private enum Field { Name, Description, HelpText, SongName, SongNotes, Max };

        public HelpMode(ImmutableArray<ModeEntry<ModeId>> modes)
        {
            this.modes = modes;
        }

        private HelpMode(ImmutableArray<ModeEntry<ModeId>> modes,
            int currentMode, Field currentField)
        {
            this.modes = modes;
            this.currentMode = currentMode;
            this.currentField = currentField;
        }

        TextValue IDeskPiMode.Text {
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
                return new SimpleTextValue(text);
            }
        }

        public IDeskPiMode ReceiveKey(Key key)
        {
            var result = this;
            switch (key)
            {
                case Key.A:
                    result = new HelpMode(modes, 
                        currentMode <= 0 ? modes.Length - 1 : currentMode - 1,
                        Field.Name);
                    break;
                case Key.B:
                    result = new HelpMode(modes, currentMode,
                        currentField + 1 >= Field.Max ? Field.Name : currentField + 1);
                    break;
                case Key.C:
                    result = new HelpMode(modes,
                        currentMode + 1 >= modes.Length ? 0 : currentMode + 1,
                        Field.Name);
                    break;
            }
            return result;
        }

        private readonly ImmutableArray<ModeEntry<ModeId>> modes;
        private readonly int currentMode = 0;
        private readonly Field currentField = Field.Name;
    }
}
