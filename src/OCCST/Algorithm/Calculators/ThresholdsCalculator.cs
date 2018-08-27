using System;
using System.Collections.Generic;
using Accord.Math;

namespace OCCST.Algorithm.Calculators
{
    public static class ThresholdsCalculator
    {
        public static double[][] Calculate(double[][] inputs)
        {
            var attributes = inputs[0].Length;

            var thresholds = new double[attributes][];
            var candidates = new List<double>(inputs.Length);

            for (int i = 0; i < attributes; i++)
            {
                var sortedValue = inputs.GetColumn(i)
                    .Distinct();
                sortedValue.Sort();

                for (int j = 0; j < sortedValue.Length - 1; j++)
                {
                    double currentValue = sortedValue[j];
                    double nextValue = sortedValue[j + 1];

                    if (Math.Abs(currentValue - nextValue) > Constants.DoubleEpsilon)
                    {
                        candidates.Add((currentValue + nextValue) / 2.0);
                    }
                }

                candidates.Add(
                    candidates[0] - (Math.Abs(candidates[0] - candidates[1])));
                candidates.Sort();
                candidates.Add(
                    candidates[candidates.Count - 1] + (Math.Abs(candidates[candidates.Count - 1] - candidates[candidates.Count - 2])));

                thresholds[i] = candidates.ToArray();
                candidates.Clear();
            }

            return thresholds;
        }
    }
}