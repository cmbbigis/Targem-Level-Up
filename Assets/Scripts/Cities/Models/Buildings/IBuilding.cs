using System.Collections.Generic;
using Cities.Models.City;
using Cities.Models.Effects;

namespace Cities.Models.Buildings
{
    public interface IBuilding
    {
        string Name { get; } // Название постройки
        int Cost { get; } // Стоимость постройки
        int BuildTime { get; } // Время, необходимое для строительства
        bool IsConstructed { get; set; } // Статус постройки: построена ли она
        List<IEffect> Effects { get; } // Эффекты от постройки

        void Construct(ICityData city); // Метод для начала строительства постройки в городе
        void ApplyEffects(ICityData city); // Метод для применения эффектов постройки к городу
        void RemoveEffects(ICityData city); // Метод для удаления эффектов постройки из города
    }
}