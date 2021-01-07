using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using deskpi.ocarinaSelector;
using immutableSsd;
using piCommon;

namespace deskpi
{
    public class PinValueChangeEvent
    {
        public Button Button { get; }

        public PinValueChangeEvent(Button Button)
        {
            this.Button = Button;
        }
    }

    // todo separate to be able to do concurrency?
    public class DeskPi : ITickable<DeskPi>
    {
        public DeskPi(GpioHandler gpioHandler, Action<object> pushEvent)
        {
            stringWriter = ImmutableSsd.CreateMax7219BackedDisplay(gpioHandler,
                scrollDelay: 500, endsScrollDelay: 1000);

            var modeEntries = new List<ModeEntry<ModeId>>
            {
                new ModeEntry<ModeId>(ModeId.Help, "About deskPi - Press middle",
                    "What follows is how to use the thing - press middle",
                    "Press the the top and bottom buttons to get info about " +
                    "the different modes. Press the middle button to get different " +
                    "info about the current mode. Press the top and bottom buttons " +
                    "to play a song to reach a new mode. The available notes: " +
                    "A - Bottom and middle, \u02C5 - Bottom, \u02C3 - Middle, " +
                    "\u02C2 - top, \u02C4 - Middle and top.",
                    Song.SariasSong),
                new ModeEntry<ModeId>(ModeId.Dummy1, Song.ZeldasLullaby.Name,
                    "Description", "Help Text", Song.ZeldasLullaby),
                new ModeEntry<ModeId>(ModeId.Dummy2, Song.EponasSong.Name,
                    "Description", "Help Text", Song.EponasSong),
                new ModeEntry<ModeId>(ModeId.Dummy4, Song.SunsSong.Name,
                    "Description", "Help Text", Song.SunsSong),
                new ModeEntry<ModeId>(ModeId.Dummy5, Song.SongOfTime.Name,
                    "Description", "Help Text", Song.SongOfTime),
                new ModeEntry<ModeId>(ModeId.Dummy6, Song.SongOfStorms.Name,
                    "Description", "Help Text", Song.SongOfStorms),
                new ModeEntry<ModeId>(ModeId.Dummy7, Song.MinuetOfForest.Name,
                    "Description", "Help Text", Song.MinuetOfForest),
                new ModeEntry<ModeId>(ModeId.Dummy8, Song.BoleroOfFire.Name,
                    "Description", "Help Text", Song.BoleroOfFire),
                new ModeEntry<ModeId>(ModeId.Dummy9, Song.SerenadeOfWater.Name,
                    "Description", "Help Text", Song.SerenadeOfWater),
                new ModeEntry<ModeId>(ModeId.Dummy10, Song.NocturneOfShadow.Name,
                    "Description", "Help Text", Song.NocturneOfShadow),
                new ModeEntry<ModeId>(ModeId.Dummy11, Song.RequiemOfSpirit.Name,
                    "Description", "Help Text", Song.RequiemOfSpirit),
                new ModeEntry<ModeId>(ModeId.Dummy12, Song.PreludeOfLight.Name,
                    "Description", "Help Text", Song.PreludeOfLight),
                new ModeEntry<ModeId>(ModeId.Dummy13, Song.ScarecrowsSong.Name,
                    "Description", "Help Text", Song.ScarecrowsSong)
            }.ToImmutableArray();

            var modes = new Dictionary<ModeId, IDeskPiMode>{
                { ModeId.Help, new HelpMode(modeEntries) },
                { ModeId.Dummy1, new DummyMode(Song.ZeldasLullaby) },
                { ModeId.Dummy2, new DummyMode(Song.EponasSong) },
                { ModeId.Dummy4, new DummyMode(Song.SunsSong) },
                { ModeId.Dummy5, new DummyMode(Song.SongOfTime) },
                { ModeId.Dummy6, new DummyMode(Song.SongOfStorms) },
                { ModeId.Dummy7, new DummyMode(Song.MinuetOfForest) },
                { ModeId.Dummy8, new DummyMode(Song.BoleroOfFire) },
                { ModeId.Dummy9, new DummyMode(Song.SerenadeOfWater) },
                { ModeId.Dummy10, new DummyMode(Song.NocturneOfShadow) },
                { ModeId.Dummy11, new DummyMode(Song.RequiemOfSpirit) },
                { ModeId.Dummy12, new DummyMode(Song.PreludeOfLight) },
                { ModeId.Dummy13, new DummyMode(Song.ScarecrowsSong) },
            }.ToImmutableDictionary();

            var songTrie = new Trie<Note, ModeId>();

            foreach (var entry in modeEntries)
            {
                songTrie = songTrie.Insert(entry.Song.Notes, entry.Mode);
            }

            var keyToNote = new Dictionary<Key, Note>{
                { Key.A, Note.Bottom },
                { Key.B, Note.Right },
                { Key.C, Note.Left },
                { Key.D, Note.A },
                { Key.E, Note.Top }
            }.ToImmutableDictionary();

            ocarinaSelector = new OcarinaSelector(songTrie, keyToNote, ModeId.Help, modes);

            void topLed(bool b) => gpioHandler.Write(20, b);
            void middleLed(bool b) => gpioHandler.Write(16, b);
            void bottomLed(bool b) => gpioHandler.Write(17, b);

            var topButton = SetupButton(pushEvent, gpioHandler, topLed, 4, Button.Top);
            var middleButton = SetupButton(pushEvent, gpioHandler, middleLed, 3, Button.Middle);
            var bottomButton = SetupButton(pushEvent, gpioHandler, bottomLed, 2, Button.Bottom);

            buttonAggregator = new ButtonAggregator(
                new List<ImmutableButton<Button>> { topButton, middleButton, bottomButton });

            //////////////////////////////

            topLed(false);
            middleLed(false);
            bottomLed(false);

            stringWriter = Write(ocarinaSelector.Text);

            Console.WriteLine("Start");
        }

