using ExperimentDatabase;
using System.Reflection;

namespace OCCST.Extensions
{
    public static class ExperimentExtensions
    {
        public static void Save(this Experiment experiment, object obj, string pre = null, string post = null)
        {
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                experiment[pre + propertyInfo.Name + post] = propertyInfo.GetValue(obj);
            }
        }

        public static void SaveIn(this Experiment experiment, object obj)
        {
            experiment.Save(obj, "in_");
        }

        public static void SaveOut(this Experiment experiment, object obj)
        {
            experiment.Save(obj, "out_");
        }
    }
}