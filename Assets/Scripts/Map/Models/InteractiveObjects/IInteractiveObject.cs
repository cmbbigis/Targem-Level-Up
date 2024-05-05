using System.Collections.Generic;

namespace Map.Models.InteractiveObjects
{
    public interface IInteractiveObject
    {
        public string Name { get; set; }
        public bool IsDestroyable { get; set; }
        public bool IsExplorable { get; set; }
        public List<IItem> Rewards { get; set; }
        public void Destroy();
        public void Explore();
    }

    // Пример класса Item для наград
    public interface IItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}
