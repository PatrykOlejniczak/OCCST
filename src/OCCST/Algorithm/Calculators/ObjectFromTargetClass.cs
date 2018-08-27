using Accord.MachineLearning.DecisionTrees;
using OCCST.Extensions;

namespace OCCST.Algorithm.Calculators
{
    public class ObjectFromTargetClass
    {
        public int TruePositives { get; }
        public int FalseNegatives { get; }

        public ObjectFromTargetClass(int truePositives, int falseNegatives)
        {
            TruePositives = truePositives;
            FalseNegatives = falseNegatives;
        }

        public ObjectFromTargetClass(double[] inputs, ComparisonKind comparisonKind, double comparisonConstant)
        {
            TruePositives = inputs.GetVeryfiedCount(comparisonKind, comparisonConstant);
            FalseNegatives = inputs.Length - TruePositives;
        }
    }
}
