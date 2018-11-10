using Microsoft.ML.Runtime.Api;

namespace inference
{

    public class SearchData
    {
        [ColumnName("Target")]
        public float DummyUnused{ get; set; }
        
        public float RateCode{ get; set; }

        public float PassengerCount{ get; set; }

        public float TripTime{ get; set; }

        public float TripDistance{ get; set; }

    }

}