        private DeskPi(DeskPi source,
            StringSsdWriter stringWriter = null,
            ButtonAggregator buttonAggregator = null,
            IDeskPiMode ocarinaSelector = null)
        {
            this.stringWriter = stringWriter ?? source.stringWriter;
            this.buttonAggregator = buttonAggregator ?? source.buttonAggregator;
            this.ocarinaSelector = ocarinaSelector ?? source.ocarinaSelector;
        }

        public DeskPi ReceiveEvent(object ev)
        {
            switch (ev)
            {
                case PinValueChangeEvent pvce:
                    return ReceiveEvent(pvce);
                default:
                    Console.WriteLine($"Unrecognized event {ev}");
                    return this;
            }
        }

        public uint? NextTick(uint currentTime) => 
            PiUtils.Min(buttonAggregator.NextTick(currentTime), 
                stringWriter.NextTick(currentTime));

        public DeskPi Tick(uint currentTime)
        {
            var buttonAggregatorN = buttonAggregator.Tick(currentTime);
            var ocarinaSelectorN = ocarinaSelector;
            var stringWriterN = stringWriter;

            if (buttonAggregatorN == buttonAggregator)
            {
                stringWriterN = stringWriterN.Tick(currentTime);

                if (stringWriter == stringWriterN) return this;

                return new DeskPi(this, stringWriterN);
            }
            if (buttonAggregator.KeyState != buttonAggregatorN.KeyState)
            {
                ocarinaSelectorN = ocarinaSelector.ReceiveKey(buttonAggregatorN.KeyState);
                stringWriterN = Write(ocarinaSelectorN.Text);
            }
            return new DeskPi(this, buttonAggregator: buttonAggregatorN,
                ocarinaSelector: ocarinaSelectorN, stringWriter: stringWriterN);
        }

        private DeskPi ReceiveEvent(PinValueChangeEvent pvce)
        {
            return new DeskPi(this, buttonAggregator: buttonAggregator.OnPinValueChange(pvce.Button));
        }

        private StringSsdWriter Write(TextValue value)
        {
            switch (value)
            {
                case SimpleTextValue stv:
                    return stringWriter.Write(stv.Value);
                case ComplexTextValue ctv:
                    return stringWriter.Write(ctv.Values);
            }
            return stringWriter;
        }

        private static ImmutableButton<Button> SetupButton(
            Action<object> pushEvent, GpioHandler gpioHandler, Action<bool> led,
            int pin, Button id, uint interval = 50)
        {
            gpioHandler.RegisterInterruptCallback(pin,
                () => {
                    pushEvent(new PinValueChangeEvent(id));
                });
            return new ImmutableButton<Button>(
                () => new Ticker(interval, gpioHandler.Millis),
                () => !gpioHandler.Read(pin), led, id);
        }

        private readonly StringSsdWriter stringWriter;
        private readonly ButtonAggregator buttonAggregator;
        private readonly IDeskPiMode ocarinaSelector;
    }
}
