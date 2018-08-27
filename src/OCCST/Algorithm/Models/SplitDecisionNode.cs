using Accord.MachineLearning.DecisionTrees;

namespace OCCST.Algorithm.Models
{
    internal class SplitDecisionNode : DecisionNode
    {
        internal SplitInformation SplitInformation { get; }
        internal int AttributeIndex { get; }

        internal SplitDecisionNode(DecisionTree owner, DecisionNode parent,
                                   SplitInformation splitInformation,
                                   ComparisonKind comparisonKind, double splitValue, int attributeIndex)
            : base(owner)
        {
            Parent = parent;

            Value = splitValue;
            Comparison = comparisonKind;
            SplitInformation = splitInformation;
            AttributeIndex = attributeIndex;
        }
    }
}