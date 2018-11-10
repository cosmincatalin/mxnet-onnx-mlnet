using System;
using Microsoft.ML;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Transforms;

namespace inference
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var env = new MLContext();
            var reader = TextLoader.CreateReader(env,
                ctx => (
                    RateCode: ctx.LoadFloat(1),
                    PassengerCount: ctx.LoadFloat(2),
                    TripTime: ctx.LoadFloat(3),
                    TripDistance: ctx.LoadFloat(4),
                    Target: ctx.LoadFloat(6)),
                separator: ',',
                hasHeader: true);
            var data = reader.Read(new MultiFileSource(args[1]));

            var learningPipeline = reader.MakeNewEstimator()
                .Append(row => (Target: row.Target, Features: row.RateCode.ConcatWith(row.PassengerCount, row.TripTime, row.TripDistance)))
                .Append(row => (Truth: row.Target, Estimate: row.Features.ApplyOnnxModel(args[0])));

            var model = learningPipeline.Fit(data);
            
            var predictionFunction = model.AsDynamic.MakePredictionFunction<SearchData, SearchPrediction>(env);

            var prediction = predictionFunction.Predict(new SearchData()
            {
                RateCode = 1.0f,
                PassengerCount = 1.0f,
                TripTime = 1.0f,
                TripDistance = 1.0f
            });            
            Console.WriteLine($"Prediction output is {prediction.Estimate}");
        }
    }
}
