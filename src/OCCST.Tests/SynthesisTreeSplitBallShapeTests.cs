using System.Linq;
using Accord.MachineLearning.DecisionTrees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCCST.Algorithm;
using OCCST.Algorithm.Models;
using OCCST.Models;

namespace OCCST.Tests
{
    [TestClass]
    public class SynthesisTreeSplitBallShapeTests
    {
        private DecisionVariable[] attributes;
        private double[][] learn;
        private double[][] validation;
        private int[] validationOutput;

        [TestInitialize]
        public void Initialize()
        {
            GlobalVariables.Dimensions = 2;

            attributes = new DecisionVariable[]
            {
                new DecisionVariable("X1", DecisionVariableKind.Continuous),
                new DecisionVariable("X2", DecisionVariableKind.Continuous),
            };

            validation = new[]
            {
                new double[] { 1, 1 }, new double[] { 0, 2 }, new double[] { 0, 3 }, new double[] { 0, 4 }, new double[] { 0, 5 }, new double[] { 0, 6 }, new double[] { 0, 7 }, new double[] { 0, 8 }, new double[] { 0, 9 },
                new double[] { 1, 1 }, new double[] { 1, 2 }, new double[] { 1, 3 }, new double[] { 1, 4 }, new double[] { 1, 5 }, new double[] { 1, 6 }, new double[] { 1, 7 }, new double[] { 1, 8 }, new double[] { 1, 9 },
                new double[] { 2, 1 }, new double[] { 2, 2 }, new double[] { 2, 3 }, new double[] { 2, 4 }, new double[] { 2, 5 }, new double[] { 2, 6 }, new double[] { 2, 7 }, new double[] { 2, 8 }, new double[] { 2, 9 },
                new double[] { 3, 1 }, new double[] { 3, 2 }, new double[] { 3, 3 }, new double[] { 3, 4 }, new double[] { 3, 5 }, new double[] { 3, 6 }, new double[] { 3, 7 }, new double[] { 3, 8 }, new double[] { 3, 9 },
                new double[] { 4, 1 }, new double[] { 4, 2 }, new double[] { 4, 3 }, new double[] { 4, 4 }, new double[] { 4, 5 }, new double[] { 4, 6 }, new double[] { 4, 7 }, new double[] { 4, 8 }, new double[] { 4, 9 },
                new double[] { 5, 1 }, new double[] { 5, 2 }, new double[] { 5, 3 }, new double[] { 5, 4 }, new double[] { 5, 5 }, new double[] { 5, 6 }, new double[] { 5, 7 }, new double[] { 5, 8 }, new double[] { 5, 9 },
                new double[] { 6, 1 }, new double[] { 6, 2 }, new double[] { 6, 3 }, new double[] { 6, 4 }, new double[] { 6, 5 }, new double[] { 6, 6 }, new double[] { 6, 7 }, new double[] { 6, 8 }, new double[] { 6, 9 },
                new double[] { 7, 1 }, new double[] { 7, 2 }, new double[] { 7, 3 }, new double[] { 7, 4 }, new double[] { 7, 5 }, new double[] { 7, 6 }, new double[] { 7, 7 }, new double[] { 7, 8 }, new double[] { 7, 9 },
                new double[] { 7, 1 }, new double[] { 8, 2 }, new double[] { 8, 3 }, new double[] { 8, 4 }, new double[] { 8, 5 }, new double[] { 8, 6 }, new double[] { 8, 7 }, new double[] { 8, 8 }, new double[] { 8, 9 }
            };

            validationOutput = new int[]
            {
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 0, 1, 1, 1, 1,
                1, 1, 0, 0, 0, 0, 0, 1, 1,
                1, 1, 0, 0, 0, 0, 0, 1, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 1, 0, 0, 0, 0, 0, 1, 1,
                1, 1, 0, 0, 0, 0, 0, 1, 1,
                1, 1, 1, 1, 0, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
            };

            learn = new[]
            {
                                                                                                            new double[] { 1, 5 },
                                                              new double[] { 2, 3 }, new double[] { 2, 4 }, new double[] { 2, 5 }, new double[] { 2, 6 }, new double[] { 2, 7 },
                                                              new double[] { 3, 3 }, new double[] { 3, 4 }, new double[] { 3, 5 }, new double[] { 3, 6 }, new double[] { 3, 7 },
                                       new double[] { 4, 2 }, new double[] { 4, 3 }, new double[] { 4, 4 }, new double[] { 4, 5 }, new double[] { 4, 6 }, new double[] { 4, 7 }, new double[] { 4, 8 },
                                                              new double[] { 5, 3 }, new double[] { 5, 4 }, new double[] { 5, 5 }, new double[] { 5, 6 }, new double[] { 5, 7 },
                                                              new double[] { 6, 3 }, new double[] { 6, 4 }, new double[] { 6, 5 }, new double[] { 6, 6 }, new double[] { 6, 7 },
                                                                                                            new double[] { 7, 5 },
            };
        }

