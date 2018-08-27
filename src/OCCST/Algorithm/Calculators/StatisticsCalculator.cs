using System;

namespace OCCST.Algorithm.Calculators
{
    public class StatisticsCalculator
    {
        public static double CalculateRecall(ObjectFromTargetClass confusionMatrix)
        {
            return CalculateRecall(confusionMatrix.TruePositives, confusionMatrix.FalseNegatives);
        }

        public static double CalculateRecall(int truePositives, int falseNegatives)
        {
            return (double)truePositives / (double)(truePositives + falseNegatives);
        }

        public static double CalculateObjectiveFunction(double recall, double probability)
        {
            return (double)Math.Pow(recall, 2) / probability;
        }
    }
}