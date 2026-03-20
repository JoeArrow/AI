namespace LogicPuzzle.Core
{
    public interface ICategory
    {
        string Name { get; set; }
        List<CategoryItem> Items { get; }
    }
}