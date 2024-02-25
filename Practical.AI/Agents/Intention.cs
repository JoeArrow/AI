using System.Collections.Generic;

namespace Practical.AI.Agents
{
    // ------------------------------------------------

    public class Intention: Desire
    {
        public static Intention FromDesire(Desire desire)
        {
            var result = new Intention
                             {
                                 Name = desire.Name,
                                 SubDesires = new List<Desire>(desire.SubDesires),
                                 Predicate = desire.Predicate
                             };

            return result;
        }
    }
}
