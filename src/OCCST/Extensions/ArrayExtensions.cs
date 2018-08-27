using Accord.MachineLearning.DecisionTrees;
using System.Collections.Generic;
using System.Linq;

namespace OCCST.Extensions
{
    public static class ArrayExtensions
    {
        public static int GetVeryfiedCount(this double[] inputs, ComparisonKind comparisonKind, double comparisonConstant)
        {
            return inputs.Count(input => comparisonKind.Check(input, comparisonConstant));
        }

        public static double[] GetVeryfied(this double[] array, ComparisonKind comparisonKind, double comparisonConstant)
        {
            var corrects = new List<double>();

            corrects.AddRange(array.Where(element => comparisonKind.Check(element, comparisonConstant)));

            return corrects.ToArray();
        }
    }
}