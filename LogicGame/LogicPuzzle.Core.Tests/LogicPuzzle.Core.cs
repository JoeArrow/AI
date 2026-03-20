#region © 2026 Joe Arrowood (JoeWare)
//
// All rights reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical, or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
#endregion

using LogicPuzzle.Core;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicPuzzle.Core.Tests
{
    // ----------------------------------------------------
    /// <summary>
    ///     Summary description for ArrowUnitTestXML1
    /// </summary>

    [TestClass]
    public class LogicPuzzleTests
    {
        public LogicPuzzleTests() { }

        // ------------------------------------------------

        [TestMethod]
        public void Constructor_Category()
        {
            // -------
            // Arrange

            var bob = new CategoryItem("Bob");
            var alice = new CategoryItem("Alice");
            var carol = new CategoryItem("Carol");

            // ---
            // Act

            Category category = new Category("People",
                                             new List<CategoryItem>
                                             {
                                                 bob,
                                                 alice,
                                                 carol
                                             });

            // ------
            // Assert

            Assert.AreEqual("People", category.Name);
            Assert.AreEqual(3, category.Items.Count);

            Assert.IsTrue(category.Items.Contains(bob));
            Assert.IsTrue(category.Items.Contains(alice));
            Assert.IsTrue(category.Items.Contains(carol));

            Assert.AreEqual("Bob", category.Items[0].Name);
            Assert.AreEqual("Alice", category.Items[1].Name);
            Assert.AreEqual("Carol", category.Items[2].Name);
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow(
            "People",
            "['Alice','Bob','Carol']",
            "Pets",
            "['Cat','Dog','Fish']",
            "['Alice','Cat','Fish','Dog']")]
        [DataRow(
            "People",
            "['Alice','Bob','Carol']",
            "Pets",
            "['Cat','Dog','Fish']",
            "['Bob','Dog','Fish','Cat']")]
        [DataRow(
            "People",
            "['Alice','Bob','Carol']",
            "Pets",
            "['Cat','Dog','Fish']",
            "['Carol','Cat','Dog','Fish']")]
        public void SinglePossibility_TriggersYes(string catName1, string catItems1JSON, string catName2, string catItems2JSON, string valuesJSON)
        {
            // -------
            // Arrange

            var catItems1 = JsonSerializer.Deserialize<List<string>>(catItems1JSON.Replace("\'", "\""));
            var catItems2 = JsonSerializer.Deserialize<List<string>>(catItems2JSON.Replace("\'", "\""));
            var values = JsonSerializer.Deserialize<string[]>(valuesJSON.Replace("\'", "\""));

            var categories = new List<Category>
            {
                new Category(catName1, CreateCategoryItems(catItems1)),
                new Category(catName2, CreateCategoryItems(catItems2))
            };

            var itemName1 = values[0];
            var noItemName1 = values[1];
            var noItemName2 = values[2];
            var expectedItemName = values[3];

            WorkingGrid grid = new WorkingGrid(categories);

            grid.SetNo(catName1, itemName1, catName2, noItemName1);
            grid.SetNo(catName1, itemName1, catName2, noItemName2);

            Assert.AreEqual(
                CellState.No,
                grid.GetState(catName1, itemName1, catName2, noItemName1));

            Assert.AreEqual(
                CellState.No,
                grid.GetState(catName1, itemName1, catName2, noItemName2));

            Assert.AreEqual(
                CellState.Yes,
                grid.GetState(catName1, itemName1, catName2, expectedItemName));
        }

        // ------------------------------------------------

        private static List<CategoryItem> CreateCategoryItems(List<string> names)
        {
            List<CategoryItem> items = new List<CategoryItem>();

            foreach(string name in names)
            {
                items.Add(new CategoryItem(name));
            }

            return items;
        }
    }
}