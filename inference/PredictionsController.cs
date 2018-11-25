using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.Runtime.Data;

namespace inference.Controllers
{
    [Route("")]
    [ApiController]
    public class PredictionsController : ControllerBase
    {
        private PredictionFunction<SearchData, FlatPrediction> _predictionFn;

        public PredictionsController(PredictionFunction<SearchData, FlatPrediction> predictionFn)
        {
            _predictionFn = predictionFn;
        }

        [HttpPost]
        public float Post([FromBody] SearchData instance) {
            return _predictionFn.Predict(instance).Estimate;
        }
    }
}
