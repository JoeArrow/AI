using System;
using System.Linq;
using System.Collections.Generic;

namespace Practical.AI.Agents
{
    public class MarsRover
    {
        public int X { get; set; }
        public int Y { get; set; }
        public IDomain Mars { get; set; }
        public int SenseRadius { get; set; }
        public Plan CurrentPlan { get; set; }
        public List<Belief> Beliefs { get; set; }
        public Queue<Desire> Desires { get; set; }
        public List<Plan> PlanLibrary { get; set; }
        public double ObstacleThreshold { get; set; }
        public Stack<Intention> Intentions { get; set; }
        public List<Tuple<int, int>> WaterFound { get; set; }

        // ---------------------------------------------------------
        // Identifies the last part of the terrain seen by the Rover

        public List<Tuple<int, int>> CurrentTerrain { get; set; }

        private int _wanderTimes;
        private double[,] _terrain;
        private static Random _random;
        private const int WanderThreshold = 10;
        private Dictionary<Tuple<int, int>, int> _perceivedCells;

        // ------------------------------------------------

        public MarsRover(DomainRequest req)
        {
            X = req.X;
            Y = req.Y;
            Mars = req.Domain;

            _random = new Random();
            SenseRadius = req.SenseRadius;
            ObstacleThreshold = req.ObstacleThreshold;

            Desires = new Queue<Desire>();
            Intentions = new Stack<Intention>();
            WaterFound = new List<Tuple<int, int>>();
            Beliefs = new List<Belief>(req.InitialBeliefs);
            CurrentTerrain = new List<Tuple<int, int>>();
            _perceivedCells = new Dictionary<Tuple<int, int>, int>();
            _terrain = new double[req.Terrain.GetLength(0), req.Terrain.GetLength(1)];
            PlanLibrary = new List<Plan> { new  Plan(eTypesPlan.PathFinding, this), };
            Array.Copy(req.Terrain, _terrain, req.Terrain.GetLength(0) * req.Terrain.GetLength(1));
        }

        // ------------------------------------------------

        public MarsRover(IDomain mars, double[,] terrain, int x, int y, 
                         IEnumerable<Belief> initialBeliefs, double obstacle, int senseRadius)
            : this(new DomainRequest() { X = x, Y = y, Domain = mars, SenseRadius = senseRadius, Terrain = terrain,
                                         ObstacleThreshold = obstacle, InitialBeliefs = initialBeliefs, }) { }

        // ------------------------------------------------
        /// <summary>
        ///     Percepts function
        /// </summary>
        /// <returns></returns>

        public List<Percept> GetPercepts()
        {
            var result = new List<Percept>();

            if(MoveAvailable(X - 1, Y)) { result.Add(new Percept(new Tuple<int, int>(X - 1, Y), eTypePercept.MoveUp)); }

            if(MoveAvailable(X + 1, Y)) { result.Add(new Percept(new Tuple<int, int>(X + 1, Y), eTypePercept.MoveDown)); }

            if(MoveAvailable(X, Y - 1)) { result.Add(new Percept(new Tuple<int, int>(X, Y - 1), eTypePercept.MoveLeft)); }

            if(MoveAvailable(X, Y + 1)) { result.Add(new Percept(new Tuple<int, int>(X, Y + 1), eTypePercept.MoveRight)); }

            result.AddRange(LookAround());

            return result;
        }

        // ------------------------------------------------

        public IEnumerable<Percept> GetCurrentTerrain()
        {
            var R = SenseRadius;
            CurrentTerrain.Clear();
            var result = new List<Percept>();

            for(var i = X - R > 0 ? X - R : 0; i <= X + R; i++)
            {
                for(var j = Y; Math.Pow((j - Y), 2) + Math.Pow((i - X), 2) <= Math.Pow(R, 2); j--)
                {
                    if(j < 0 || i >= _terrain.GetLength(0)) { break; }

                    // -------------
                    // In the circle

                    result.AddRange(CheckTerrain(Mars.TerrainAt(i, j), new Tuple<int, int>(i, j)));
                    CurrentTerrain.Add(new Tuple<int, int>(i, j));
                    UpdatePerceivedCellsDicc(new Tuple<int, int>(i, j));
                }

                for(var j = Y + 1; (j - Y) * (j - Y) + (i - X) * (i - X) <= R * R; j++)
                {
                    if(j >= _terrain.GetLength(1) || i >= _terrain.GetLength(0)) { break; }

                    // -------------
                    // In the circle

                    result.AddRange(CheckTerrain(Mars.TerrainAt(i, j), new Tuple<int, int>(i, j)));
                    CurrentTerrain.Add(new Tuple<int, int>(i, j));
                    UpdatePerceivedCellsDicc(new Tuple<int, int>(i, j));
                }
            }

            return result;
        }

