using System;

namespace Practical.AI.Simulation.Airport.Objects
{
    public class Airplane
    {
        public Guid Id { get; set; }
        public bool BrokenDown { get; set; }
        public int RunwayOccupied { get; set; }
        public int PassengersCount { get; set; }
        public double TimeToTakeOff { get; set; }

        public Airplane(int passengers)
        {
            Id = Guid.NewGuid();
            RunwayOccupied = -1;
            PassengersCount = passengers;
        }
    }
}
