using Map.Models.Hex;

namespace Core
{
    public interface IAttackable : IEntity
    {
        IHexData Hex { get; set; }
        float HealthPoints { get; set; }
        float Defense { get; set; }
        float GetDefensiveAbilitiesModifier();
        void Die();
    }
}