using Cities.Models.City;
using Units.Models.Unit;

namespace Cities.Models.Effects
{
    public interface IEffect
    {
        string Description { get; } // Описание эффекта для отображения в UI или логах

        void Apply(ICityData city); // Применяет эффект к городу
        void Apply(IUnitData unit); // Применяет эффект к юниту
        void Revert(ICityData city); // Отменяет эффект для города
        void Revert(IUnitData unit); // Отменяет эффект для юнита
    }
}