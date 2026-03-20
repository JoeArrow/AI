#region © 2026 Joe Arrowood (JoeWare)
//
// All rights reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical, or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
#endregion

using LogicPuzzle.Core.Clues;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicPuzzle.Core.Tests.Clues
{
    // ----------------------------------------------------
    /// <summary>
    ///     Summary description for ArrowUnitTestXML1
    /// </summary>

    [TestClass]
    public class MatchClueTests
    {
        public MatchClueTests() { }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("Alice does not own a Dog", null, null, null, null)]
        [DataRow("Alice does not own a Dog", "People", null, null, null)]
        [DataRow("Alice does not own a Dog", "People", "Alice", null, null)]
        [DataRow("Alice does not own a Dog", "People", "Alice", "Pets", null)]
        [DataRow("Alice does not own a Dog", "People", "Alice", "People", "Alice")]
        public void Constructor_MatchClue(string clueText, string cat1, string item1, string cat2, string item2)
        {
            // ----------
            // Act/Assert

            Assert.ThrowsExactly<ArgumentException>(() => new MatchClue(clueText, cat1, item1, cat2, item2));
        }

        // ------------------------------------------------

        [TestMethod]
        public void Apply_MatchClue()
        {
            // -------
            // Arrange

            var sut = new MatchClue("Alice does not own a Dog", "People", "Alice", "Pets", "Dog");

            var categories = new List<Category>
            {
                new Category("People", new List<CategoryItem> { new CategoryItem("Alice"),
                                                                new CategoryItem("Dan"),
                                                                new CategoryItem("Joe") }),

                new Category("Pets", new List<CategoryItem> { new CategoryItem("Dog"),
                                                              new CategoryItem("Lizard"),
                                                              new CategoryItem("Bat") })
            };

            var grid = new WorkingGrid(categories);

            // ---
            // Act

            sut.Apply(grid);

            // ------
            // Assert

            Assert.AreEqual(CellState.No, grid.GetState("People", "Dan", "Pets", "Dog"));
            Assert.AreEqual(CellState.No, grid.GetState("People", "Joe", "Pets", "Dog"));
            Assert.AreEqual(CellState.Yes, grid.GetState("People", "Alice", "Pets", "Dog"));
        }

        // ------------------------------------------------
        // With this 2 x 2 grid, the Solver should be able
        // to deduce the remaining cells.

        [TestMethod]
        public void Apply_Deduction_MatchClue()
        {
            // -------
            // Arrange

            var sut = new MatchClue("Alice does not own a Dog", "People", "Alice", "Pets", "Dog");

            var categories = new List<Category>
            {
                new Category("People", new List<CategoryItem> { new CategoryItem("Alice"),
                                                                new CategoryItem("Dan") }),

                new Category("Pets", new List<CategoryItem> { new CategoryItem("Dog"),
                                                              new CategoryItem("Lizard") })
            };

            var grid = new WorkingGrid(categories);

            // ---
            // Act

            sut.Apply(grid);

            // ------
            // Assert

            Assert.AreEqual(CellState.No, grid.GetState("People", "Dan", "Pets", "Dog"));
            Assert.AreEqual(CellState.Yes, grid.GetState("People", "Alice", "Pets", "Dog"));
            Assert.AreEqual(CellState.Yes, grid.GetState("People", "Dan", "Pets", "Lizard"));
            Assert.AreEqual(CellState.No, grid.GetState("People", "Alice", "Pets", "Lizard"));
        }

        // ------------------------------------------------

        [TestMethod]
        public void Apply_MatchClue_Throws()
        {
            // -------
            // Arrange

            var sut = new MatchClue("Alice does not own a Dog", "People", "Alice", "Pets", "Dog");

            // ----------
            // Act/Assert

            Assert.ThrowsExactly<ArgumentNullException>(() => sut.Apply(null));
        }

        // ------------------------------------------------

        [TestMethod]
        public void Apply_MatchClue_Throws2()
        {
            // -------
            // Arrange

            var sut = new MatchClue("Alice does not own a Dog", "People", "Alice", "Pets", "Dog");

            // ----------
            // Act/Assert

            Assert.ThrowsExactly<ArgumentNullException>(() => sut.Apply(null));
        }
    }
}