using System.Collections.Generic;

namespace Practical.AI.Agents
{
    public class Desire
    {
        public eTypesDesire Name { get; set; }
        public dynamic Predicate;
        public List<Desire> SubDesires { get; set; }

        // ------------------------------------------------

        public Desire() 
        { 
            SubDesires = new List<Desire>(); 
        }

        // ------------------------------------------------

        public Desire(eTypesDesire name)
            : this()
        {
            Name = name;
        }

        // ------------------------------------------------

        public Desire(eTypesDesire name, dynamic predicate)
            : this(name)
        {
            Predicate = predicate;
        }

        // ------------------------------------------------

        public Desire(eTypesDesire name, IEnumerable<Desire> subDesires)
            :this(name)
        {
            SubDesires = new List<Desire>(subDesires);
        }

        // ------------------------------------------------

        public Desire(eTypesDesire name, params Desire[] subDesires)
            : this(name)
        {
            SubDesires = new List<Desire>(subDesires);
        }

        // ------------------------------------------------

        public List<Desire> GetSubDesires()
        {
            if(SubDesires.Count == 0)
            {
                return new List<Desire>() { this };
            }

            var result = new List<Desire>();

            foreach(var desire in SubDesires)
            {
                result.AddRange(desire.GetSubDesires());
            }

            return result;
        }

        // ------------------------------------------------

        public override string ToString()
        {
            return Name.ToString() + "\n";
        }
    }
}
