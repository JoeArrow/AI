using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Practical.AI.Agents;

namespace Practical.AI.UnitTests
{
    [TestClass]
    public class CleaningRobot_UnitTests
    {
        [TestMethod]
        [DataRow(1, 5, 4, 4, 1)]
        [DataRow(1, 5, 4, 4, 5)]
        [DataRow(1, 50, 5, 5, 50)]
        [DataRow(1, 5, 4, 4, 500)]
        [DataRow(1, 5, 4, 4, 2000)]
        [DataRow(1, 5, 4, 4, 10000)]
        public void TestMethod1(int min, int max, int width, int height, int limit)
        {
            var terrain = new int[width, height];
            var random = new Random();

            for(int i = 0; i < terrain.GetLength(0); i++)
            {
                for(int j = 0; j < terrain.GetLength(1); j++)
                {
                    terrain[i, j] = random.Next(min, max);
                }
            }

            var cleaningRobot = new CleaningAgent(terrain, 0, 0);
            cleaningRobot.Print();
            cleaningRobot.Start(limit);
            cleaningRobot.Print();
        }
    }
}
