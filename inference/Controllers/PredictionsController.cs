using System;
using System.Collections.Generic;
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
        
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var prediction = _predictionFn.Predict(new SearchData
                {RateCode = 1.0f, PassengerCount = 1.0f, TripTime = 1.0f, TripDistance = 1.0f});
            Console.WriteLine($"Test prediction on ones is {prediction.Estimate}");
            return new string[] { "value1", "value2" };
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
    }
}
