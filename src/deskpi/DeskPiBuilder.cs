using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using immutableSsd;
using piCommon;

namespace deskpi
{
    public static class DeskPiBuilder
    {
        public static DeskPi Create(GpioHandler gpioHandler, Action<object> pushEvent)
        {
            var stringWriter = ImmutableSsd.CreateMax7219BackedDisplay(gpioHandler,
                scrollDelay: 500, endsScrollDelay: 1000);

            var modesData = new Dictionary<ModeId, ModeData>
            {
                {  ModeId.Help, new ModeData("About deskPi - Press middle",
                    "What follows is how to use the thing - press middle",
                    "Press the the top and bottom buttons to get info about " +
                    "the different modes. Press the middle button to get different " +
                    "info about the current mode. Press the top and bottom buttons " +
                    "to play a song to reach a new mode. The available notes: " +
                    "A - Bottom and middle, \u02C5 - Bottom, \u02C3 - Middle, " +
                    "\u02C2 - top, \u02C4 - Middle and top.",
                    Song.SariasSong) },
                { ModeId.Dummy1, new ModeData(Song.ZeldasLullaby.Name,
                    "Description", "Help Text", Song.ZeldasLullaby) },
                { ModeId.Dummy2, new ModeData(Song.EponasSong.Name,
                    "Description", "Help Text", Song.EponasSong) },
                { ModeId.Dummy4,new ModeData( Song.SunsSong.Name,
                    "Description", "Help Text", Song.SunsSong) },
                { ModeId.Time, new ModeData(Song.SongOfTime.Name,
                    "Clock", "Clock in format MM.dd HH.mm", Song.SongOfTime) },
                { ModeId.Dummy6, new ModeData(Song.SongOfStorms.Name,
                    "Description", "Help Text", Song.SongOfStorms) },
                { ModeId.Dummy7, new ModeData(Song.MinuetOfForest.Name,
                    "Description", "Help Text", Song.MinuetOfForest) },
                { ModeId.Dummy8, new ModeData(Song.BoleroOfFire.Name,
                    "Description", "Help Text", Song.BoleroOfFire) },
                { ModeId.Dummy9, new ModeData(Song.SerenadeOfWater.Name,
                    "Description", "Help Text", Song.SerenadeOfWater) },
                { ModeId.Dummy10, new ModeData(Song.NocturneOfShadow.Name,
                    "Description", "Help Text", Song.NocturneOfShadow) },
                { ModeId.Dummy11, new ModeData(Song.RequiemOfSpirit.Name,
                    "Description", "Help Text", Song.RequiemOfSpirit) },
                { ModeId.Dummy12, new ModeData(Song.PreludeOfLight.Name,
                    "Description", "Help Text", Song.PreludeOfLight) },
                { ModeId.Dummy13, new ModeData(Song.ScarecrowsSong.Name,
                    "Description", "Help Text", Song.ScarecrowsSong) }
            }.ToImmutableDictionary();

            var modes = new Dictionary<ModeId, IDeskPiMode>{
                { ModeId.Help, new HelpMode(modesData) },
                { ModeId.Dummy1, new DummyMode(Song.ZeldasLullaby) },
                { ModeId.Dummy2, new DummyMode(Song.EponasSong) },
                { ModeId.Dummy4, new DummyMode(Song.SunsSong) },
                { ModeId.Time, new TimeMode(() => DateTime.Now, (i) => new Ticker(i, gpioHandler.Millis))},
                { ModeId.Dummy6, new DummyMode(Song.SongOfStorms) },
                { ModeId.Dummy7, new DummyMode(Song.MinuetOfForest) },
                { ModeId.Dummy8, new DummyMode(Song.BoleroOfFire) },
                { ModeId.Dummy9, new DummyMode(Song.SerenadeOfWater) },
                { ModeId.Dummy10, new DummyMode(Song.NocturneOfShadow) },
                { ModeId.Dummy11, new DummyMode(Song.RequiemOfSpirit) },
                { ModeId.Dummy12, new DummyMode(Song.PreludeOfLight) },
                { ModeId.Dummy13, new DummyMode(Song.ScarecrowsSong) },
            }.ToImmutableDictionary();

            var keyToNote = new Dictionary<KeyId, Note>{
                {KeyId.A, Note.Bottom },
                {KeyId.B, Note.Right },
                {KeyId.C, Note.Left },
                {KeyId.D, Note.A },
                {KeyId.E, Note.Top }
            }.ToImmutableDictionary();

            var ocarinaSelector = new OcarinaSelector(modesData, modes, keyToNote, ModeId.Help);

            void topLed(bool b) => gpioHandler.Write(20, b);
            void middleLed(bool b) => gpioHandler.Write(16, b);
            void bottomLed(bool b) => gpioHandler.Write(17, b);

            var topButton = SetupButton(pushEvent, gpioHandler, topLed, 4, ButtonId.Top);
            var middleButton = SetupButton(pushEvent, gpioHandler, middleLed, 3, ButtonId.Middle);
            var bottomButton = SetupButton(pushEvent, gpioHandler, bottomLed, 2, ButtonId.Bottom);

            var buttonAggregator = new ButtonAggregator(() => new Ticker(200, gpioHandler.Millis),
                new List<Button<ButtonId>> { topButton, middleButton, bottomButton });

            topLed(false);
            middleLed(false);
            bottomLed(false);

            return new DeskPi(stringWriter, buttonAggregator, ocarinaSelector);
        }

        private static Button<ButtonId> SetupButton(
            Action<object> pushEvent, GpioHandler gpioHandler, Action<bool> led,
            int pin, ButtonId id, uint interval = 50)
        {
            gpioHandler.RegisterInterruptCallback(pin,
                () => {
                    pushEvent(new PinValueChangeEvent(id));
                });
            return new Button<ButtonId>(
                () => new Ticker(interval, gpioHandler.Millis),
                () => !gpioHandler.Read(pin), led, id);
        }
    }
}
