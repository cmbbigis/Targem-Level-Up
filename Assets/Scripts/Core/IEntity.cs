namespace Core
{
    public interface IEntity
    {
        // HIGHLIGHTING
        bool IsChosen { get; set; }
        bool IsHighlighted { get; set; }
    }
}