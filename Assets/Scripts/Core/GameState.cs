using System;
using System.Collections.Generic;

namespace Yamblatro.Core
{
    /// <summary>
    /// A match and its players. Turn state, the active player and match
    /// status will be added with the turn logic.
    /// </summary>
    public class GameState
    {
        private readonly PlayerState[] players;
        private readonly IReadOnlyList<PlayerState> readOnlyPlayers;

        /// <summary>Creates a match with the given number of players, with IDs 0 to playerCount - 1.</summary>
        public GameState(int playerCount)
        {
            if (playerCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(playerCount), playerCount, "A match needs at least one player.");
            }

            players = new PlayerState[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                players[i] = new PlayerState(i);
            }
            readOnlyPlayers = Array.AsReadOnly(players);
        }

        public IReadOnlyList<PlayerState> Players
        {
            get { return readOnlyPlayers; }
        }
    }
}
