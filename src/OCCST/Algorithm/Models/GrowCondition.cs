namespace OCCST.Algorithm.Models
{
    public class GrowCondition
    {
        public int? MaxTreeHeight { get; }
        public int? MaxAttributeUsage { get; }
        public int? MinParentSizePercent { get; }

        public GrowCondition(int? maxTreeHeight = null, int? maxAttributeUsage = null, int? minParentSizePercent = null)
        {
            this.MaxTreeHeight = maxTreeHeight;
            this.MaxAttributeUsage = maxAttributeUsage;
            this.MinParentSizePercent = minParentSizePercent;
        }
    }
}