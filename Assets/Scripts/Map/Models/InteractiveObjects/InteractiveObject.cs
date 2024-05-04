using System.Collections.Generic;

namespace Map.Models.InteractiveObjects
{
    public class InteractiveObject
    {
        public string Name { get; set; }
        public bool IsDestroyable { get; set; }
        public bool IsExplorable { get; set; }
        public List<Item> Rewards { get; set; }

        public InteractiveObject(string name, bool isDestroyable, bool isExplorable, List<Item> rewards)
        {
            Name = name;
            IsDestroyable = isDestroyable;
            IsExplorable = isExplorable;
            Rewards = rewards;
        }

        // Методы для взаимодействия с объектом
        public void Destroy()
        {
            // Логика уничтожения объекта и получения наград
        }

        public void Explore()
        {
            // Логика исследования объекта и получения наград
        }
    }

    // Пример класса Item для наград
    public class Item
    {
        public string Name { get; set; }
        public int Quantity { get; set; }

        public Item(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;
        }
    }
}
