namespace Core
{
    public interface IEntity
    {
        bool IsChosen { get; set; }
        bool IsHighlighted { get; set; }
    }
}