        [TestMethod]
        public void DecisionTreeLearn_BoundingBox()
        {
            GlobalVariables.GrowCondition = new GrowCondition(0);

            var synthesisTree = new SynthesisTreeLearn(attributes, learn, validation);
            synthesisTree.Learn();

            var tree = synthesisTree.Tree;

            Assert.AreEqual(1, tree.Root.Branches.Count);
            Assert.AreEqual(1, tree.Root.Branches[0].Branches.Count);
            Assert.AreEqual(1, tree.Root.Branches[0].Branches[0].Branches.Count);
            Assert.AreEqual(1, tree.Root.Branches[0].Branches[0].Branches[0].Branches.Count);
            Assert.AreEqual(learn.Length, tree.Root.Branches[0].Branches[0].Branches[0].Branches[0].Output);
            Assert.IsTrue(tree.Root.Branches[0].Branches[0].Branches[0].Branches[0].IsLeaf);

            int[] actual = tree.Decide(validation)
                               .Select(val => val > 0 ? 0 : 1)
                               .ToArray();

            var areaSize = (double)tree.Decide(validation)
                                       .Count(val => val > 0);

            var confusionMatrix = new Accord.Statistics.Analysis.ConfusionMatrix(actual, validationOutput, 0, 1);

            Assert.AreEqual(29, confusionMatrix.TruePositives);
            Assert.AreEqual(32, confusionMatrix.TrueNegatives);
            Assert.AreEqual(0, confusionMatrix.FalseNegatives);
            Assert.AreEqual(20, confusionMatrix.FalsePositives);
            Assert.AreEqual(49, areaSize);
        }

        [TestMethod]
        public void DecisionTreeLearn_OneSplit()
        {
            GlobalVariables.GrowCondition = new GrowCondition(5);

            var synthesisTree = new SynthesisTreeLearn(attributes, learn, validation);
            synthesisTree.Learn();

            var tree = synthesisTree.Tree;

            Assert.AreEqual(1, tree.Root.Branches.Count);
            Assert.AreEqual(1, tree.Root.Branches[0].Branches.Count);
            Assert.AreEqual(1, tree.Root.Branches[0].Branches[0].Branches.Count);
            Assert.AreEqual(1, tree.Root.Branches[0].Branches[0].Branches[0].Branches.Count);
            Assert.AreEqual(28, tree.Root.Branches[0].Branches[0].Branches[0].Branches[0].Branches[0].Output);
            Assert.AreEqual(1, tree.Root.Branches[0].Branches[0].Branches[0].Branches[0].Branches[1].Output);

            int[] actual = tree.Decide(validation)
                               .Select(val => val > 0 ? 0 : 1)
                               .ToArray();

            var areaSize = (double)tree.Decide(validation)
                                       .Count(val => val > 0);

            var confusionMatrix = new Accord.Statistics.Analysis.ConfusionMatrix(actual, validationOutput, 0, 1);

            Assert.AreEqual(29, confusionMatrix.TruePositives);
            Assert.AreEqual(32, confusionMatrix.TrueNegatives);
            Assert.AreEqual(0, confusionMatrix.FalseNegatives);
            Assert.AreEqual(20, confusionMatrix.FalsePositives);
            Assert.AreEqual(49, areaSize);
        }

        [TestMethod]
        public void DecisionTreeLearn_TwoSplit()
        {
            GlobalVariables.GrowCondition = new GrowCondition(6);

            var synthesisTree = new SynthesisTreeLearn(attributes, learn, validation);
            synthesisTree.Learn();

            var tree = synthesisTree.Tree;

            Assert.AreEqual(1, tree.Root.Branches.Count);
            Assert.AreEqual(1, tree.Root.Branches[0].Branches.Count);
            Assert.AreEqual(1, tree.Root.Branches[0].Branches[0].Branches.Count);
            Assert.AreEqual(1, tree.Root.Branches[0].Branches[0].Branches[0].Branches.Count);
            Assert.AreEqual(1, tree.Root.Branches[0].Branches[0].Branches[0].Branches[0].Branches[0].Branches[0].Output);
            Assert.AreEqual(27, tree.Root.Branches[0].Branches[0].Branches[0].Branches[0].Branches[0].Branches[1].Output);
            Assert.AreEqual(1, tree.Root.Branches[0].Branches[0].Branches[0].Branches[0].Branches[1].Output);

            int[] actual = tree.Decide(validation)
                               .Select(val => val > 0 ? 0 : 1)
                               .ToArray();

            var areaSize = (double)tree.Decide(validation)
                                       .Count(val => val > 0);

            var confusionMatrix = new Accord.Statistics.Analysis.ConfusionMatrix(actual, validationOutput, 0, 1);

            Assert.AreEqual(29, confusionMatrix.TruePositives);
            Assert.AreEqual(32, confusionMatrix.TrueNegatives);
            Assert.AreEqual(0, confusionMatrix.FalseNegatives);
            Assert.AreEqual(20, confusionMatrix.FalsePositives);
            Assert.AreEqual(49, areaSize);
        }
    }
}