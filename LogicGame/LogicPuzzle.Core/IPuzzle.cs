namespace LogicPuzzle.Core
{
    public interface IPuzzle
    {
        IReadOnlyList<ICategory> Categories { get; }
    }
}