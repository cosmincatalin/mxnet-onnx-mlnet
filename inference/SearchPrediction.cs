using Microsoft.ML.Runtime.Api;

namespace inference
{
    public class SearchPrediction
    {
        [ColumnName("Target")]
        public float Estimate{ get; set; }
    }
}