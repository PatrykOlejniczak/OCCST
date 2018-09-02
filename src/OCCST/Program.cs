using System;
using System.Collections.Generic;
using Accord.MachineLearning.DecisionTrees;
using CommandLine;
using OCCST.Algorithm;
using OCCST.Algorithm.Models;
using OCCST.Data;
using OCCST.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ExperimentDatabase;
using OCCST.Algorithm.Calculators;
using OCCST.Extensions;
using OCCST.Models.GRB;

namespace OCCST
{
    internal class Program
    {
        private static Stopwatch timer = new Stopwatch();
        private static DataStorage dataStorage;
        private static DecisionTree decisionTree;

        private static void Main(string[] args)
        {
            var culture = (System.Globalization.CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = culture;

            Parser.Default.ParseArguments<ExperimentSettings>(args)
                  .WithParsed(RunExperiment);
        }

        private static void RunExperiment(ExperimentSettings settings)
        {
            try
            {
                GlobalVariables.Dimensions = settings.Dimensions;
                GlobalVariables.GrowCondition = new GrowCondition(
                    settings.MaxDecisionTreeHeight, settings.MaxAttributeUsage, settings.MinLeafSize);

                dataStorage = new DataStorage(
                    settings.LearnFilePath, settings.ValidationFilePath, settings.TestFilePath);

                LearnDecisionTree();

                CalculateStatistics(settings);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
            }
        }

        private static void LearnDecisionTree()
        {
            timer.Start();
            var learnDecisionTree = new SynthesisTreeLearn(
                    dataStorage.DecisionVariables, dataStorage.LearnDataSet, dataStorage.ValidationDataSet);
            learnDecisionTree.Learn();
            decisionTree = learnDecisionTree.Tree;
            timer.Stop();
        }

        private static void CalculateStatistics(ExperimentSettings settings)
        {
            using (var database = new Database(settings.DatabaseFilePath))
            {
                using (var experiment = database.NewExperiment())
                {
                    try
                    {
                        experiment.SaveIn(settings);

                        experiment["out_totalTime"] = timer.ElapsedMilliseconds;
                        experiment["out_decisionTreeHeight"] = decisionTree.GetHeight();
                        experiment["in_terms"] = dataStorage.TestInputDataSet[0].Length;

                        if (decisionTree == null)
                        {
                            throw new ArgumentNullException(nameof(decisionTree));
                        }

                        int[] actual = decisionTree.Decide(dataStorage.TestInputDataSet)
                                                   .Select(val => val > 0 ? 0 : 1)
                                                   .ToArray();

                        var confusionMatrix = new Accord.Statistics.Analysis.ConfusionMatrix(actual, dataStorage.TestOutputDataSet, 0, 1);

                        var validationPropablity =
                                (double)decisionTree.Decide(dataStorage.ValidationDataSet)
                                                    .Count(val => val > 0) / dataStorage.ValidationDataSet.Length;

                        experiment["in_test_examples"] = dataStorage.TestOutputDataSet.Length;
                        experiment["in_test_positives"] = confusionMatrix.ActualPositives;
                        experiment["in_test_negatives"] = confusionMatrix.ActualNegatives;

                        experiment["out_accurancy"] = confusionMatrix.Accuracy;
                        experiment["out_error"] = confusionMatrix.Error;
                        experiment["out_sensitivity"] = confusionMatrix.Sensitivity;
                        experiment["out_specificity"] = confusionMatrix.Specificity;

                        experiment["out_efficiency"] = confusionMatrix.Efficiency;
                        experiment["out_precision"] = confusionMatrix.Precision;
                        experiment["out_recall"] = confusionMatrix.Recall;
                        experiment["out_fScore"] = confusionMatrix.FScore;
                        experiment["out_jaccardIndex"] =
                            (double)confusionMatrix.TruePositives / (confusionMatrix.TruePositives +
                                                                     confusionMatrix.FalseNegatives +
                                                                     confusionMatrix.FalsePositives);

                        experiment["out_falsePositives"] = confusionMatrix.FalsePositives;
                        experiment["out_falseNegatives"] = confusionMatrix.FalseNegatives;
                        experiment["out_trueNegatives"] = confusionMatrix.TrueNegatives;
                        experiment["out_truePositives"] = confusionMatrix.TruePositives;
                        experiment["out_measure"] =
                            StatisticsCalculator.CalculateObjectiveFunction(confusionMatrix.Recall, validationPropablity);
                        experiment["out_matthewsCorrelationCoefficient"] = confusionMatrix.MatthewsCorrelationCoefficient;

                        var rules = decisionTree.ToRules();
                        var rules2 = new List<Rule>();
                        foreach (var rule in rules)
                        {
                            rules2.Add(new Rule(rule.ToString().Replace(",", ".")));
                        }

                        var constraints = rules2.ReturnUniqueSplitsWithBooleans();

                        experiment["out_constraints"] = constraints.Item1.Count;
                    }
                    catch (Exception e)
                    {
                        experiment["error"] = e.Message;
                    }
                    finally
                    {
                        experiment.Save();
                    }
                }
            }
        }
    }
}
