using Core;

namespace Resources.Models.Resource
{
    public interface IResourceData: IAttackable
    {
        ResourceType Type { get; }
        int Quantity { get; set; }
        float Level { get; set; }
        void Harvest();
        void Deplete(int amount);
    }
}