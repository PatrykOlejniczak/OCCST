using OCCST.Algorithm.Models;
using System;
using System.IO;

namespace OCCST.Models
{
    public class GlobalVariables
    {
        public static GrowCondition GrowCondition;

        /// <summary>
        /// Number of problem's variables
        /// </summary>
        public static int Dimensions { get; set; }

        /// <summary>
        /// Folder for output files
        /// </summary>
        public static readonly string ProjectPath =
            Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output"));

        /// <summary>
        /// Path to output Gurobi model
        /// </summary>
        public static readonly string GurobiModelPath = Path.Combine(ProjectPath, "Gurobi_out.lp");

        /// <summary>
        /// Variable used in MILP model as M - big constant
        /// </summary>
        public static readonly int M = (int)Math.Pow(10, 6);
    }
}