        // ------------------------------------------------

        private void UpdatePerceivedCellsDicc(Tuple<int, int> position)
        {
            if(!_perceivedCells.ContainsKey(position))
            {
                _perceivedCells.Add(position, 0);
            }

            _perceivedCells[position]++;
        }

        // ------------------------------------------------
        /// <summary>
        ///     Look around the rover.
        /// </summary>
        /// <returns></returns>

        private IEnumerable<Percept> LookAround()
        {
            return GetCurrentTerrain();
        }

        // ------------------------------------------------
        /// <summary>
        ///     Check a given cell in the terrain
        /// </summary>
        /// <param name="cell">Value of the cell</param>
        /// <param name="position">Its coordenates</param>
        /// <returns></returns>

        private IEnumerable<Percept> CheckTerrain(double cell, Tuple<int, int> position)
        {
            var result = new List<Percept>();

            if(cell > ObstacleThreshold)
            {
                result.Add(new Percept(position, eTypePercept.Obstacle));
            }
            else if(cell < 0)
            {
                result.Add(new Percept(position, eTypePercept.WaterSpot));
            }

            _terrain[position.Item1, position.Item2] = cell;

            return result;
        }

        // ------------------------------------------------
        /// <summary>
        ///     Determines whether a move to cell (x, y) is available.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>

        public bool MoveAvailable(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _terrain.GetLength(0) && y < _terrain.GetLength(1) && _terrain[x, y] < ObstacleThreshold;
        }

        // ------------------------------------------------

        public eTypesAction Action(List<Percept> percepts)
        {
            // --------------
            // Reactive Layer

            if(Mars.WaterAt(X, Y) && !WaterFound.Contains(new Tuple<int, int>(X, Y)))
            {
                return eTypesAction.Dig;
            }

            var waterPercepts = percepts.FindAll(p => p.Type == eTypePercept.WaterSpot);

            if(waterPercepts.Count > 0)
            {
                foreach(var waterPercept in waterPercepts)
                {
                    var belief = Beliefs.FirstOrDefault(b => b.Name == eTypesBelief.PotentialWaterSpots);
                    List<Tuple<int, int>> pred;

                    if(belief != null)
                    {
                        pred = belief.Predicate as List<Tuple<int, int>>;
                    }
                    else
                    {
                        pred = new List<Tuple<int, int>> { waterPercept.Position };
                        Beliefs.Add(new Belief(eTypesBelief.PotentialWaterSpots, pred));
                    }

                    if(!WaterFound.Contains(waterPercept.Position)) 
                    { 
                        pred.Add(waterPercept.Position); 
                    }
                    else
                    {
                        pred.RemoveAll(t => t.Item1 == waterPercept.Position.Item1 && t.Item2 == waterPercept.Position.Item2);

                        if(pred.Count == 0) 
                        { 
                            Beliefs.RemoveAll(b => (b.Predicate as List<Tuple<int, int>>).Count == 0); 
                        }
                    }
                }

                if(waterPercepts.Any(p => !WaterFound.Contains(p.Position))) { CurrentPlan = null; }
            }

            if(Beliefs.Count == 0)
            {
                if(_wanderTimes == WanderThreshold)
                {
                    _wanderTimes = 0;
                    InjectBelief();
                }
                _wanderTimes++;
                return RandomMove(percepts);
            }

            if(CurrentPlan == null || CurrentPlan.FulFill())
            {
                // ------------------
                // Deliberative Layer

                Brf(percepts);
                Options();
                Filter();
            }

            return CurrentPlan.NextAction();
        }

        // ------------------------------------------------

