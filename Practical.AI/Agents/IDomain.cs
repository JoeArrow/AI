namespace Practical.AI.Agents
{
    public interface IDomain
    {
        double TerrainAt(int x, int y);
        bool WaterAt(int x, int y);
    }
}