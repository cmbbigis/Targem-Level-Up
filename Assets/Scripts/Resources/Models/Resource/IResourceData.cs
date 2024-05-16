using Cities.Models.City;
using Core;

namespace Resources.Models.Resource
{
    public interface IResourceData: IAttackable
    {
        ResourceType Type { get; }
        ICityData ConnectedCity { get; set; }
        int Quantity { get; set; }
        float Level { get; set; }
        int IntLevel { get; }
        void Harvest();
        void Deplete(int amount);
    }
}