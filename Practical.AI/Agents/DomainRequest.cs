using System.Collections.Generic;

namespace Practical.AI.Agents
{
    // ----------------------------------------------------
    /// <summary>
    ///     DomainRequest Description
    /// </summary>

    public class DomainRequest
    {
        public int X { set; get; }
        public int Y { set; get; }
        public IDomain Domain { set; get; }
        public int SenseRadius { set; get; }
        public double[,] Terrain { set; get; }
        public double ObstacleThreshold { set; get; }
        public IEnumerable<Belief> InitialBeliefs { set; get; }
    }
}
