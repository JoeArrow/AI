using System;
using System.Collections.Generic;

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
            var result = "";
            var coord = Predicate as List<Tuple<int, int>>;

            foreach(var c in coord)
            {
                result += Name + " (" + c.Item1 + "," + c.Item2 + ")" + "\n";
            }
            
            return result;
        }
    }
}
