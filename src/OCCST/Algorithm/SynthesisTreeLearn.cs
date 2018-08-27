using Accord.MachineLearning.DecisionTrees;
using Accord.Math;
using OCCST.Algorithm.Calculators;
using OCCST.Algorithm.Models;
using OCCST.Extensions;
using OCCST.Models;

namespace OCCST.Algorithm
{
    public class SynthesisTreeLearn
    {
        public DecisionTree Tree { get; }

        private double[][] learn;
        private double[][] validation;

        public SynthesisTreeLearn(DecisionVariable[] attributes, double[][] learn, double[][] validation)
        {
            this.learn = learn;
            this.validation = validation;

            if (Tree == null)
            {
                Tree = new DecisionTree(attributes, 2);
            }
        }

        public DecisionTree Learn()
        {
            Run(learn.Copy(), validation.Copy());

            return Tree;
        }

        private void Run(double[][] inputs, double[][] validationInputs)
        {
            var root = Tree.Root = new DecisionNode(Tree);
            var thresholds = ThresholdsCalculator.Calculate(inputs);

            for (int attributeIndex = 0; attributeIndex < GlobalVariables.Dimensions; attributeIndex++)
            {
                var min = thresholds[attributeIndex].Min();

                thresholds[attributeIndex]
                    = thresholds[attributeIndex].RemoveAll(min);

                var minSplitInformation = new SplitInformation(inputs.GetColumn(attributeIndex),
                                                               validationInputs.GetColumn(attributeIndex),
                                                               validation.Length,
                                                               ComparisonKind.GreaterThan,
                                                               min,
                                                               learn.Length);

                var minSplit = new SuggestSplitPoint(attributeIndex,
                                                     ComparisonKind.GreaterThan,
                                                     min,
                                                     minSplitInformation,
                                                     null);

                var minNode = new SplitDecisionNode(Tree, root, minSplit.Left,
                                                    minSplit.ComparisonKind, minSplit.SplitValue, attributeIndex);

                inputs = inputs.CutRowByColumnValue(attributeIndex, ComparisonKind.GreaterThan, min);
                validationInputs = validationInputs.CutRowByColumnValue(attributeIndex, ComparisonKind.GreaterThan, min);

                root.Branches.AttributeIndex = attributeIndex;
                root.Branches.AddRange(minNode);
                root = minNode;

                var max = thresholds[attributeIndex].Max();

                thresholds[attributeIndex]
                    = thresholds[attributeIndex].RemoveAll(max);

                var maxSplitInformation = new SplitInformation(inputs.GetColumn(attributeIndex),
                                                               validationInputs.GetColumn(attributeIndex),
                                                               validation.Length,
                                                               ComparisonKind.LessThan,
                                                               max,
                                                               learn.Length);

                var maxSplit = new SuggestSplitPoint(attributeIndex,
                                                     ComparisonKind.LessThan,
                                                     max,
                                                     maxSplitInformation);

                var maxNode = new SplitDecisionNode(Tree, root, maxSplit.Left,
                                                    maxSplit.ComparisonKind, maxSplit.SplitValue, attributeIndex);

                inputs = inputs.CutRowByColumnValue(attributeIndex, ComparisonKind.LessThan, max);
                validationInputs = validationInputs.CutRowByColumnValue(attributeIndex, ComparisonKind.LessThan, max);

                root.Branches.AttributeIndex = attributeIndex;
                root.Branches.AddRange(maxNode);
                root = maxNode;
            }

            Split(root, inputs, validationInputs, thresholds, root.GetHeight(), null);
        }

        private void Split(DecisionNode root, double[][] inputs, double[][] validationInputs, double[][] thresholds, int height, int[] attributeUsage)
        {
            if (GlobalVariables.GrowCondition.MaxTreeHeight.HasValue && GlobalVariables.GrowCondition.MaxTreeHeight <= height)
            {
                root.Output = inputs.Length;
                return;
            }

            SuggestSplitPoint suggestSplitPoint = null;

            for (int thresholdAttribute = 0; thresholdAttribute < thresholds.Length; thresholdAttribute++)
            {
                for (int thresholdIndex = 0; thresholdIndex < thresholds[thresholdAttribute].Length; thresholdIndex++)
                {
                    var comparisonKind = ComparisonKind.GreaterThanOrEqual;
                    var splitInformationLeft
                            = new SplitInformation(inputs.GetColumn(thresholdAttribute),
                                                   validationInputs.GetColumn(thresholdAttribute),
                                                   validation.Length,
                                                   comparisonKind,
                                                   thresholds[thresholdAttribute][thresholdIndex],
                                                   learn.Length);

                    var splitInformationRight
                            = new SplitInformation(inputs.GetColumn(thresholdAttribute),
                                validationInputs.GetColumn(thresholdAttribute),
                                validation.Length,
                                comparisonKind.GetOpposed(),
                                thresholds[thresholdAttribute][thresholdIndex],
                                learn.Length);

                    var localBestSplitPoint = new SuggestSplitPoint(thresholdAttribute,
                                                                    comparisonKind,
                                                                    thresholds[thresholdAttribute][thresholdIndex],
                                                                    splitInformationLeft,
                                                                    splitInformationRight);


                    var parentSize = splitInformationLeft.ConfusionMatrix.TruePositives
                                     + splitInformationRight.ConfusionMatrix.TruePositives;

                    if (parentSize <= 1)
                        continue;

                    if (suggestSplitPoint == null
                        || localBestSplitPoint.IsBetterThan(suggestSplitPoint))
                    {
                        suggestSplitPoint = localBestSplitPoint;
                    }
                }
            }

            if (suggestSplitPoint == null || !suggestSplitPoint.IsBetterThanParent(root))
            {
                root.Output = inputs.Length;
                return;
            }

            if (GlobalVariables.GrowCondition.MaxAttributeUsage.HasValue)
            {
                attributeUsage[suggestSplitPoint.AttributeIndex] = attributeUsage[suggestSplitPoint.AttributeIndex] - 1;

                if (attributeUsage[suggestSplitPoint.AttributeIndex] == 0)
                {
                    thresholds[suggestSplitPoint.AttributeIndex] = new double[0];
                }
            }

            var children = new[]
                            {
                                new SplitDecisionNode(Tree, root, suggestSplitPoint.Left, suggestSplitPoint.ComparisonKind, suggestSplitPoint.SplitValue, suggestSplitPoint.AttributeIndex),
                                new SplitDecisionNode(Tree, root, suggestSplitPoint.Right, suggestSplitPoint.ComparisonKind.GetOpposed(), suggestSplitPoint.SplitValue, suggestSplitPoint.AttributeIndex)
                            };

            root.Branches.AttributeIndex = suggestSplitPoint.AttributeIndex;
            root.Branches.AddRange(children);

            foreach (var child in children)
            {
                var fulfillingInputs
                        = inputs.CutRowByColumnValue(suggestSplitPoint.AttributeIndex, child.Comparison, child.Value.Value);
                var fulfillingValidation
                        = validationInputs.CutRowByColumnValue(suggestSplitPoint.AttributeIndex, child.Comparison, child.Value.Value);

                var tempTresholds = thresholds.Copy();

                tempTresholds[suggestSplitPoint.AttributeIndex]
                    = tempTresholds[suggestSplitPoint.AttributeIndex].GetVeryfied(child.Comparison, suggestSplitPoint.SplitValue);

                Split(child, fulfillingInputs, fulfillingValidation, tempTresholds, height + 1, null);
            }
        }
    }
}