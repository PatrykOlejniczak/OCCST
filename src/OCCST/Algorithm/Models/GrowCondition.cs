namespace OCCST.Algorithm.Models
{
    public class GrowCondition
    {
        public int? MaxTreeHeight { get; }
        public int? MaxAttributeUsage { get; }
        public int? MinLeafSize { get; }

        public GrowCondition(int? maxTreeHeight = null, int? maxAttributeUsage = null, int? minLeafSize = null)
        {
            this.MaxTreeHeight = maxTreeHeight;
            this.MaxAttributeUsage = maxAttributeUsage;
            this.MinLeafSize = minLeafSize;
        }
    }
}