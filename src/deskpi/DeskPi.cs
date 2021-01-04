using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using deskpi.ocarinaSelector;
using immutableSsd;
using piCommon;

namespace deskpi
{
    public class TimerEvent
    {
        public object Caller { get; }

        public TimerEvent(object Caller)
        {
            this.Caller = Caller;
        }
    }

    public class PinValueChangeEvent
    {
        public Button Button { get; }

        public PinValueChangeEvent(Button Button)
        {
            this.Button = Button;
        }
    }

    public class ButtonEvent
    {
        public Button Source { get; }
        public bool Rising { get; }

        public ButtonEvent(Button Source, bool Rising)
        {
            this.Source = Source;
            this.Rising = Rising;
        }
    }

    public class KeyEvent
    {
        public Key Key { get; }

        public KeyEvent(Key Key)
        {
            this.Key = Key;
        }
    }

    public class DeskPi
    {
        public DeskPi(GpioHandler gpioHandler, Action<object> pushEvent)
        {
            stringWriter = ImmutableSsd.CreateMax7219BackedDisplay(gpioHandler,
                InterruptHandler.RequestInterrupt((obj) => pushEvent(new TimerEvent(obj))));

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

            var songTrie = new Trie<Note,Mode>()
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

            // TODO use enum values?
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

            buttonAggregator = new ButtonAggregator((obj) => pushEvent(new KeyEvent(obj)));

            topButton = SetupButton(pushEvent, gpioHandler, topLed, 4, Button.Top);
            middleButton = SetupButton(pushEvent, gpioHandler, middleLed, 3, Button.Middle);
            bottomButton = SetupButton(pushEvent, gpioHandler, bottomLed, 2, Button.Bottom);

            //////////////////////////////

            topLed(false);
            middleLed(false);
            bottomLed(false);

            stringWriter = Write(ocarinaSelector.Text);
        }
         
        private StringSsdWriter Write(TextValue value)
        {
            switch (value)
            {
                case SimpleTextValue stv :
                    return stringWriter.Write(stv.Value);
                case ComplexTextValue ctv:
                    return stringWriter.Write(ctv.Values);
            }
            return stringWriter;
        }

        private DeskPi(DeskPi source,
            StringSsdWriter stringWriter = null,
            ButtonAggregator buttonAggregator = null,
            ImmutableButton topButton = null,
            ImmutableButton middleButton = null,
            ImmutableButton bottomButton = null,
            IDeskPiMode ocarinaSelector = null)
        {
            this.stringWriter = stringWriter ?? source.stringWriter;
            this.buttonAggregator = buttonAggregator ?? source.buttonAggregator;
            this.topButton = topButton ?? source.topButton;
            this.middleButton = middleButton ?? source.middleButton;
            this.bottomButton = bottomButton ?? source.bottomButton;
            this.ocarinaSelector = ocarinaSelector ?? source.ocarinaSelector;
        }

        public DeskPi ReceiveEvent(object ev)
        {
            switch (ev)
            {
                case TimerEvent te:
                    var topButtonN = topButton.ReceiveInterrupt(te.Caller);
                    var middleButtonN = middleButton.ReceiveInterrupt(te.Caller);
                    var bottomButtonN = bottomButton.ReceiveInterrupt(te.Caller);
                    var stringWriterN = stringWriter.ReceiveInterrupt(te.Caller);

                    if (topButton == topButtonN && middleButton == middleButtonN && 
                        bottomButton == bottomButtonN && stringWriter == stringWriterN)
                    {
                        // TODO sometimes happens
                        Console.WriteLine($"Unrecognized TimerEvent with caller {te.Caller}");
                        return this;
                    }
                    return new DeskPi(this, topButton: topButtonN, 
                        middleButton: middleButtonN, bottomButton: bottomButtonN,
                        stringWriter: stringWriterN);

                case PinValueChangeEvent pvce:
                    switch (pvce.Button)
                    {
                        case Button.Top:
                            return new DeskPi(this, topButton : topButton.OnPinValueChange());
                        case Button.Middle:
                            return new DeskPi(this, middleButton: middleButton.OnPinValueChange());
                        case Button.Bottom:
                            return new DeskPi(this, bottomButton: bottomButton.OnPinValueChange());
                        default:
                            Console.WriteLine($"Unrecognized PinValueChangeEvent with button {pvce.Button}");
                            return this;
                    }
                case ButtonEvent be:
                    return new DeskPi(this, buttonAggregator : buttonAggregator.OnButtonUpdate(be.Source, !be.Rising));

                case KeyEvent ke:
                    var ocarinaSelectorN = ocarinaSelector.ReceiveKey(ke.Key);

                    return new DeskPi(this, ocarinaSelector: ocarinaSelectorN,
                        stringWriter: stringWriter = Write(ocarinaSelectorN.Text));

                default:
                    Console.WriteLine($"Unrecognized event {ev}");
                    return this;
            }
        }

        private static ImmutableButton SetupButton(Action<object> pushEvent,
            GpioHandler gpioHandler, Action<bool> led, int pin, Button button)
        {
            var result = new ImmutableButton(
                InterruptHandler.RequestInterrupt((c) => { pushEvent(new TimerEvent(c)); }),
                () => gpioHandler.Read(pin),
                (b) => {
                    led(!b);
                    pushEvent(new ButtonEvent(button, b));
                });
            gpioHandler.RegisterInterruptCallback(pin,
                () => pushEvent(new PinValueChangeEvent(button)));

            return result;
        }

        private StringSsdWriter stringWriter;

        private readonly ButtonAggregator buttonAggregator;

        private readonly ImmutableButton topButton;
        private readonly ImmutableButton middleButton;
        private readonly ImmutableButton bottomButton;

        private readonly IDeskPiMode ocarinaSelector;
    }
}
