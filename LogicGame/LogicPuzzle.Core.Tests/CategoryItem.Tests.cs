#region © 2026 Joe Arrowood (JoeWare)
//
// All rights reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical, or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
#endregion

using LogicPuzzle.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CategoryItem.Tests
{
    // ----------------------------------------------------
    /// <summary>
    ///     Summary description for ArrowUnitTestXML1
    /// </summary>

    [TestClass]
    public class CategoryItemTests
    {
        public CategoryItemTests() { }

        // ------------------------------------------------

        [TestMethod]
        [DataRow("MyTest", "MyTest")]
        public void ToString_CategoryItem(string input, string expected)
        {
            // -------
            // Arrange

            var iut = new LogicPuzzle.Core.CategoryItem(input);

            // ---
            // Act

            var res = iut.ToString();

            // ------
            // Assert

            Assert.AreEqual(expected, res);
        }
    }
}