        private void InjectBelief()
        {
            var halfC = _terrain.GetLength(1) / 2;
            var halfR = _terrain.GetLength(0) / 2;

            var firstSector = _perceivedCells.Where(k => k.Key.Item1 < halfR && k.Key.Item2 < halfC).ToList();
            var secondSector = _perceivedCells.Where(k => k.Key.Item1 < halfR && k.Key.Item2 >= halfC).ToList();

            var thirdSector = _perceivedCells.Where(k => k.Key.Item1 >= halfR && k.Key.Item2 < halfC).ToList();
            var fourthSector = _perceivedCells.Where(k => k.Key.Item1 >= halfR && k.Key.Item2 >= halfC).ToList();

            var freq1stSector = SetRelativeFreq(firstSector);
            var freq2ndSector = SetRelativeFreq(secondSector);
            var freq3rdSector = SetRelativeFreq(thirdSector);
            var freq4thSector = SetRelativeFreq(fourthSector);

            var min = Math.Min(freq1stSector, Math.Min(freq2ndSector, Math.Min(freq3rdSector, freq4thSector)));

            if(min == freq1stSector)
                Beliefs.Add(new Belief(eTypesBelief.PotentialWaterSpots, new List<Tuple<int, int>>
                { new Tuple<int, int>(0, 0) }));

            else if(min == freq2ndSector)
                Beliefs.Add(new Belief(eTypesBelief.PotentialWaterSpots, new List<Tuple<int, int>> 
                { new Tuple<int, int>(0, _terrain.GetLength(1) - 1) }));

            else if(min == freq3rdSector)
                Beliefs.Add(new Belief(eTypesBelief.PotentialWaterSpots, new List<Tuple<int, int>> 
                { new Tuple<int, int>(_terrain.GetLength(0) - 1, 0) }));

            else
                Beliefs.Add(new Belief(eTypesBelief.PotentialWaterSpots, new List<Tuple<int, int>> 
                { new Tuple<int, int>(_terrain.GetLength(0) - 1, _terrain.GetLength(1) - 1) }));
        }

        // ------------------------------------------------

        private double SetRelativeFreq(List<KeyValuePair<Tuple<int, int>, int>> cells)
        {
            var result = 0.0;

            foreach(var cell in cells)
            {
                result += RelativeFrequency(cell.Value, cells.Count);
            }

            return result;
        }

        // ------------------------------------------------

        private double RelativeFrequency(int absFreq, int n)
        {
            return (double) absFreq / n;
        }

        // ------------------------------------------------

        private eTypesAction RandomMove(List<Percept> percepts)
        {
            var retVal = eTypesAction.None;
            var moves = percepts.FindAll(p => p.Type.ToString().Contains("Move"));
            var selectedMove = moves[_random.Next(0, moves.Count)];

            switch(selectedMove.Type)
            {
                case eTypePercept.MoveUp:
                    retVal = eTypesAction.MoveUp;
                    break;

                case eTypePercept.MoveDown:
                    retVal = eTypesAction.MoveDown;
                    break;

                case eTypePercept.MoveRight:
                    retVal = eTypesAction.MoveRight;
                    break;

                case eTypePercept.MoveLeft:
                    retVal = eTypesAction.MoveLeft;
                    break;
            }

            return retVal;
        }

        // ------------------------------------------------

        public void ExecuteAction(eTypesAction action, List<Percept> percepts)
        {
            switch(action)
            {
                case eTypesAction.MoveUp:
                    X -= 1;
                    break;

                case eTypesAction.MoveDown:
                    X += 1;
                    break;

                case eTypesAction.MoveLeft:
                    Y -= 1;
                    break;

                case eTypesAction.MoveRight:
                    Y += 1;
                    break;

                case eTypesAction.Dig:
                    WaterFound.Add(new Tuple<int, int>(X, Y));
                    break;
            }
        }

        // ------------------------------------------------
        /// <summary>
        ///     Beliefs revision function
        /// </summary>
        /// <param name="percepts"></param>

        public void Brf(List<Percept> percepts)
        {
            var newBeliefs = new List<Belief>();

            foreach(var b in Beliefs)
            {
                switch(b.Name)
                {
                    case eTypesBelief.PotentialWaterSpots:
                        var waterSpots = new List<Tuple<int, int>>(b.Predicate);
                        waterSpots = UpdateBelief(eTypesBelief.PotentialWaterSpots, waterSpots);

                        if(waterSpots.Count > 0)
                        {
                            newBeliefs.Add(new Belief(eTypesBelief.PotentialWaterSpots, waterSpots));
                        }

                        break;

                    case eTypesBelief.ObstaclesOnTerrain:
                        var obstacleSpots = new List<Tuple<int, int>>(b.Predicate);
                        obstacleSpots = UpdateBelief(eTypesBelief.ObstaclesOnTerrain, obstacleSpots);

                        if(obstacleSpots.Count > 0)
                        {
                            newBeliefs.Add(new Belief(eTypesBelief.ObstaclesOnTerrain, obstacleSpots));
                        }

                        break;
                }
            }

            Beliefs = new List<Belief>(newBeliefs);
        }

