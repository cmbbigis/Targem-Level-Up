namespace Resources.Models.Resource
{
    public class ResourceData: IResourceData
    {
        public ResourceType Type { get; set; }
        public int Quantity { get; set; }
        public int Level { get; set; }

        public void Harvest()
        {
            Level = 1;
        }

        public void Deplete(int amount)
        {
            throw new System.NotImplementedException();
        }

        public bool IsChosen { get; set; }
        public bool IsHighlighted { get; set; }
    }
}