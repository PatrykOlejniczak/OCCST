using Accord.MachineLearning.DecisionTrees;

namespace OCCST.Algorithm.Models
{
    internal class SuggestSplitPoint
    {
        internal int AttributeIndex { get; }
        internal double SplitValue { get; }
        internal ComparisonKind ComparisonKind { get; }

        internal SplitInformation Left { get; }
        internal SplitInformation Right { get; }

        internal SuggestSplitPoint(int attributeIndex, ComparisonKind comparisonKind, double splitValue, SplitInformation left, SplitInformation right = null)
        {
            AttributeIndex = attributeIndex;
            ComparisonKind = comparisonKind;
            SplitValue = splitValue;

            Left = left;
            Right = right;
        }

        internal bool IsBetterThanParent(DecisionNode parent)
        {
            var betterBranch = Left.MeasureValue + Right.MeasureValue;

            return parent is SplitDecisionNode node
                        &&
                   betterBranch - node.SplitInformation.MeasureValue > 0.1;
        }

        internal bool IsBetterThan(SuggestSplitPoint other)
        {
            var betterBranch = Left.MeasureValue + Right.MeasureValue;
            var betterOtherBranch = other.Left.MeasureValue + other.Right.MeasureValue;

            return betterBranch - betterOtherBranch > 0.1;
        }
    }
}