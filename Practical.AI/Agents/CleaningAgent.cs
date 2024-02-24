using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Practical.AI.Agents
{
    public class CleaningAgent
    {
        public int X { get; set; }
        public int Y { get; set; }
        private readonly int[,] _terrain;
        private static Stopwatch _stopwatch;
        public bool TaskFinished { get; set; }
        public bool _expired = false;
        private Random _random;

        // -----------------------------------------
        // Internal data structure for keeping state
        
        private List<Tuple<int, int>> _cleanedCells;

        // ------------------------------------------------
        //                                      Constructor

        public CleaningAgent(int [,] terrain, int x, int y)
        {
            X = x;
            Y = y;

            _terrain = new int[terrain.GetLength(0), terrain.GetLength(1)];
            Array.Copy(terrain, _terrain, terrain.GetLength(0) * terrain.GetLength(1));

            _stopwatch = new Stopwatch();
            _cleanedCells = new List<Tuple<int, int>>();
            _random = new Random();
        }

        // ------------------------------------------------
        //                                            Start

        public void Start(int miliseconds)
        {
            _stopwatch.Start();

            do
            {
                Action(Perceived());
                _expired = _stopwatch.ElapsedMilliseconds > miliseconds;
            }
            while (!TaskFinished && !(_stopwatch.ElapsedMilliseconds > miliseconds));

            _stopwatch.Stop();
        }

        // ------------------------------------------------
        //                                      UpdateState

        private void UpdateState()
        {
            var room = new Tuple<int, int>(X, Y);

            if(!_cleanedCells.Contains(room)) 
            { 
                _cleanedCells.Add(room); 
            }
        }

        // ------------------------------------------------
        //                                   Function Clean

        public void Clean()
        {
            // ---------------------------
            // Once we enter a dirty room,
            // clean the room completely

            while(_terrain[X, Y] > 0) { _terrain[X, Y] -= 1; }
        }

        // ------------------------------------------------
        //                                Predicate IsDirty

        public bool IsDirty()
        {
            return _terrain[X, Y] > 0;
        }

        // ------------------------------------------------
        //                                           Action

        public void Action(List<Percepts> percepts)
        {
            if(percepts.Contains(Percepts.Clean)) { UpdateState(); }

            if(percepts.Contains(Percepts.Dirty)) { Clean(); }
            else if(percepts.Contains(Percepts.Finished)) { TaskFinished = true; }
            else if(percepts.Contains(Percepts.MoveUp) && !_cleanedCells.Contains(new Tuple<int, int>(X - 1, Y))) { Move(Percepts.MoveUp); }
            else if(percepts.Contains(Percepts.MoveDown) && !_cleanedCells.Contains(new Tuple<int, int>(X + 1, Y))) { Move(Percepts.MoveDown); }
            else if(percepts.Contains(Percepts.MoveLeft) && !_cleanedCells.Contains(new Tuple<int, int>(X, Y - 1))) { Move(Percepts.MoveLeft); }
            else if(percepts.Contains(Percepts.MoveRight) && !_cleanedCells.Contains(new Tuple<int, int>(X, Y + 1))) { Move(Percepts.MoveRight); }
            else { RandomAction(percepts); }
        }

        // ------------------------------------------------
        //                                     RandomAction

        private void RandomAction(List<Percepts> perceptions)
        {
            var perception = perceptions[_random.Next(1, perceptions.Count)];
            Move(perception);
        }

        // ------------------------------------------------
        //                                             Move

        private void Move(Percepts p)
        {
            switch (p)
            {
                case Percepts.MoveUp:
                    X -= 1;
                    break;

                case Percepts.MoveDown:
                    X += 1;
                    break;

                case Percepts.MoveLeft:
                    Y -= 1;
                    break;

                case Percepts.MoveRight:
                    Y += 1;
                    break;
            }
        }

        // ------------------------------------------------
        //                               Function Perceived

        private List<Percepts> Perceived()
        {
            var retVal = new List<Percepts>();

            if(IsDirty()) { retVal.Add(Percepts.Dirty); }
            else { retVal.Add(Percepts.Clean); }

            if(_cleanedCells.Count == _terrain.GetLength(0) * _terrain.GetLength(1)) { retVal.Add(Percepts.Finished); }

            if(MoveAvailable(X - 1, Y)) { retVal.Add(Percepts.MoveUp); }
            if(MoveAvailable(X + 1, Y)) { retVal.Add(Percepts.MoveDown); }
            if(MoveAvailable(X, Y - 1)) { retVal.Add(Percepts.MoveLeft); }
            if(MoveAvailable(X, Y + 1)) { retVal.Add(Percepts.MoveRight); }
            
            return retVal;
        }

        // ------------------------------------------------
        //                          Predicate MoveAvailable

        public bool MoveAvailable(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _terrain.GetLength(0) && y < _terrain.GetLength(1);
        }

        // ------------------------------------------------
        //                                            Print

        public void Print()
        {
            var i = 0;
            var line = string.Empty;
            var col = _terrain.GetLength(1);

            var width = _terrain.GetLength(1) * 7;

            Console.WriteLine(new string('=', width));
            Console.WriteLine($"{new string(' ', (width / 2) - 2)}{_stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Alloted time expired: {_expired}");
            Console.WriteLine(new string('-', width));

            foreach (var c in _terrain)
            {
                line += string.Format("  {0,3}  ", c);
                i++;

                if (col == i)
                {
                    Console.WriteLine(line);
                    line = string.Empty;
                    i = 0;
                }
            }
        }
    }

    // ------------------------------------------------

    public enum Percepts
    {
        Dirty, 
        Clean, 
        Finished,
        MoveUp, 
        MoveDown, 
        MoveLeft, 
        MoveRight  
    }
}
