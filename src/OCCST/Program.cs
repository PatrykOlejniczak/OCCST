using CommandLine;
using OCCST.Models;
using System.Diagnostics;
using System.Threading;

namespace OCCST
{
    internal class Program
    {
        private static Stopwatch timer = new Stopwatch();

        private static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";

            Parser.Default.ParseArguments<ExperimentSettings>(args)
                  .WithParsed(RunExperiment);
        }

        private static void RunExperiment(ExperimentSettings obj)
        {
            LoadDataSet();

            timer.Start();
            LearnDecisionTree();
            timer.Stop();

            CalculateStatistics();
        }

        private static void LoadDataSet()
        {

        }

        private static void LearnDecisionTree()
        {

        }

        private static void CalculateStatistics()
        {

        }   
    }
}
