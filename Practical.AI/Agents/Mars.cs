using System;

namespace Practical.AI.Agents
{
    // ----------------------------------------------------
    /// <summary>
    ///     Represents Mars environment.
    /// </summary>
    
    public class Mars
    {
        private readonly double[,] _terrain;

        public Mars(double[,] terrain)
        {
            _terrain = new double[terrain.GetLength(0), terrain.GetLength(1)];
            Array.Copy(terrain, _terrain, terrain.GetLength(0) * terrain.GetLength(1));
        }

        // ----------------------------------------------------

        public double TerrainAt(int x, int y)
        {
            return _terrain[x, y];
        }

        // ----------------------------------------------------

        public bool WaterAt(int x, int y)
        {
            return _terrain[x, y] < 0;
        }
    }
}
