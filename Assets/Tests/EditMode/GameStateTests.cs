using System;
using NUnit.Framework;

namespace Yamblatro.Core.Tests
{
    public class GameStateTests
    {
        [Test]
        public void NewGame_HasRequestedPlayersWithDistinctIds()
        {
            GameState game = new GameState(2);

            Assert.AreEqual(2, game.Players.Count);
            Assert.AreEqual(0, game.Players[0].Id);
            Assert.AreEqual(1, game.Players[1].Id);
        }

        [Test]
        public void Players_HaveSeparateEmptyScorecards()
        {
            GameState game = new GameState(2);

            game.Players[0].Scorecard.Record(ScoreColumn.Free, ScoreCategory.Ones, 3);

            Assert.IsTrue(game.Players[0].Scorecard.IsFilled(ScoreColumn.Free, ScoreCategory.Ones));
            Assert.IsFalse(game.Players[1].Scorecard.IsFilled(ScoreColumn.Free, ScoreCategory.Ones));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void FewerThanOnePlayer_Throws(int playerCount)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new GameState(playerCount));
        }
    }
}
