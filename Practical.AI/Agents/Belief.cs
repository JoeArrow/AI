using System;
using System.Collections.Generic;
using System.Text;

namespace Practical.AI.Agents
{
    public class Belief
    {
        public eTypesBelief Name { get; set; }
        public dynamic Predicate;

        // ------------------------------------------------

        public Belief(eTypesBelief name, dynamic predicate)
        {
            Name = name;
            Predicate = predicate;
        }

        // ------------------------------------------------

        public override string ToString()
        {
            var cr = Environment.NewLine;
            var retVal = new StringBuilder();
            var coord = Predicate as List<Tuple<int, int>>;

            foreach(var c in coord)
            {
                retVal.Append($"{Name} ({c.Item1},{c.Item2}){cr}");
            }
            
            return retVal.ToString();
        }
    }
}
