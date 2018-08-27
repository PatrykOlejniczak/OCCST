using Accord.MachineLearning.DecisionTrees;
using Accord.Math;
using OCCST.Algorithm.Calculators;
using OCCST.Algorithm.Models;
using OCCST.Extensions;
using System.Collections.Generic;
using System.Linq;
using OCCST.Models;

namespace OCCST.Algorithm
{
    public class SynthesisTree
    {
        public DecisionTree DecisionTree { get; private set; }

        private IList<DecisionVariable> attributes;
        private int validationInputsTotalCount;

        public SynthesisTree(DecisionVariable[] attributes)
        {
            this.attributes = attributes;
        }

        public DecisionTree Learn(double[][] learnInputs, int[] learnOutputs, double[][] validationInputs)
        {
            validationInputsTotalCount = validationInputs.Length;

            if (DecisionTree == null)
            {
                DecisionTree = new DecisionTree(attributes, learnOutputs.Max() + 1);
                attributes = DecisionTree.Attributes;
            }

            Run(learnInputs, validationInputs);

            return DecisionTree;
        }

        private void Run(double[][] inputs, double[][] validationInputs)
        {
            var root = DecisionTree.Root = new DecisionNode(DecisionTree);
            var thresholds = ThresholdsCalculator.Calculate(inputs);

            for (int attributeIndex = 0; attributeIndex < GlobalVariables.Dimensions; attributeIndex++)
            {
                var min = thresholds[attributeIndex].Min();

                thresholds[attributeIndex]
                    = thresholds[attributeIndex].RemoveAll(min);

                var minSplitInformation = new SplitInformation(inputs.GetColumn(attributeIndex),
                                                               validationInputs.GetColumn(attributeIndex),
                                                               validationInputsTotalCount,
                                                               ComparisonKind.GreaterThan,
                                                               min);

                var minSplit = new SuggestSplitPoint(attributeIndex,
                                                     ComparisonKind.GreaterThan,
                                                     min,
                                                     minSplitInformation,
                                                     null);

                var minNode = new SplitDecisionNode(DecisionTree, root, minSplit.Left,
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
                                                               validationInputsTotalCount,
                                                               ComparisonKind.LessThan,
                                                               max);

                var maxSplit = new SuggestSplitPoint(attributeIndex,
                                                     ComparisonKind.LessThan,
                                                     max,
                                                     maxSplitInformation);

                var maxNode = new SplitDecisionNode(DecisionTree, root, maxSplit.Left,
                                                    maxSplit.ComparisonKind, maxSplit.SplitValue, attributeIndex);

                inputs = inputs.CutRowByColumnValue(attributeIndex, ComparisonKind.LessThan, max);
                validationInputs = validationInputs.CutRowByColumnValue(attributeIndex, ComparisonKind.LessThan, max);

                root.Branches.AttributeIndex = attributeIndex;
                root.Branches.AddRange(maxNode);
                root = maxNode;
            }

            int[] attributeUsage = null;
            if (GlobalVariables.GrowCondition.MaxAttributeUsage.HasValue)
            {
                attributeUsage = Enumerable.Repeat(GlobalVariables.GrowCondition.MaxAttributeUsage.Value, attributes.Count - 1).ToArray();
            }

            Split(root, inputs, validationInputs, thresholds, root.GetHeight(), attributeUsage);
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
                                                   validationInputsTotalCount,
                                                   comparisonKind,
                                                   thresholds[thresholdAttribute][thresholdIndex]);

                    var splitInformationRight
                            = new SplitInformation(inputs.GetColumn(thresholdAttribute),
                                validationInputs.GetColumn(thresholdAttribute),
                                validationInputsTotalCount,
                                comparisonKind.GetOpposed(),
                                thresholds[thresholdAttribute][thresholdIndex]);

                    var localBestSplitPoint = new SuggestSplitPoint(thresholdAttribute,
                                                                    comparisonKind,
                                                                    thresholds[thresholdAttribute][thresholdIndex],
                                                                    splitInformationLeft,
                                                                    splitInformationRight);

                    if (GlobalVariables.GrowCondition.MinParentSizePercent.HasValue)
                    {
                        var parentSize = splitInformationLeft.ConfusionMatrix.TruePositives
                                            + splitInformationRight.ConfusionMatrix.TruePositives;

                        if (parentSize == 0)
                            continue;

                        var minSize = (double)GlobalVariables.GrowCondition.MinParentSizePercent * parentSize / 100;
                        if (splitInformationLeft.ConfusionMatrix.TruePositives > minSize
                                && splitInformationRight.ConfusionMatrix.TruePositives > minSize)
                        {
                            if (suggestSplitPoint == null
                                || localBestSplitPoint.IsBetterThan(suggestSplitPoint))
                            {
                                suggestSplitPoint = localBestSplitPoint;
                            }
                        }
                    }
                    else
                    {
                        var parentSize = splitInformationLeft.ConfusionMatrix.TruePositives
                                         + splitInformationRight.ConfusionMatrix.TruePositives;

                        if (parentSize == 0 || parentSize == 1)
                            continue;

                        if (suggestSplitPoint == null
                            || localBestSplitPoint.IsBetterThan(suggestSplitPoint))
                        {
                            suggestSplitPoint = localBestSplitPoint;
                        }
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
                                new SplitDecisionNode(DecisionTree, root, suggestSplitPoint.Left, suggestSplitPoint.ComparisonKind, suggestSplitPoint.SplitValue, suggestSplitPoint.AttributeIndex),
                                new SplitDecisionNode(DecisionTree, root, suggestSplitPoint.Right, suggestSplitPoint.ComparisonKind.GetOpposed(), suggestSplitPoint.SplitValue, suggestSplitPoint.AttributeIndex)
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
                    = tempTresholds[suggestSplitPoint.AttributeIndex].Remove(child.Comparison, suggestSplitPoint.SplitValue);

                Split(child, fulfillingInputs, fulfillingValidation, tempTresholds, height + 1, attributeUsage.Copy());
            }
        }
    }
}