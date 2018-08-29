using System;
using System.Collections.Generic;
using System.Data;
using Accord.IO;
using Accord.Math;
using System.IO;
using System.Linq;
using Accord.MachineLearning.DecisionTrees;

namespace OCCST.Data
{
    public class DataStorage
    {
        public double[][] LearnDataSet { get; }
        public double[][] ValidationDataSet { get; }
        public double[][] TestInputDataSet { get; }
        public int[] TestOutputDataSet { get; private set; }
        public DecisionVariable[] DecisionVariables { get; private set; }

        public DataStorage(string learnDataSetPath,
            string validationDataSetPath, string testDataSetPath)
        {
            try
            {
                LearnDataSet = LoadDataSet(learnDataSetPath, loadOnlyPositives: true, loadDecisionVariables: true);
                ValidationDataSet = LoadDataSet(validationDataSetPath);
                TestInputDataSet = LoadDataSet(testDataSetPath, loadOutput: true);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong when loading files.");
            }
        }

        private double[][] LoadDataSet(string path, bool loadOnlyPositives = false, bool loadDecisionVariables = false,
            bool loadOutput = false)
        {
            double[][] dataSet = null;

            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                var excelReader = new ExcelReader(stream, true);
                var sheets = excelReader.GetWorksheetList();

                var tableSource = excelReader.GetWorksheet(sheets.Single());
                if (loadOnlyPositives)
                {
                    var result = tableSource.Select("Y = 1");
                    foreach (DataRow row in result)
                    {
                        tableSource.Rows.Remove(row);
                    }
                }
                var matrix = tableSource.ToMatrix(out var columnNames);

                int[] arr = Enumerable.Range(0, columnNames.Length - 1).ToArray();
                dataSet = matrix.GetColumns(arr).ToJagged();

                if (loadDecisionVariables)
                {
                    var tempList = new List<DecisionVariable>();
                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        if (i != columnNames.Length - 1)
                        {
                            tempList.Add(new DecisionVariable("X" + i, DecisionVariableKind.Continuous));
                        }
                        else
                        {
                            tempList.Add(new DecisionVariable("Y", DecisionVariableKind.Continuous));
                        }
                    }

                    DecisionVariables = tempList.ToArray();
                }

                if (loadOutput)
                {
                    var testCount = matrix.GetLength(1);
                    TestOutputDataSet = matrix.GetColumn(testCount - 1).ToInt32();
                }
            }

            return dataSet;
        }
    }
}