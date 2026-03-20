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
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'},{'Name':'Carol'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'},{'Name':'Fish'}]}",
                 "['Alice','Cat','Fish','Dog']")]

        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'},{'Name':'Carol'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'},{'Name':'Fish'}]}",
                 "['Bob','Dog','Fish','Cat']")]

        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'},{'Name':'Carol'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'},{'Name':'Fish'}]}",
                 "['Carol','Cat','Dog','Fish']")]
        public void SinglePossibility_TriggersYes(string cat1Json, string cat2Json, string valuesJson)
        {
            // -------
            // Arrange

            var cat1 = JsonSerializer.Deserialize<Category>(cat1Json.Replace("\'", "\""));
            var cat2 = JsonSerializer.Deserialize<Category>(cat2Json.Replace("\'", "\""));
            var values = JsonSerializer.Deserialize<string[]>(valuesJson.Replace("\'", "\""));

            // --------------------
            // Arrange based Assert

            Assert.IsNotNull(cat1, "cat1 deserialization failed.");
            Assert.IsNotNull(cat2, "cat2 deserialization failed.");
            Assert.IsNotNull(values, "values deserialization failed.");
            Assert.AreEqual(4, values.Length, "values must contain exactly 4 elements.");

            var categories = new List<Category> { cat1, cat2 };

            var item1Name = values[0];
            var noItem1Name = values[1];
            var noItem2Name = values[2];
            var expectedItemName = values[3];

            WorkingGrid grid = new WorkingGrid(categories);

            // ---
            // Act

            grid.SetNo(cat1.Name, item1Name, cat2.Name, noItem1Name);
            grid.SetNo(cat1.Name, item1Name, cat2.Name, noItem2Name);

            // ------
            // Assert

            Assert.AreEqual(CellState.No, grid.GetState(cat1.Name, item1Name, cat2.Name, noItem1Name));
            Assert.AreEqual(CellState.No, grid.GetState(cat1.Name, item1Name, cat2.Name, noItem2Name));
            Assert.AreEqual(CellState.Yes, grid.GetState(cat1.Name, item1Name, cat2.Name, expectedItemName));
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'},{'Name':'Carol'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'},{'Name':'Fish'}]}",
                 "['Alice','Dog']",
                 "['Cat','Fish']",
                 "['Bob','Carol']")]

        [DataRow("{'Name':'People','Items':[{'Name':'Alpha'},{'Name':'Beta'},{'Name':'Gamma'}]}",
                 "{'Name':'Colors','Items':[{'Name':'Red'},{'Name':'Blue'},{'Name':'Green'}]}",
                 "['Beta','Green']",
                 "['Red','Blue']",
                 "['Alpha','Gamma']")]
        public void SetYes_WorkingGrid_EliminatesOtherPossibilities(string cat1Json,
                                                                    string cat2Json,
                                                                    string yesPairJson,
                                                                    string sameRowNoItemsJson,
                                                                    string sameColumnNoItemsJson)
        {
            // -------
            // Arrange

            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);
            var yesPair = DeserializeStringArray(yesPairJson);
            var sameRowNoItems = DeserializeStringArray(sameRowNoItemsJson);
            var sameColumnNoItems = DeserializeStringArray(sameColumnNoItemsJson);

            // --------------------
            // Arrange based Assert

            Assert.IsNotNull(cat1, "cat1 deserialization failed.");
            Assert.IsNotNull(cat2, "cat2 deserialization failed.");
            Assert.IsNotNull(yesPair, "yesPair deserialization failed.");
            Assert.IsNotNull(sameRowNoItems, "sameRowNoItems deserialization failed.");
            Assert.IsNotNull(sameColumnNoItems, "sameColumnNoItems deserialization failed.");

            Assert.AreEqual(2, yesPair.Length, "yesPair must contain exactly 2 elements.");

            var categories = new List<Category> { cat1, cat2 };

            var item1Name = yesPair[0];
            var item2Name = yesPair[1];

            var grid = new WorkingGrid(categories);

            // ---
            // Act

            grid.SetYes(cat1.Name, item1Name, cat2.Name, item2Name);

            // ------
            // Assert

            Assert.AreEqual(CellState.Yes, grid.GetState(cat1.Name, item1Name, cat2.Name, item2Name));

            foreach(var otherItem2Name in sameRowNoItems)
            {
                Assert.AreEqual(CellState.No,
                                grid.GetState(cat1.Name, item1Name, cat2.Name, otherItem2Name));
            }

            foreach(var otherItem1Name in sameColumnNoItems)
            {
                Assert.AreEqual(CellState.No,
                                grid.GetState(cat1.Name, otherItem1Name, cat2.Name, item2Name));
            }
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 "['Alice','Cat']",
                 "['Alice','Dog']")]

        [DataRow("{'Name':'Letters','Items':[{'Name':'A'},{'Name':'B'}]}",
                 "{'Name':'Numbers','Items':[{'Name':'One'},{'Name':'Two'}]}",
                 "['B','Two']",
                 "['A','Two']")]
        public void GetState_WorkingGrid_ReturnsSameValueInEitherCategoryOrder(string cat1Json,
                                                                                string cat2Json,
                                                                                string yesPairJson,
                                                                                string noPairJson)
        {
            // -------
            // Arrange

            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);
            var yesPair = DeserializeStringArray(yesPairJson);
            var noPair = DeserializeStringArray(noPairJson);

            // --------------------
            // Arrange based Assert

            Assert.IsNotNull(cat1, "cat1 deserialization failed.");
            Assert.IsNotNull(cat2, "cat2 deserialization failed.");
            Assert.IsNotNull(yesPair, "yesPair deserialization failed.");
            Assert.IsNotNull(noPair, "noPair deserialization failed.");

            Assert.AreEqual(2, yesPair.Length, "yesPair must contain exactly 2 elements.");
            Assert.AreEqual(2, noPair.Length, "noPair must contain exactly 2 elements.");

            var categories = new List<Category> { cat1, cat2 };

            var yesItem1Name = yesPair[0];
            var yesItem2Name = yesPair[1];
            var noItem1Name = noPair[0];
            var noItem2Name = noPair[1];

            var grid = new WorkingGrid(categories);

            // ---
            // Act

            grid.SetYes(cat1.Name, yesItem1Name, cat2.Name, yesItem2Name);

            // ------
            // Assert

            Assert.AreEqual(CellState.Yes,
                            grid.GetState(cat1.Name, yesItem1Name, cat2.Name, yesItem2Name));

            Assert.AreEqual(CellState.Yes,
                            grid.GetState(cat2.Name, yesItem2Name, cat1.Name, yesItem1Name));

            Assert.AreEqual(CellState.No,
                            grid.GetState(cat1.Name, noItem1Name, cat2.Name, noItem2Name));

            Assert.AreEqual(CellState.No,
                            grid.GetState(cat2.Name, noItem2Name, cat1.Name, noItem1Name));
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
         "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
         "['Alice','Cat']")]

        [DataRow("{'Name':'Letters','Items':[{'Name':'A'},{'Name':'B'}]}",
         "{'Name':'Numbers','Items':[{'Name':'One'},{'Name':'Two'}]}",
         "['B','Two']")]
        public void SetNo_WorkingGrid_ThrowsOnContradictoryAssignment(string cat1Json,
                                                              string cat2Json,
                                                              string pairJson)
        {
            // -------
            // Arrange

            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);
            var pair = DeserializeStringArray(pairJson);

            // --------------------
            // Arrange based Assert

            Assert.IsNotNull(cat1, "cat1 deserialization failed.");
            Assert.IsNotNull(cat2, "cat2 deserialization failed.");
            Assert.IsNotNull(pair, "pair deserialization failed.");
            Assert.AreEqual(2, pair.Length, "pair must contain exactly 2 elements.");

            var categories = new List<Category> { cat1, cat2 };

            var item1Name = pair[0];
            var item2Name = pair[1];

            var grid = new WorkingGrid(categories);

            grid.SetYes(cat1.Name, item1Name, cat2.Name, item2Name);

            // ----------
            // Act/Assert

            Assert.ThrowsExactly<InvalidOperationException>(() => grid.SetNo(cat1.Name, item1Name, cat2.Name, item2Name));
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 "{'Name':'Colors','Items':[{'Name':'Red'},{'Name':'Blue'}]}",
                 "['Alice','Cat']",
                 "['Cat','Red']",
                 "['Alice','Red']")]

        [DataRow("{'Name':'Workers','Items':[{'Name':'Ann'},{'Name':'Ben'}]}",
                 "{'Name':'Cars','Items':[{'Name':'Ford'},{'Name':'Tesla'}]}",
                 "{'Name':'Cities','Items':[{'Name':'Rome'},{'Name':'Paris'}]}",
                 "['Ben','Tesla']",
                 "['Tesla','Paris']",
                 "['Ben','Paris']")]
        public void CrossCategoryPropagation_WorkingGrid_TransfersYesAcrossBridge(string cat1Json,
                                                                                  string cat2Json,
                                                                                  string cat3Json,
                                                                                  string pair1Json,
                                                                                  string pair2Json,
                                                                                  string expectedPairJson)
        {
            // -------
            // Arrange

            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);
            var cat3 = DeserializeCategory(cat3Json);
            var pair1 = DeserializeStringArray(pair1Json);
            var pair2 = DeserializeStringArray(pair2Json);
            var expectedPair = DeserializeStringArray(expectedPairJson);

            // --------------------
            // Arrange based Assert

            Assert.IsNotNull(cat1, "cat1 deserialization failed.");
            Assert.IsNotNull(cat2, "cat2 deserialization failed.");
            Assert.IsNotNull(cat3, "cat3 deserialization failed.");
            Assert.IsNotNull(pair1, "pair1 deserialization failed.");
            Assert.IsNotNull(pair2, "pair2 deserialization failed.");
            Assert.IsNotNull(expectedPair, "expectedPair deserialization failed.");

            Assert.AreEqual(2, pair1.Length, "pair1 must contain exactly 2 elements.");
            Assert.AreEqual(2, pair2.Length, "pair2 must contain exactly 2 elements.");
            Assert.AreEqual(2, expectedPair.Length, "expectedPair must contain exactly 2 elements.");

            var categories = new List<Category> { cat1, cat2, cat3 };

            var pair1Item1Name = pair1[0];
            var pair1Item2Name = pair1[1];

            var pair2Item1Name = pair2[0];
            var pair2Item2Name = pair2[1];

            var expectedItem1Name = expectedPair[0];
            var expectedItem2Name = expectedPair[1];

            var grid = new WorkingGrid(categories);

            // ---
            // Act

            grid.SetYes(cat1.Name, pair1Item1Name, cat2.Name, pair1Item2Name);
            grid.SetYes(cat2.Name, pair2Item1Name, cat3.Name, pair2Item2Name);

            // ------
            // Assert

            Assert.AreEqual(CellState.Yes,
                            grid.GetState(cat1.Name, expectedItem1Name, cat3.Name, expectedItem2Name));
        }

        // ------------------------------------------------

        private Category DeserializeCategory(string json)
        {
            return JsonSerializer.Deserialize<Category>(json.Replace("\'", "\""));
        }

        // ------------------------------------------------

        private string[] DeserializeStringArray(string json)
        {
            return JsonSerializer.Deserialize<string[]>(json.Replace("\'", "\""));
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 "{'Name':'Colors','Items':[{'Name':'Red'},{'Name':'Blue'}]}",
                 "['Alice','Cat']",
                 "['Cat','Blue']",
                 "['Alice','Blue']"
)]
        public void CrossCategoryNegativePropagation_WorkingGrid_InfersNo(string cat1Json,
                                                                          string cat2Json,
                                                                          string cat3Json,
                                                                          string pair1Json,
                                                                          string pair2Json,
                                                                          string expectedNoPairJson)
        {
            // -------
            // Arrange

            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);
            var cat3 = DeserializeCategory(cat3Json);

            var pair1 = DeserializeStringArray(pair1Json);
            var pair2 = DeserializeStringArray(pair2Json);
            var expectedPair = DeserializeStringArray(expectedNoPairJson);

            // --------------------
            // Arrange based Assert

            Assert.IsNotNull(cat1);
            Assert.IsNotNull(cat2);
            Assert.IsNotNull(cat3);
            Assert.IsNotNull(pair1);
            Assert.IsNotNull(pair2);
            Assert.IsNotNull(expectedPair);

            var categories = new List<Category> { cat1, cat2, cat3 };

            var grid = new WorkingGrid(categories);

            // ---
            // Act

            grid.SetYes(cat1.Name, pair1[0], cat2.Name, pair1[1]);   // A = B
            grid.SetNo(cat2.Name, pair2[0], cat3.Name, pair2[1]);    // B != C

            // ------
            // Assert

            Assert.AreEqual(CellState.No, grid.GetState(cat1.Name, expectedPair[0], cat3.Name, expectedPair[1]));
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow(
    "{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'},{'Name':'Carol'}]}",
    "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'},{'Name':'Fish'}]}",
    "{'Name':'Colors','Items':[{'Name':'Red'},{'Name':'Blue'},{'Name':'Green'}]}",
    "['Alice','Cat']",
    "['Cat','Blue']",
    "['Alice','Red']",
    "['Alice','Green']",
    "['Alice','Blue']"
)]
        public void PropagationChain_WorkingGrid_ReachesExpectedStableState(string cat1Json,
                                                                    string cat2Json,
                                                                    string cat3Json,
                                                                    string yesPair1Json,
                                                                    string yesPair2Json,
                                                                    string noPair1Json,
                                                                    string noPair2Json,
                                                                    string expectedYesPairJson)
        {
            // -------
            // Arrange

            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);
            var cat3 = DeserializeCategory(cat3Json);

            var yesPair1 = DeserializeStringArray(yesPair1Json);
            var yesPair2 = DeserializeStringArray(yesPair2Json);
            var noPair1 = DeserializeStringArray(noPair1Json);
            var noPair2 = DeserializeStringArray(noPair2Json);
            var expectedYesPair = DeserializeStringArray(expectedYesPairJson);

            // --------------------
            // Arrange based Assert

            Assert.IsNotNull(cat1);
            Assert.IsNotNull(cat2);
            Assert.IsNotNull(cat3);
            Assert.IsNotNull(yesPair1);
            Assert.IsNotNull(yesPair2);
            Assert.IsNotNull(noPair1);
            Assert.IsNotNull(noPair2);
            Assert.IsNotNull(expectedYesPair);

            var categories = new List<Category> { cat1, cat2, cat3 };

            var grid = new WorkingGrid(categories);

            // ---
            // Act

            grid.SetYes(cat1.Name, yesPair1[0], cat2.Name, yesPair1[1]);
            grid.SetYes(cat2.Name, yesPair2[0], cat3.Name, yesPair2[1]);
            grid.SetNo(cat1.Name, noPair1[0], cat3.Name, noPair1[1]);
            grid.SetNo(cat1.Name, noPair2[0], cat3.Name, noPair2[1]);

            // ------
            // Assert

            Assert.AreEqual(CellState.Yes,
                            grid.GetState(cat1.Name, expectedYesPair[0], cat3.Name, expectedYesPair[1]));
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 "['Alice','Cat']",
                 "['Alice','Dog']")]
        public void Exhaustion_WorkingGrid_ThrowsWhenNoCandidatesRemain(string cat1Json, string cat2Json,
                                                                        string noPair1Json, string noPair2Json)
        {
            // -------
            // Arrange

            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);

            var noPair1 = DeserializeStringArray(noPair1Json);
            var noPair2 = DeserializeStringArray(noPair2Json);

            var categories = new List<Category> { cat1, cat2 };
            var grid = new WorkingGrid(categories);

            // ----------
            // Act/Assert

            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                grid.SetNo(cat1.Name, noPair1[0], cat2.Name, noPair1[1]);
                grid.SetNo(cat1.Name, noPair2[0], cat2.Name, noPair2[1]);
            });
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow(2, 2, -1, 0)]
        [DataRow(2, 2, 2, 0)]
        [DataRow(2, 2, 0, -1)]
        [DataRow(2, 2, 0, 2)]
        public void GetCell_GridMatrix_ThrowsOnInvalidCoordinates(int rowCount, int columnCount, int row, int column)
        {
            // -------
            // Arrange

            var matrix = new GridMatrix(rowCount, columnCount);

            // -------------
            // Act / Assert

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => matrix.GetCell(row, column));
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow(2, 2, -1, 0, CellState.Unknown)]
        [DataRow(2, 2, 2, 0, CellState.Unknown)]
        [DataRow(2, 2, 0, -1, CellState.Unknown)]
        [DataRow(2, 2, 0, 2, CellState.Unknown)]
        public void SetCell_GridMatrix_ThrowsOnInvalidCoordinates(int rowCount, int columnCount, int row, int column, CellState state)
        {
            // -------
            // Arrange

            var matrix = new GridMatrix(rowCount, columnCount);

            // -------------
            // Act / Assert

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => matrix.SetCell(row, column, state));
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 "GhostCategory",
                 "Alice",
                 "Pets",
                 "Cat"
             )]
        public void GetState_WorkingGrid_ThrowsOnUnknownFirstCategory(string cat1Json,
                                                                      string cat2Json,
                                                                      string badCategoryName,
                                                                      string item1Name,
                                                                      string category2Name,
                                                                      string item2Name)
        {
            // -------
            // Arrange

            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);

            Assert.IsNotNull(cat1);
            Assert.IsNotNull(cat2);

            var categories = new List<Category> { cat1, cat2 };
            var grid = new WorkingGrid(categories);

            // -------------
            // Act / Assert

            Assert.ThrowsExactly<ArgumentException>( () => grid.GetState(badCategoryName, item1Name, category2Name, item2Name));
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 "People",
                 "Alice",
                 "GhostCategory",
                 "Cat"
             )]
        public void GetState_WorkingGrid_ThrowsOnUnknownSecondCategory(string cat1Json,
                                                               string cat2Json,
                                                               string category1Name,
                                                               string item1Name,
                                                               string badCategoryName,
                                                               string item2Name)
        {
            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);

            Assert.IsNotNull(cat1);
            Assert.IsNotNull(cat2);

            var categories = new List<Category> { cat1, cat2 };
            var grid = new WorkingGrid(categories);

            Assert.ThrowsExactly<ArgumentException>(() => grid.GetState(category1Name, item1Name, badCategoryName, item2Name));
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 "People",
                 "GhostItem",
                 "Pets",
                 "Cat"
             )]
        public void GetState_WorkingGrid_ThrowsOnUnknownFirstItem(string cat1Json,
                                                          string cat2Json,
                                                          string category1Name,
                                                          string badItemName,
                                                          string category2Name,
                                                          string item2Name)
        {
            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);

            Assert.IsNotNull(cat1);
            Assert.IsNotNull(cat2);

            var categories = new List<Category> { cat1, cat2 };
            var grid = new WorkingGrid(categories);

            Assert.ThrowsExactly<ArgumentException>(() => grid.GetState(category1Name, badItemName, category2Name, item2Name));
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 "People",
                 "Alice",
                 "Pets",
                 "GhostItem"
             )]
        public void GetState_WorkingGrid_ThrowsOnUnknownSecondItem(string cat1Json,
                                                           string cat2Json,
                                                           string category1Name,
                                                           string item1Name,
                                                           string category2Name,
                                                           string badItemName)
        {
            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);

            Assert.IsNotNull(cat1);
            Assert.IsNotNull(cat2);

            var categories = new List<Category> { cat1, cat2 };
            var grid = new WorkingGrid(categories);

            Assert.ThrowsExactly<ArgumentException>(() => grid.GetState(category1Name, item1Name, category2Name, badItemName));
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}", "People", "Alice", "Bob" )]
        public void GetState_WorkingGrid_ThrowsWhenNoMatrixExistsForSameCategory(string catJson,
                                                                         string categoryName,
                                                                         string item1Name,
                                                                         string item2Name)
        {
            // -------
            // Arrange

            var cat = DeserializeCategory(catJson);

            Assert.IsNotNull(cat);

            var categories = new List<Category> { cat };
            var grid = new WorkingGrid(categories);

            // -------------
            // Act / Assert

            Assert.ThrowsExactly<InvalidOperationException>( () => grid.GetState(categoryName, item1Name, categoryName, item2Name));
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 "['Alice','Cat']",
                 "['Alice','Dog']"
             )]
        public void SetNo_WorkingGrid_ThrowsWhenRowHasNoPossibleMatch(string cat1Json,
                                                              string cat2Json,
                                                              string noPair1Json,
                                                              string noPair2Json)
        {
            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);
            var noPair1 = DeserializeStringArray(noPair1Json);
            var noPair2 = DeserializeStringArray(noPair2Json);

            Assert.IsNotNull(cat1);
            Assert.IsNotNull(cat2);
            Assert.IsNotNull(noPair1);
            Assert.IsNotNull(noPair2);

            var categories = new List<Category> { cat1, cat2 };
            var grid = new WorkingGrid(categories);

            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                grid.SetNo(cat1.Name, noPair1[0], cat2.Name, noPair1[1]);
                grid.SetNo(cat1.Name, noPair2[0], cat2.Name, noPair2[1]);
            });
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 "['Alice','Cat']",
                 "['Bob','Cat']")]
        public void SetNo_WorkingGrid_ThrowsWhenColumnHasNoPossibleMatch(string cat1Json,
                                                                 string cat2Json,
                                                                 string noPair1Json,
                                                                 string noPair2Json)
        {
            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);
            var noPair1 = DeserializeStringArray(noPair1Json);
            var noPair2 = DeserializeStringArray(noPair2Json);

            Assert.IsNotNull(cat1);
            Assert.IsNotNull(cat2);
            Assert.IsNotNull(noPair1);
            Assert.IsNotNull(noPair2);

            var categories = new List<Category> { cat1, cat2 };
            var grid = new WorkingGrid(categories);

            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                grid.SetNo(cat1.Name, noPair1[0], cat2.Name, noPair1[1]);
                grid.SetNo(cat1.Name, noPair2[0], cat2.Name, noPair2[1]);
            });
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}", 
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 0, 0, 0, 1)]
        public void ValidateMatrixConsistency_WorkingGrid_ThrowsOnRowContradiction(string cat1Json, string cat2Json, int row1, 
                                                                                   int column1, int row2, int column2)
        {
            // -------
            // Arrange

            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);

            Assert.IsNotNull(cat1);
            Assert.IsNotNull(cat2);

            var categories = new List<Category> { cat1, cat2 };
            var grid = new WorkingGrid(categories);

            var matricesField = typeof(WorkingGrid).GetField("_matrices",
                                                             System.Reflection.BindingFlags.NonPublic |
                                                             System.Reflection.BindingFlags.Instance);

            Assert.IsNotNull(matricesField);

            var matrices = matricesField.GetValue(grid) as System.Collections.IDictionary;

            Assert.IsNotNull(matrices);
            Assert.AreEqual(1, matrices.Count);

            GridMatrix matrix = null;

            foreach(System.Collections.DictionaryEntry entry in matrices)
            {
                matrix = entry.Value as GridMatrix;
                break;
            }

            Assert.IsNotNull(matrix);

            matrix.SetCell(row1, column1, CellState.Yes);
            matrix.SetCell(row2, column2, CellState.Yes);

            var validateMethod = typeof(WorkingGrid).GetMethod("ValidateMatrixConsistency",
                                                               System.Reflection.BindingFlags.NonPublic |
                                                               System.Reflection.BindingFlags.Instance);

            Assert.IsNotNull(validateMethod);

            // -------------
            // Act / Assert

            var exception = Assert.ThrowsExactly<System.Reflection.TargetInvocationException>(
                () => validateMethod.Invoke(grid, null));

            Assert.IsNotNull(exception.InnerException);
            Assert.IsInstanceOfType<InvalidOperationException>(exception.InnerException);
            Assert.AreEqual("Row contradiction in matrix 'People' x 'Pets'.",
                            exception.InnerException.Message);
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}", 
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 0, 0, 1, 0)]
        public void ValidateMatrixConsistency_WorkingGrid_ThrowsOnColumnContradiction(string cat1Json, string cat2Json, int row1, 
                                                                                      int column1, int row2, int column2)
        {
            // -------
            // Arrange

            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);

            Assert.IsNotNull(cat1);
            Assert.IsNotNull(cat2);

            var categories = new List<Category> { cat1, cat2 };
            var grid = new WorkingGrid(categories);

            var matricesField = typeof(WorkingGrid).GetField("_matrices",
                                                             System.Reflection.BindingFlags.NonPublic |
                                                             System.Reflection.BindingFlags.Instance);

            Assert.IsNotNull(matricesField);

            var matrices = matricesField.GetValue(grid) as System.Collections.IDictionary;

            Assert.IsNotNull(matrices);
            Assert.AreEqual(1, matrices.Count);

            GridMatrix matrix = null;

            foreach(System.Collections.DictionaryEntry entry in matrices)
            {
                matrix = entry.Value as GridMatrix;
                break;
            }

            Assert.IsNotNull(matrix);

            matrix.SetCell(row1, column1, CellState.Yes);
            matrix.SetCell(row2, column2, CellState.Yes);

            var validateMethod = typeof(WorkingGrid).GetMethod("ValidateMatrixConsistency",
                                                               System.Reflection.BindingFlags.NonPublic |
                                                               System.Reflection.BindingFlags.Instance);

            Assert.IsNotNull(validateMethod);

            // -------------
            // Act / Assert

            var exception = Assert.ThrowsExactly<System.Reflection.TargetInvocationException>(
                () => validateMethod.Invoke(grid, null));

            Assert.IsNotNull(exception.InnerException);
            Assert.IsInstanceOfType<InvalidOperationException>(exception.InnerException);
            Assert.AreEqual("Column contradiction in matrix 'People' x 'Pets'.", exception.InnerException.Message);
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 0, 0, CellState.Yes,
                 0, 1, CellState.Yes,
                 "Row contradiction in matrix 'People' x 'Pets'.")]

        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 0, 0, CellState.Yes,
                 1, 0, CellState.Yes,
                 "Column contradiction in matrix 'People' x 'Pets'."
             )]

        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 0, 0, CellState.No,
                 0, 1, CellState.No,
                 "Row has no possible match in matrix 'People' x 'Pets'."
             )]

        [DataRow("{'Name':'People','Items':[{'Name':'Alice'},{'Name':'Bob'}]}",
                 "{'Name':'Pets','Items':[{'Name':'Cat'},{'Name':'Dog'}]}",
                 0, 0, CellState.No,
                 1, 0, CellState.No,
                 "Column has no possible match in matrix 'People' x 'Pets'."
             )]
        public void ValidateMatrixConsistency_WorkingGrid_CatchesInvalidStates(string cat1Json,
                                                                               string cat2Json,
                                                                               int row1,
                                                                               int col1,
                                                                               CellState state1,
                                                                               int row2,
                                                                               int col2,
                                                                               CellState state2,
                                                                               string expectedMessage)
        {
            // -------
            // Arrange

            var cat1 = DeserializeCategory(cat1Json);
            var cat2 = DeserializeCategory(cat2Json);

            Assert.IsNotNull(cat1);
            Assert.IsNotNull(cat2);

            var categories = new List<Category> { cat1, cat2 };
            var grid = new WorkingGrid(categories);

            var matrix = GetOnlyMatrix(grid);

            matrix.SetCell(row1, col1, state1);
            matrix.SetCell(row2, col2, state2);

            // -------------
            // Act

            var ex = InvokeValidateMatrixConsistency(grid);

            // ------
            // Assert

            Assert.AreEqual(expectedMessage, ex.Message);
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("People", "Pets", "People", "Pets", true)]
        [DataRow("People", "Pets", "People", "Colors", false)]
        [DataRow("People", "Pets", "Colors", "Pets", false)]
        [DataRow("People", "Pets", "Colors", "Numbers", false)]
        public void Equals_CategoryPairKey_ReturnsExpectedResultForCategoryPairKey(string firstCategoryName1,
                                                                                   string secondCategoryName1,
                                                                                   string firstCategoryName2,
                                                                                   string secondCategoryName2,
                                                                                   bool expected)
        {
            // -------
            // Arrange

            var key1 = new CategoryPairKey(firstCategoryName1, secondCategoryName1);
            var key2 = new CategoryPairKey(firstCategoryName2, secondCategoryName2);

            // ---
            // Act

            var result = key1.Equals(key2);

            // ------
            // Assert

            Assert.AreEqual(expected, result);
        }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("People", "Pets")]
        public void Equals_CategoryPairKey_ReturnsFalseForNull(string firstCategoryName,
                                                       string secondCategoryName)
        {
            // -------
            // Arrange

            var key = new CategoryPairKey(firstCategoryName, secondCategoryName);

            // ---
            // Act

            var result = key.Equals(null);

            // ------
            // Assert

            Assert.IsFalse(result);
        }

        // ------------------------------------------------

        private GridMatrix GetOnlyMatrix(WorkingGrid grid)
        {
            var matricesField = typeof(WorkingGrid).GetField("_matrices",
                                                             System.Reflection.BindingFlags.NonPublic |
                                                             System.Reflection.BindingFlags.Instance);

            Assert.IsNotNull(matricesField);

            var matrices = matricesField.GetValue(grid) as System.Collections.IDictionary;

            Assert.IsNotNull(matrices);
            Assert.AreEqual(1, matrices.Count);

            foreach(System.Collections.DictionaryEntry entry in matrices)
            {
                var matrix = entry.Value as GridMatrix;

                if(matrix != null)
                {
                    return matrix;
                }
            }

            Assert.Fail("Expected exactly one GridMatrix.");
            return null;
        }

        // ------------------------------------------------

        private Exception InvokeValidateMatrixConsistency(WorkingGrid grid)
        {
            var validateMethod = typeof(WorkingGrid).GetMethod("ValidateMatrixConsistency",
                                                               System.Reflection.BindingFlags.NonPublic |
                                                               System.Reflection.BindingFlags.Instance);

            Assert.IsNotNull(validateMethod);

            var exception = Assert.ThrowsExactly<System.Reflection.TargetInvocationException>(
                () => validateMethod.Invoke(grid, null));

            Assert.IsNotNull(exception.InnerException);

            return exception.InnerException;
        }
    }
}