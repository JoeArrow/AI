using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Practical.AI.Agents;

namespace Practical.AI.UnitTests.Agents
{
    [TestClass]
    public class MarsRover_UnitTests
    {
        private double[,] terrain = new double[,]
        {
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0.8, -1, 0, 0, 0, 0, 0, 0},
                {0, 0, 0.8, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0.8, 0, 0, 0, 0},
                {0, -1, 0, 0, 0, 0, 0, 0, 0, 0}
        };

        private double[,] roverTerrain = new[,]
        {
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0.8, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0.8, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0.8, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
            };

        private static List<Tuple<int, int>> water = new List<Tuple<int, int>>
        {
            new Tuple<int, int>(1, 2),
            new Tuple<int, int>(3, 5),
        };

        private static List<Tuple<int, int>> obstacles = new List<Tuple<int, int>>
        {
            new Tuple<int, int> (2, 2),
            new Tuple<int, int> (4, 5),
        };

        private List<Belief> beliefs = new List<Belief>
        {
            new Belief(eTypesBelief.PotentialWaterSpots, water),
            new Belief(eTypesBelief.ObstaclesOnTerrain, obstacles),
        };

        // ------------------------------------------------

        [TestMethod]
        [DataRow(7, 8, 0.75, 2)]
        public void Constructor_MarsRover(int x, int y, double obsticalThreshold, int senseRadious)
        {
            // -------
            // Arrange

            var mars = new Mars(terrain);

            // ---
            // Act

            var sut = new MarsRover(mars, terrain, x, y, beliefs, obsticalThreshold, senseRadious);

            // ------
            // Assert

            Assert.IsNotNull(sut);
            Assert.AreEqual(x, sut.X);
            Assert.AreEqual(y, sut.Y);
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow(7, 8, 0.75, 2, 4)]
        public void GetPercepts_MarsRover(int x, int y, double obsticalThreshold, int senseRadious, int expected)
        {
            // -------
            // Arrange

            var mars = new Mars(terrain);
            var sut = new MarsRover(mars, terrain, x, y, beliefs, obsticalThreshold, senseRadious);

            // ---
            // Act

            var resp = sut.GetPercepts();

            // ------
            // Assert

            Assert.AreEqual(expected, resp.Count);
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow(0.0, 7, 8, 0.75, 2, 0)]
        [DataRow(1.0, 7, 8, 0.75, 2, 1)]
        public void GetCurrentTerrain_MarsRover(double testVal, int x, int y, double obsticalThreshold, int senseRadious, int expected)
        {
            // -------
            // Arrange

            terrain[x, y] = testVal;
            var mars = new Mars(terrain);
            var sut = new MarsRover(mars, terrain, x, y, beliefs, obsticalThreshold, senseRadious);

            // ---
            // Act

            var resp = sut.GetCurrentTerrain();

            // ------
            // Assert

            Assert.AreEqual(expected, resp.Count());
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow(1.0, 7, 8, 0.75, 2, eTypePercept.WaterSpot, eTypesAction.MoveUp)]
        public void Action_MarsRover(double testVal, int x, int y, double obsticalThreshold, int senseRadious, eTypePercept percept, eTypesAction expected)
        {
            // -------
            // Arrange

            terrain[x, y] = testVal;
            var mars = new Mars(terrain);
            var sut = new MarsRover(mars, terrain, x, y, beliefs, obsticalThreshold, senseRadious);
            var percepts = sut.GetPercepts();
            percepts.Add(new Percept(new Tuple<int, int>(5, 5), percept));

            // ---
            // Act

            var resp = sut.Action(percepts);

            // ------
            // Assert

            Assert.AreEqual(expected, resp);
        }
    }
}
