namespace Logic
{
    public sealed class CategoryItem
    {
        public string Name
        {
            get;
        }

        public CategoryItem(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
