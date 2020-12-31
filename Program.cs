using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using deskpi;
using deskpi.src.deskpi.ocarinaSelector;
using piCommon;
using static deskpi.ButtonAggregator;

namespace immutableSsd
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            MainClass main = new MainClass();
            main.Run();
        }

        private MainClass()
        {
            var gpioHandler = new GpioHandler();

            var directWriter = new Max7219CommonAnodeWriter((obj) => gpioHandler.SpiWrite(obj));

            var converter = new SegmentsConverter();

            var selector = new ScrollingSelector<byte>(
                InterruptHandler.RequestInterrupt(ReceiveInterrupt), 1000, 2000, 8);

            stringWriter = new StringSsdWriter(directWriter, converter.GetSegments, selector);

            var songTrie = new SongsTrie()
                .Insert(Songs.ZeldasLullaby, () => stringWriter = stringWriter.Write("Zelda's Lullaby"))
                .Insert(Songs.EponasSong, () => stringWriter = stringWriter.Write("Epona's Song"))
                .Insert(Songs.SariasSong, () => stringWriter = stringWriter.Write("Saria's Song"))
                .Insert(Songs.SunsSong, () => stringWriter = stringWriter.Write("Sun's Song"))
                .Insert(Songs.SongOfTime, () => stringWriter = stringWriter.Write("Song Of Time"))
                .Insert(Songs.SongOfStorms, () => stringWriter = stringWriter.Write("Song Of Storms"))
                .Insert(Songs.MinuetOfForest, () => stringWriter = stringWriter.Write("Minuet Of Forest"))
                .Insert(Songs.BoleroOfFire, () => stringWriter = stringWriter.Write("Bolero Of Fire"))
                .Insert(Songs.SerenadeOfWater, () => stringWriter = stringWriter.Write("Serenade Of Water"))
                .Insert(Songs.NocturneOfShadow, () => stringWriter = stringWriter.Write("Nocturne Of Shadow"))
                .Insert(Songs.RequiemOfSpirit, () => stringWriter = stringWriter.Write("Requiem Of Spirit"))
                .Insert(Songs.PreludeOfLight, () => stringWriter = stringWriter.Write("Prelude Of Light"))
                .Insert(Songs.ScarecrowsSong, () => stringWriter = stringWriter.Write("Scarecrow's Song"));

            ocarinaSelector = new OcarinaSelector(songTrie);

            topLed = (b) => gpioHandler.Write(20, b);
            middleLed = (b) => gpioHandler.Write(16, b);
            bottomLed = (b) => gpioHandler.Write(17, b);

            var KeyToNote = new Dictionary<Key, Songs.Note>{
                { Key.A, Songs.Note.Bottom },
                { Key.B, Songs.Note.Right },
                { Key.C, Songs.Note.Left },
                { Key.D, Songs.Note.A },
                { Key.E, Songs.Note.Top }
            };

            var buttonAggregator = new ButtonAggregator((obj) => {
                if (KeyToNote.ContainsKey(obj))
                    ocarinaSelector = ocarinaSelector.ReceiveNote(KeyToNote[obj]);
            });

            topButton = new ImmutableButton(
                InterruptHandler.RequestInterrupt((c) => { topButton = topButton.ReceiveInterrupt(c); }),
                () => gpioHandler.Read(4),
                (b) => {
                    topLed(!b);
                    buttonAggregator = buttonAggregator.OnButtonUpdate(Button.Top, !b);
                });
                
            middleButton = new ImmutableButton(
                InterruptHandler.RequestInterrupt((c) => { middleButton = middleButton.ReceiveInterrupt(c); }),
                () => gpioHandler.Read(3),
                (b) => {
                    middleLed(!b);
                    buttonAggregator = buttonAggregator.OnButtonUpdate(Button.Middle, !b);
                });

            bottomButton = new ImmutableButton(
                InterruptHandler.RequestInterrupt((c) => { bottomButton = bottomButton.ReceiveInterrupt(c); }),
                () => gpioHandler.Read(2),
                (b) => {
                    bottomLed(!b);
                    buttonAggregator = buttonAggregator.OnButtonUpdate(Button.Bottom, !b);
                });

            gpioHandler.RegisterInterruptCallback(4,
                () => events.Enqueue(() => { topButton = topButton.OnPinValueChange(); }));
            gpioHandler.RegisterInterruptCallback(3,
                () => events.Enqueue(() => { middleButton = middleButton.OnPinValueChange(); }));
            gpioHandler.RegisterInterruptCallback(2,
                () => events.Enqueue(() => { bottomButton = bottomButton.OnPinValueChange(); }));

        }

        private void Run()
        {
            topLed(false);
            middleLed(false);
            bottomLed(false);

            stringWriter = stringWriter.Write("Hello world please work...");

            while (true)
            {
                if (events.TryDequeue(out Action ev))
                {
                    ev();
                }
            }
        }

        // TODO if raised before construction is completed, events are lost
        // TODO stop / restart program ?
        private void ReceiveInterrupt(object caller)
        {
            if (stringWriter != null)
            {
                stringWriter = stringWriter.ReceiveInterrupt(caller);
            }
        }

        private ISsdWriter<string> stringWriter;

        private readonly Action<bool> topLed;
        private readonly Action<bool> middleLed;
        private readonly Action<bool> bottomLed;

        private ImmutableButton topButton;
        private ImmutableButton middleButton;
        private ImmutableButton bottomButton;

        private OcarinaSelector ocarinaSelector;

        private ConcurrentQueue<Action> events = new ConcurrentQueue<Action>();
    }
}
