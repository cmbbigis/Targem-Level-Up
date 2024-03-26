using Map.Models.Hex;

namespace Units.Models.Unit
{
    public interface IUnitData
    {
        string UnitType { get; }
        int HealthPoints { get; set; }
        int AttackPower { get; }
        int MovementRange { get; }
        void MoveTo(IHexData destination);
        void Attack(IUnitData target);
    }
}