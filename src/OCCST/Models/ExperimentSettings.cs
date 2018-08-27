using CommandLine;

namespace OCCST.Models
{
    internal class ExperimentSettings
    {
        public ExperimentSettings(string learnFilePath, string validationFilePath, string testFilePath,
                                  int dimensions,
                                  int? maxDecisionTreeHeight, int? maxAttributeUsage, int? minLeafSize,
                                  string name, string description, string version,
                                  string databaseFilePath, bool displaySteps)
        {
            LearnFilePath = learnFilePath;
            ValidationFilePath = validationFilePath;
            TestFilePath = testFilePath;

            Dimensions = dimensions;
            MaxDecisionTreeHeight = maxDecisionTreeHeight;
            MaxAttributeUsage = maxAttributeUsage;
            MinLeafSize = minLeafSize;

            Name = name;
            Description = description;
            Version = version;

            DatabaseFilePath = databaseFilePath;
            DisplaySteps = displaySteps;
        }

        [Option('l', nameof(LearnFilePath), Required = true)]
        public string LearnFilePath { get; }

        [Option('v', nameof(ValidationFilePath), Required = true)]
        public string ValidationFilePath { get; }

        [Option('t', nameof(TestFilePath), Required = true)]
        public string TestFilePath { get; }

        [Option(nameof(Dimensions), Required = true)]
        public int Dimensions { get; }

        [Option(nameof(MaxDecisionTreeHeight), Default = null, Required = false)]
        public int? MaxDecisionTreeHeight { get; }

        [Option(nameof(MaxAttributeUsage), Default = null, Required = false)]
        public int? MaxAttributeUsage { get; }

        [Option(nameof(MinLeafSize), Default = null, Required = false)]
        public int? MinLeafSize { get; }

        [Option(nameof(Name), Default = null, Required = false)]
        public string Name { get; }

        [Option(nameof(Description), Default = null, Required = false)]
        public string Description { get; }

        [Option(nameof(Version), Default = null, Required = false)]
        public string Version { get; }

        [Option(nameof(DatabaseFilePath), Default = null, Required = false)]
        public string DatabaseFilePath { get; }

        [Option(nameof(DisplaySteps), Default = false, Required = false)]
        public bool DisplaySteps { get; }
    }
}
