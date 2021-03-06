﻿using System;
using System.Collections.Immutable;
using System.Linq;

namespace deskpi
{
    public class HelpMode : DeskPiMode
    {
        private enum Field { Name, Description, HelpText, SongName, SongNotes, Max };

        public HelpMode(Func<DeskPiMode> buildSelector, ModeData data,
            ImmutableDictionary<ModeId, ModeData> modes,
            ModeId defaultMode = ModeId.Help) : base(buildSelector, data)
        {
            this.modes = modes;

            var keys = from id in modes.Keys
                       orderby (int)id
                       select id;

            ids = ImmutableArray<ModeId>.Empty.AddRange(keys);
            currentMode = ids.IndexOf(defaultMode);
            if (currentMode == -1)
            {
                currentMode = 0;
            }
        }

        private HelpMode(HelpMode source, 
            ImmutableDictionary<ModeId, ModeData> modes = null,
            ImmutableArray<ModeId>? ids = null, int? currentMode = null, 
            Field? currentField = null) : base(source)
        {
            this.modes = modes ?? source.modes;
            this.ids = ids ?? source.ids;
            this.currentMode = currentMode ?? source.currentMode;
            this.currentField = currentField ?? source.currentField;
        }

        public override ImmutableList<(string, uint)> Text {
            get
            {
                var text = "";
                switch (currentField)
                {
                    case Field.Name:
                        text = modes[ids[currentMode]].Name;
                        break;
                    case Field.Description:
                        text = modes[ids[currentMode]].Description;
                        break;
                    case Field.HelpText:
                        text = modes[ids[currentMode]].HelpText;
                        break;
                    case Field.SongName:
                        text = modes[ids[currentMode]].Song.Name;
                        break;
                    case Field.SongNotes:
                        text = Song.NotesToString(modes[ids[currentMode]].Song.Notes);
                        break;
                }
                return DeskPiUtils.StringToText(text);
            }
        }

        protected override DeskPiMode ReceiveKeyImpl(KeyId key)
        {
            var result = this;
            switch (key)
            {
                case KeyId.A:
                    result = new HelpMode(this,
                        currentMode: currentMode <= 0 ? ids.Length - 1 : currentMode - 1,
                        currentField: Field.Name);
                    break;
                case KeyId.B:
                    result = new HelpMode(this, currentField:
                        currentField + 1 >= Field.Max ? Field.Name : currentField + 1);
                    break;
                case KeyId.C:
                    result = new HelpMode(this,
                        currentMode: currentMode + 1 >= ids.Length ? 0 : currentMode + 1,
                        currentField: Field.Name);
                    break;
            }
            return result;
        }

        public override uint? NextTick(uint currentTime) => null;

        public override DeskPiMode Tick(uint currentTicks) => this;

        private readonly ImmutableDictionary<ModeId, ModeData> modes;
        private readonly ImmutableArray<ModeId> ids;
        private readonly int currentMode = 0;
        private readonly Field currentField = Field.Name;
    }
}
