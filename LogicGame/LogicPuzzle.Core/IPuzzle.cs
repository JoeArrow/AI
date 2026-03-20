using LogicPuzzle.Core.Clues;

namespace LogicPuzzle.Core
{
    public interface IPuzzle
    {
        string Title { get; }
        IReadOnlyList<IClue> Clues { get; }
        IReadOnlyList<ICategory> Categories { get; }
    }
}