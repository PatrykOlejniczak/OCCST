using Accord.MachineLearning.DecisionTrees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCCST.Algorithm.Calculators;

namespace OCCST.Tests
{
    [TestClass]
    public class ObjectFromTargetClassTests
    {
        [TestMethod]
        public void Constructor_CalculateTruePositivesAndFalseNegative()
        {
            var inputs = new double[] { 1.0, 2.0, 3.0, 4.0 };

            var oftc = new ObjectFromTargetClass(inputs, ComparisonKind.GreaterThanOrEqual, 3.0);

            Assert.AreEqual(2, oftc.FalseNegatives);
            Assert.AreEqual(2, oftc.TruePositives);
        }
    }
}