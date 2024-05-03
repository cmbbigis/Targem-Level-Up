using Players.Models.Player;

namespace Core
{
    public class Player
    {
        public PlayerData Data { get; set; }
        public PlayerTurnState TurnState { get; set; }
    }
}