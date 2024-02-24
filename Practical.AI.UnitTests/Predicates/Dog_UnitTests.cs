using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Practical.AI.Predicates;

namespace Practical.AI.UnitTests.Predicates
{
    [TestClass]
    public class Dog_UnitTests
    { 

        // ------------------------------------------------

        [TestMethod]
        [DataRow(3)]
        public void FindAll_Dog(int expected)
        {
            // -------
            // Arrange

            var jack = new Dog("Jack", 23.5, Gender.Male);
            var jordan = new Dog("Jack", 21.2, Gender.Male);
            var johnny = new Dog("Johnny", 17.5, Gender.Male);
            var melissa = new Dog("Melissa", 19.7, Gender.Female);

            var dogs = new List<Dog> { johnny, jack, jordan, melissa };

            Predicate<Dog> maleFinder = (Dog d) => { return d.Sex == Gender.Male; };
            Predicate<Dog> heavyDogsFinder = (Dog d) => { return d.Weight >= 22; };

            // ---
            // Act

            var maleDogs = dogs.FindAll(maleFinder);

            // ------
            // Assert

            Assert.AreEqual(expected, maleDogs.Count);
        }
    }
}
