using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NUnit.Framework;

namespace deskpi.test
{
    [TestFixture]
    public class HelpModeTests
    {
        [TestCase]
        public void ReceiveKey_B_CycleBetweenFields()
        {
            var modesData = new Dictionary<ModeId, ModeData>{
                { ModeId.Help, new ModeData("Name", "Description", "Help", Song.ZeldasLullaby)}
            }.ToImmutableDictionary();

            var modeData = new ModeData("", "", "", Song.EmptySong);
            DeskPiMode deskPiMode = new HelpMode(() => null, modeData, modesData);
            Assert.AreEqual(modesData[ModeId.Help].Name, deskPiMode.Text[0].Item1);

            deskPiMode = deskPiMode.ReceiveKey(KeyId.B);
            Assert.AreEqual(modesData[ModeId.Help].Description, deskPiMode.Text[0].Item1);

            deskPiMode = deskPiMode.ReceiveKey(KeyId.B);
            Assert.AreEqual(modesData[ModeId.Help].HelpText, deskPiMode.Text[0].Item1);

            deskPiMode = deskPiMode.ReceiveKey(KeyId.B);
            Assert.AreEqual(modesData[ModeId.Help].Song.Name, deskPiMode.Text[0].Item1);

            deskPiMode = deskPiMode.ReceiveKey(KeyId.B);
            Assert.AreEqual(Song.NotesToString(modesData[ModeId.Help].Song.Notes), deskPiMode.Text[0].Item1);

            deskPiMode = deskPiMode.ReceiveKey(KeyId.B);
            Assert.AreEqual(modesData[ModeId.Help].Name, deskPiMode.Text[0].Item1);
        }

        [TestCase]
        public void ReceiveKey_A_GoToPreviousMode()
        {
            var modesData = new Dictionary<ModeId, ModeData>{
                { ModeId.Dummy1, new ModeData("1", "Description", "Help", Song.ZeldasLullaby)},
                { ModeId.Dummy2, new ModeData("2", "Description", "Help", Song.ZeldasLullaby)},
                { ModeId.Help, new ModeData("3", "Description", "Help", Song.ZeldasLullaby)}
            }.ToImmutableDictionary();

            var modeData = new ModeData("", "", "", Song.EmptySong);
            DeskPiMode deskPiMode = new HelpMode(() => null, modeData, modesData, ModeId.Dummy1);
            Assert.AreEqual(modesData[ModeId.Dummy1].Name, deskPiMode.Text[0].Item1);

            deskPiMode = deskPiMode.ReceiveKey(KeyId.A);
            Assert.AreEqual(modesData[ModeId.Help].Name, deskPiMode.Text[0].Item1);

            deskPiMode = deskPiMode.ReceiveKey(KeyId.A);
            Assert.AreEqual(modesData[ModeId.Dummy2].Name, deskPiMode.Text[0].Item1);

            deskPiMode = deskPiMode.ReceiveKey(KeyId.A);
            Assert.AreEqual(modesData[ModeId.Dummy1].Name, deskPiMode.Text[0].Item1);
        }

        [TestCase]
        public void ReceiveKey_C_GoToNextMode()
        {
            var modesData = new Dictionary<ModeId, ModeData>{
                { ModeId.Dummy1, new ModeData("1", "Description", "Help", Song.ZeldasLullaby)},
                { ModeId.Dummy2, new ModeData("2", "Description", "Help", Song.ZeldasLullaby)},
                { ModeId.Help, new ModeData("3", "Description", "Help", Song.ZeldasLullaby)}
            }.ToImmutableDictionary();

            var modeData = new ModeData("", "", "", Song.EmptySong);
            DeskPiMode deskPiMode = new HelpMode(() => null, modeData, modesData, ModeId.Dummy1);
            Assert.AreEqual(modesData[ModeId.Dummy1].Name, deskPiMode.Text[0].Item1);

            deskPiMode = deskPiMode.ReceiveKey(KeyId.C);
            Assert.AreEqual(modesData[ModeId.Dummy2].Name, deskPiMode.Text[0].Item1);

            deskPiMode = deskPiMode.ReceiveKey(KeyId.C);
            Assert.AreEqual(modesData[ModeId.Help].Name, deskPiMode.Text[0].Item1);

            deskPiMode = deskPiMode.ReceiveKey(KeyId.C);
            Assert.AreEqual(modesData[ModeId.Dummy1].Name, deskPiMode.Text[0].Item1);
        }

        [TestCase]
        public void ReceivedKey_F_GoesToSelectorMode()
        {
            var modesData = new Dictionary<ModeId, ModeData>{
                { ModeId.Dummy1, new ModeData("1", "Description", "Help", Song.ZeldasLullaby)},
                { ModeId.Dummy2, new ModeData("2", "Description", "Help", Song.ZeldasLullaby)},
                { ModeId.Help, new ModeData("3", "Description", "Help", Song.ZeldasLullaby)}
            }.ToImmutableDictionary();

            var modeData = new ModeData("", "", "", Song.EmptySong);
            DeskPiMode deskPiMode = new HelpMode(() => null, modeData, modesData, ModeId.Dummy1);

            var deskPiModeN = deskPiMode.ReceiveKey(KeyId.F);

            Assert.IsNull(deskPiModeN);
        }
    }
}
