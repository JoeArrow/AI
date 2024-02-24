using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Practical.AI.PropositionalLogic;

namespace Practical.AI.UnitTests
{
    [TestClass]
    public class DPLL_UnitTests
    {
        // ------------------------------------------------
        // p v q ^ p v 'q ^ 'p v q ^ 'p v 'r

        [TestMethod]
        [DataRow(true)]
        public void Formula1_DPLL(bool expected)
        {
            // -------
            // Arrange

            var p = new Variable(true) { Name = "p" };
            var q = new Variable(true) { Name = "q" };
            var r = new Variable(true) { Name = "r" };

            var f1 = new And(new Or(p, q), new Or(p, new Not(q)));
            var f2 = new And(new Or(new Not(p), q), new Or(new Not(p), new Not(r)));

            var formula = new And(f1, f2);
            var nnf = formula.ToNnf();

            // ---
            // Act

            nnf = nnf.ToCnf();
            var cnf = new Cnf(nnf as And);
            cnf.SimplifyCnf();
            var dpll = cnf.Dpll();

            // ---
            // Log

            Console.WriteLine($"NNF:{nnf}");
            Console.WriteLine($"CNF:{cnf}");
            Console.WriteLine($"SAT:{dpll}");

            // ------
            // Assert

            Assert.AreEqual(expected, dpll);
        }
    }
}
