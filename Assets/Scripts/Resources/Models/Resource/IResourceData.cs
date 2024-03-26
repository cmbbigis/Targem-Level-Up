using Resources.Models.Resource;

namespace Resources
{
    public interface IResourceData
    {
        ResourceType Type { get; }
        int Quantity { get; set; }
        void Harvest();
        void Deplete(int amount);
    }
}