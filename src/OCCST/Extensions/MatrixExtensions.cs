using Accord.MachineLearning.DecisionTrees;
using Accord.Math;

namespace OCCST.Extensions
{
    public static class MatrixExtensions
    {
        public static double[][] CutRowByColumnValue(this double[][] inputs, int columnIndex, ComparisonKind comparisonKind, double conditionConstance)
        {
            int corrects = inputs.GetColumn(columnIndex).Count(colVal => comparisonKind.Check(colVal, conditionConstance));

            var outputMatrix = new double[corrects][];

            for (int inputRowIndex = 0, copyRowIndex = 0; inputRowIndex < inputs.GetColumn(0).Length; inputRowIndex++)
            {
                if (comparisonKind.Check(inputs[inputRowIndex][columnIndex], conditionConstance))
                {
                    outputMatrix[copyRowIndex] = new double[inputs[inputRowIndex].Length];

                    for (int copyColumnIndex = 0; copyColumnIndex < inputs[inputRowIndex].Length; copyColumnIndex++)
                    {
                        outputMatrix[copyRowIndex][copyColumnIndex] = inputs[inputRowIndex][copyColumnIndex];
                    }

                    copyRowIndex++;
                }
            }

            return outputMatrix;
        }
    }
}