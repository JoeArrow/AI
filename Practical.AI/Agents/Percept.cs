using System;

namespace Practical.AI.Agents
{
    // ------------------------------------------------

    public class Percept
    {
        public eTypePercept Type { get; set; }
        public Tuple<int, int> Position { get; set; }

        // ----------------------------------------------------

        public Percept(Tuple<int, int> position, eTypePercept percept)
        {
            Position = position;
            Type = percept;
        }
    }
}
