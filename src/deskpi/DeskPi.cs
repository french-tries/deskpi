using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            stringWriter = ImmutableSsd.CreateMax7219BackedDisplay(gpioHandler);

            // Todo review this
            var modes = new Dictionary<Mode, IDeskPiMode>{
                { Mode.Dummy1, new DummyMode(Song.ZeldasLullaby) },
                { Mode.Dummy2, new DummyMode(Song.EponasSong) },
                { Mode.Help, new HelpMode() },
                { Mode.Dummy4, new DummyMode(Song.SunsSong) },
                { Mode.Dummy5, new DummyMode(Song.SongOfTime) },
                { Mode.Dummy6, new DummyMode(Song.SongOfStorms) },
                { Mode.Dummy7, new DummyMode(Song.MinuetOfForest) },
                { Mode.Dummy8, new DummyMode(Song.BoleroOfFire) },
                { Mode.Dummy9, new DummyMode(Song.SerenadeOfWater) },
                { Mode.Dummy10, new DummyMode(Song.NocturneOfShadow) },
                { Mode.Dummy11, new DummyMode(Song.RequiemOfSpirit) },
                { Mode.Dummy12, new DummyMode(Song.PreludeOfLight) },
                { Mode.Dummy13, new DummyMode(Song.ScarecrowsSong) },
            }.ToImmutableDictionary();

            var songTrie = new Trie<Note, Mode>()
                .Insert(Song.ZeldasLullaby.Notes, Mode.Dummy1)
                .Insert(Song.EponasSong.Notes, Mode.Dummy2)
                .Insert(Song.SariasSong.Notes, Mode.Help)
                .Insert(Song.SunsSong.Notes, Mode.Dummy4)
                .Insert(Song.SongOfTime.Notes, Mode.Dummy5)
                .Insert(Song.SongOfStorms.Notes, Mode.Dummy6)
                .Insert(Song.MinuetOfForest.Notes, Mode.Dummy7)
                .Insert(Song.BoleroOfFire.Notes, Mode.Dummy8)
                .Insert(Song.SerenadeOfWater.Notes, Mode.Dummy9)
                .Insert(Song.NocturneOfShadow.Notes, Mode.Dummy10)
                .Insert(Song.RequiemOfSpirit.Notes, Mode.Dummy11)
                .Insert(Song.PreludeOfLight.Notes, Mode.Dummy12)
                .Insert(Song.ScarecrowsSong.Notes, Mode.Dummy13);

            var keyToNote = new Dictionary<Key, Note>{
                { Key.A, Note.Bottom },
                { Key.B, Note.Right },
                { Key.C, Note.Left },
                { Key.D, Note.A },
                { Key.E, Note.Top }
            }.ToImmutableDictionary();

            ocarinaSelector = new OcarinaSelector(songTrie, keyToNote, Mode.Help, modes);

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
