using System;
using Microsoft.ML;

namespace inference
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var env = new MLContext();
            var predictor = new Predictor(env, args[0], args[1]);
            var predictionFn = predictor.GetPredictor();

            var prediction = predictionFn.Predict(new SearchData { RateCode = 1.0f, PassengerCount = 1.0f, TripTime = 1.0f, TripDistance = 1.0f });            
            Console.WriteLine($"Test prediction on ones is {prediction.Estimate}");
        }
    }
}
