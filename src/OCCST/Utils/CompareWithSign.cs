using OCCST.Models.GRB;
using System.Collections.Generic;

namespace OCCST.Utils
{
    /// <summary>
    /// Comparison method for two given object of class <see cref="Constraint"/>
    /// Method assumes equality of objects when its value, axis and sign matches
    /// </summary>
    public class CompareWithSign : IEqualityComparer<Constraint>
    {
        public bool Equals(Constraint x, Constraint y)
        {
            return (x.Value == y.Value && x.Axis == y.Axis && x.Sign == y.Sign);
        }

        public int GetHashCode(Constraint obj)
        {
            return $"{obj.Axis}{obj.Value}{obj.Sign}".GetHashCode();
        }
    }
}