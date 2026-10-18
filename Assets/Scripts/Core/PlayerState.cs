namespace Yamblatro.Core
{
    /// <summary>
    /// One player in a match. Money and modifiers will be added when the shop
    /// is built.
    /// </summary>
    public class PlayerState
    {
        public int Id { get; private set; }

        public ScorecardState Scorecard { get; private set; }

        public PlayerState(int id)
        {
            Id = id;
            Scorecard = new ScorecardState();
        }
    }
}
