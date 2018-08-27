using Accord.MachineLearning.DecisionTrees;
using OCCST.Algorithm.Calculators;
using OCCST.Extensions;

namespace OCCST.Algorithm.Models
{
    public class SplitInformation
    {
        public ObjectFromTargetClass ConfusionMatrix { get; }
        public double RecallValue { get; }
        public double Probability { get; }
        public double MeasureValue { get; }

        public SplitInformation(double[] inputs, double[] validateInputs, int validationInputsTotalCount, ComparisonKind comparisonKind, double splitValue)
        {
            var validationPositives = validateInputs.Verify(comparisonKind, splitValue);

            ConfusionMatrix = new ObjectFromTargetClass(inputs, comparisonKind, splitValue);
            if (ConfusionMatrix.TruePositives == 0 || validationPositives == 0)
            {
                RecallValue = 0;
                Probability = 0;
                MeasureValue = 0;
                return;
            }

            RecallValue
                = StatisticsCalculator.CalculateRecall(ConfusionMatrix);
            Probability
                = (double)validationPositives / validationInputsTotalCount;
            MeasureValue
                = StatisticsCalculator.CalculateObjectiveFunction(RecallValue, Probability);
        }
    }
}