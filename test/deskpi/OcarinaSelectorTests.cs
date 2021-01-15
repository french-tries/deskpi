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

            var modes = new Dictionary<ModeId, Func<DeskPiMode, DeskPiMode>>
            {
            }.ToImmutableDictionary();

            var keyToNote = new Dictionary<KeyId, Note>{
            }.ToImmutableDictionary();

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                var modeData = new ModeData("", "", "", Song.EmptySong);
                var selector = new OcarinaSelector(modeData, modesData, modes, keyToNote, (x) => null);

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
            var modes = new Dictionary<ModeId, Func<DeskPiMode, DeskPiMode>> {}.ToImmutableDictionary();

            var keyToNote = new Dictionary<KeyId, Note>{
                {KeyId.A, Note.A },
            }.ToImmutableDictionary();

            var modeData = new ModeData("", "", "", Song.EmptySong);
            DeskPiMode selector = new OcarinaSelector(modeData, modesData, modes, keyToNote, (x) => null);

            foreach (var key in keys)
            {
                selector = selector.ReceiveKey(key);
            }
            Assert.AreEqual(expected, selector.Text[0].Item1);
        }

        [TestCase]
        public void ReceivedKey_MatchSong_GoesToMode()
        {
            var modesData = new Dictionary<ModeId, ModeData>{
                { ModeId.Help, new ModeData("Name", "Description", "Help",
                    new Song("Test", ImmutableList<Note>.Empty.Add(Note.A)))}
            }.ToImmutableDictionary();

            var modeStub = new DeskPiModeStub { TextVal = "test" };
            var modes = new Dictionary<ModeId, Func<DeskPiMode, DeskPiMode>>{
                {ModeId.Help, (s) => modeStub}
            }.ToImmutableDictionary();

            var keyToNote = new Dictionary<KeyId, Note> {
                {KeyId.A, Note.A },
            }.ToImmutableDictionary();

            var modeData = new ModeData("", "", "", Song.EmptySong);
            var selector = new OcarinaSelector(modeData, modesData, modes, keyToNote, (x) => x)
                .ReceiveKey(KeyId.A);

            Assert.AreEqual("test", selector.Text[0].Item1);
        }
    }
}
