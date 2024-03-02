using System;
using System.Text;
using System.Collections.Generic;

namespace Practical.AI.Agents
{
    public class Belief
    {
        public dynamic Predicate;
        public eTypesBelief Name { get; set; }

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
            var result = new StringBuilder();
            var path = Predicate as List<Tuple<int, int>>;

            foreach(var cell in path)
            {
                result.Append($"{Name} ({cell.Item1},{cell.Item2}){cr}");
            }
            
            return result.ToString();
        }
    }
}
