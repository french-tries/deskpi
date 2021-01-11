using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using deskpi.test.stubs;
using NUnit.Framework;

namespace deskpi.test
{
    [TestFixture]
    public class OcarinaSelectorTests
    {
        [TestCase]
        public void OcarinaSelector_AtStart_CheckOrphans()
        {
            var modesData = new Dictionary<ModeId, ModeData>{
                { ModeId.Help, new ModeData("Name", "Description", "Help", Song.ZeldasLullaby)}
            }.ToImmutableDictionary();

            var modes = new Dictionary<ModeId, IDeskPiMode>{
            }.ToImmutableDictionary();

            var keyToNote = new Dictionary<KeyId, Note>{
            }.ToImmutableDictionary();

            var defaultMode = ModeId.Help;

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                var selector = new OcarinaSelector(modesData, modes, keyToNote, defaultMode);

                Assert.AreEqual($"Orphaned mode with ID {ModeId.Help}\n", sw.ToString());
            }
            Console.SetOut(new StreamWriter(Console.OpenStandardError()));
        }

        [TestCase("A", KeyId.A)]
        [TestCase("AAAAAAAA", KeyId.A, KeyId.A, KeyId.A, KeyId.A, KeyId.A, KeyId.A, KeyId.A, KeyId.A)]
        [TestCase("AAAAAAAA", KeyId.A, KeyId.A, KeyId.A, KeyId.A, KeyId.A, KeyId.A, KeyId.A, KeyId.A, KeyId.A)]
        public void ReceiveKey_SelectorMode_ReturnsNotesViaText(string expected, params KeyId[] keys)
        {
            var modesData = new Dictionary<ModeId, ModeData>{}.ToImmutableDictionary();
            var modes = new Dictionary<ModeId, IDeskPiMode>{}.ToImmutableDictionary();

            var keyToNote = new Dictionary<KeyId, Note>{
                {KeyId.A, Note.A },
            }.ToImmutableDictionary();

            IDeskPiMode selector = new OcarinaSelector(modesData, modes, keyToNote, ModeId.Selector);

            foreach (var key in keys)
            {
                selector = selector.ReceiveKey(key);
            }
            Assert.AreEqual(expected, selector.Text[0].Item1);
        }

        [TestCase]
        public void Text_NotSelectorMode_ReturnsModeText()
        {
            var modesData = new Dictionary<ModeId, ModeData>{
                { ModeId.Help, new ModeData("Name", "Description", "Help", Song.ZeldasLullaby)}
            }.ToImmutableDictionary();

            var modeStub = new DeskPiModeStub { TextVal = "test" };
            var modes = new Dictionary<ModeId, IDeskPiMode>{
                {ModeId.Help, modeStub}
            }.ToImmutableDictionary();

            var keyToNote = new Dictionary<KeyId, Note>{}.ToImmutableDictionary();

            var selector = new OcarinaSelector(modesData, modes, keyToNote, ModeId.Help);

            Assert.AreEqual(modeStub.TextVal, selector.Text[0].Item1);
        }

        [TestCase]
        public void ReceivedKey_F_GoesToSelectorMode()
        {
            var modesData = new Dictionary<ModeId, ModeData>{
                { ModeId.Help, new ModeData("Name", "Description", "Help", Song.ZeldasLullaby)}
            }.ToImmutableDictionary();

            var modeStub = new DeskPiModeStub { TextVal = "test" };
            var modes = new Dictionary<ModeId, IDeskPiMode>{
                {ModeId.Help, modeStub}
            }.ToImmutableDictionary();

            var keyToNote = new Dictionary<KeyId, Note> { }.ToImmutableDictionary();

            var selector = new OcarinaSelector(modesData, modes, keyToNote, ModeId.Help)
                .ReceiveKey(KeyId.F);

            Assert.AreEqual("", selector.Text[0].Item1);
        }

         

        [TestCase]
        public void ReceivedKey_MatchSong_GoesToMode()
        {
            var modesData = new Dictionary<ModeId, ModeData>{
                { ModeId.Help, new ModeData("Name", "Description", "Help",
                    new Song("Test", ImmutableList<Note>.Empty.Add(Note.A)))}
            }.ToImmutableDictionary();

            var modeStub = new DeskPiModeStub { TextVal = "test" };
            var modes = new Dictionary<ModeId, IDeskPiMode>{
                {ModeId.Help, modeStub}
            }.ToImmutableDictionary();

            var keyToNote = new Dictionary<KeyId, Note> {
                {KeyId.A, Note.A },
            }.ToImmutableDictionary();

            var selector = new OcarinaSelector(modesData, modes, keyToNote, ModeId.Selector)
                .ReceiveKey(KeyId.A);

            Assert.AreEqual("test", selector.Text[0].Item1);
        }

        [TestCase]
        public void NextTick_RetreivesFromInner()
        {
            var modesData = new Dictionary<ModeId, ModeData>{
                { ModeId.Help, new ModeData("Name", "Description", "Help", Song.ZeldasLullaby)},
                { ModeId.Time, new ModeData("Name", "Description", "Help", Song.PreludeOfLight)}
            }.ToImmutableDictionary();

            var modeStub = new DeskPiModeStub { NextTickVal = 2 };
            var modeStub2 = new DeskPiModeStub { NextTickVal = 3 };
            var modes = new Dictionary<ModeId, IDeskPiMode>{
                {ModeId.Help, modeStub},
                {ModeId.Time, modeStub2}
            }.ToImmutableDictionary();

            var keyToNote = new Dictionary<KeyId, Note> { }.ToImmutableDictionary();

            var selector = new OcarinaSelector(modesData, modes, keyToNote, ModeId.Help);

            Assert.AreEqual(2, selector.NextTick(0));
        }

        [TestCase]
        public void Tick_NoChange_ReturnsSame()
        {
            var modesData = new Dictionary<ModeId, ModeData>{
                { ModeId.Help, new ModeData("Name", "Description", "Help", Song.ZeldasLullaby)},
            }.ToImmutableDictionary();

            var modeStub = new DeskPiModeStub { NextTickVal = 2 };
            var modes = new Dictionary<ModeId, IDeskPiMode>{
                {ModeId.Help, modeStub},
            }.ToImmutableDictionary();

            var keyToNote = new Dictionary<KeyId, Note> { }.ToImmutableDictionary();

            var selector = new OcarinaSelector(modesData, modes, keyToNote, ModeId.Help);
            var selectorN = selector.Tick(1);

            Assert.AreEqual(selector, selectorN);
            Assert.AreEqual(1, modeStub.ReceivedTick);
        }

        [TestCase]
        public void Tick_Changes_Updates()
        {
            var modesData = new Dictionary<ModeId, ModeData>{
                { ModeId.Help, new ModeData("Name", "Description", "Help", Song.ZeldasLullaby)},
            }.ToImmutableDictionary();

            var modeStub = new DeskPiModeStub { NextTickVal = 2, Next = new DeskPiModeStub()};
            var modes = new Dictionary<ModeId, IDeskPiMode>{
                {ModeId.Help, modeStub},
            }.ToImmutableDictionary();

            var keyToNote = new Dictionary<KeyId, Note> { }.ToImmutableDictionary();

            var selector = new OcarinaSelector(modesData, modes, keyToNote, ModeId.Help);
            var selectorN = selector.Tick(1);

            Assert.AreNotEqual(selector, selectorN);
        }
    }
}
