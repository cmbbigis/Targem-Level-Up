using Map.Models.Hex;
using Players.Models.Player;

namespace Core
{
    public interface IAttackable : IEntity
    {
        IHexData Hex { get; set; }
        IPlayerData Master { get; set; }
        float StartHealthPoints { get; set; }
        float HealthPoints { get; set; }
        float Defense { get; set; }
        float GetDefensiveAbilitiesModifier();
        bool IsAlive { get; }
        void Die();
    }
}