        // ------------------------------------------------
        /// <summary>
        ///     Updates set of beliefs.
        /// </summary>
        /// <param name="belief"> </param>
        /// <param name="beliefPos"></param>
        /// <returns></returns>

        private List<Tuple<int, int>> UpdateBelief(eTypesBelief belief, IEnumerable<Tuple<int, int>> beliefPos)
        {
            var result = new List<Tuple<int, int>>();

            foreach(var spot in beliefPos)
            {
                if(CurrentTerrain.Contains(new Tuple<int, int>(spot.Item1, spot.Item2)))
                {
                    switch(belief)
                    {
                        case eTypesBelief.PotentialWaterSpots:
                            if(_terrain[spot.Item1, spot.Item2] >= 0) { continue; }
                            break;

                        case eTypesBelief.ObstaclesOnTerrain:
                            if(_terrain[spot.Item1, spot.Item2] < ObstacleThreshold) { continue; }
                            break;
                    }
                }
                result.Add(spot);
            }

            return result;
        }

        // ------------------------------------------------
        /// <summary>
        ///     Generates desires.
        /// </summary>

        public void Options()
        {
            Desires.Clear();

            foreach(var b in Beliefs)
            {
                if(b.Name == eTypesBelief.PotentialWaterSpots)
                {
                    var waterPos = b.Predicate as List<Tuple<int, int>>;

                    waterPos.Sort(delegate (Tuple<int, int> tupleA, Tuple<int, int> tupleB)
                    {
                        var distA = ManhattanDistance(tupleA, new Tuple<int, int>(X, Y));
                        var distB = ManhattanDistance(tupleB, new Tuple<int, int>(X, Y));
                    
                        if(distA < distB)
                        {
                            return 1;
                        }
                              
                        if(distA > distB)
                        {
                            return -1;
                        }
                    
                        return 0;
                    });

                    foreach(var wPos in waterPos)
                    {
                        Desires.Enqueue(new Desire(eTypesDesire.FindWater, 
                                                   new Desire(eTypesDesire.GotoLocation, 
                                                              new Desire(eTypesDesire.Dig, wPos))));
                    }
                }
            }
        }

        // ------------------------------------------------
        /// <summary>
        ///     Determines which desires will become intentions 
        ///     or which intentions should remain or be deleted.
        /// </summary>
        /// <param name="percepts"></param>

        private void Filter()
        {
            Intentions.Clear();

            foreach(var desire in Desires)
            {
                if(desire.SubDesires.Count > 0)
                {
                    var primaryDesires = desire.GetSubDesires();
                    primaryDesires.Reverse();

                    foreach(var d in primaryDesires)
                    {
                        Intentions.Push(Intention.FromDesire(d));
                    }
                }
                else
                {
                    Intentions.Push(Intention.FromDesire(desire));
                }
            }

            if(Intentions.Any() && !ExistsPlan()) { ChoosePlan(); }
        }

        // ------------------------------------------------

        private void ChoosePlan()
        {
            var primaryIntention = Intentions.Pop();
            var location = primaryIntention.Predicate as Tuple<int, int>;

            switch(primaryIntention.Name)
            {
                case eTypesDesire.Dig:
                    CurrentPlan = PlanLibrary.First(p => p.Name == eTypesPlan.PathFinding);
                    CurrentPlan.BuildPlan(new Tuple<int, int>(X, Y), location);
                    break;
            }
        }

        // ------------------------------------------------

        public bool ExistsPlan()
        {
            return CurrentPlan != null && CurrentPlan.Path.Count > 0;
        }

        // ------------------------------------------------

        public int ManhattanDistance(Tuple<int, int> x, Tuple<int, int> y)
        {
            return Math.Abs(x.Item1 - y.Item1) + Math.Abs(x.Item2 - y.Item2);
        }
    }

    // ------------------------------------------------

    public enum eTypePercept
    {
        WaterSpot, 
        Obstacle, 
        MoveUp, 
        MoveDown, 
        MoveLeft, 
        MoveRight
    }

    // ------------------------------------------------

    public enum eTypesBelief
    {
        PotentialWaterSpots, 
        ObstaclesOnTerrain
    }

    // ------------------------------------------------

    public enum eTypesDesire
    {
        FindWater, 
        GotoLocation, 
        Dig
    }

    // ------------------------------------------------

    public enum eTypesPlan
    {
        PathFinding
    }

    // ------------------------------------------------

    public enum eTypesAction
    {
        MoveUp, 
        MoveDown, 
        MoveLeft, 
        MoveRight, 
        Dig,
        None
    }
}
