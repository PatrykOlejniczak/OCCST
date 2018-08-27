using Accord.MachineLearning.DecisionTrees;
using System;

namespace OCCST.Extensions
{
    public static class ComparisonKindExtensions
    {
        public static ComparisonKind GetOpposed(this ComparisonKind comparisonKind)
        {
            switch (comparisonKind)
            {
                case ComparisonKind.None:
                    return ComparisonKind.None;
                case ComparisonKind.Equal:
                    return ComparisonKind.NotEqual;
                case ComparisonKind.NotEqual:
                    return ComparisonKind.Equal;
                case ComparisonKind.GreaterThanOrEqual:
                    return ComparisonKind.LessThan;
                case ComparisonKind.GreaterThan:
                    return ComparisonKind.LessThanOrEqual;
                case ComparisonKind.LessThan:
                    return ComparisonKind.GreaterThanOrEqual;
                case ComparisonKind.LessThanOrEqual:
                    return ComparisonKind.GreaterThan;
                default:
                    throw new ArgumentException();
            }
        }

        public static bool Check(this ComparisonKind comparisonKind, double left, double right)
        {
            switch (comparisonKind)
            {
                case ComparisonKind.Equal:
                    return Math.Abs(left - right) < Double.Epsilon;
                case ComparisonKind.NotEqual:
                    return Math.Abs(left - right) > Double.Epsilon;
                case ComparisonKind.GreaterThanOrEqual:
                    return left >= right;
                case ComparisonKind.GreaterThan:
                    return left > right;
                case ComparisonKind.LessThan:
                    return left < right;
                case ComparisonKind.LessThanOrEqual:
                    return left <= right;
                default:
                    throw new ArgumentException();
            }
        }
    }
}