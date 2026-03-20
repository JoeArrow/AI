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
    }
}