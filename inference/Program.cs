using System;
using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
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

            Console.WriteLine($"Reading test data from {args[1]} for no apparent reason.");
            Console.WriteLine($"Reading ONNX model from {args[0]}.");

            var pipeline = new ColumnConcatenatingEstimator(env, "Features", "RateCode", "PassengerCount", "TripTime", "TripDistance")
                .Append(new ColumnSelectingEstimator(env, "Features", "Target"))
                .Append(new OnnxScoringEstimator(env, args[0], "Features", "Estimate"))
                .Append(new ColumnSelectingEstimator(env, "Target", "Estimate"))
                .Append(new CustomMappingEstimator<RawPrediction, FlatPrediction>(env, contractName: "OnnxPredictionExtractor",
                    mapAction: (input, output) =>
                    {
                        output.Target = input.Target;
                        output.Estimate = input.Estimate[0];
                    }));


            var transformer = pipeline.Fit(data.AsDynamic);
            var predictionFn = transformer.MakePredictionFunction<SearchData, FlatPrediction>(env);

            var prediction = predictionFn.Predict(new SearchData()
            {
                RateCode = 1.0f,
                PassengerCount = 1.0f,
                TripTime = 1.0f,
                TripDistance = 1.0f
            });            
            Console.WriteLine($"Prediction output is {prediction.Estimate}");

            var testPredictions = transformer.Transform(data.AsDynamic); 

            Console.WriteLine($"RMSE : {env.Regression.Evaluate(testPredictions, "Target", "Estimate").Rms}");
        }

        private class SearchData
        {
            [ColumnName("Target")]
            public float DummyUnused{ get; set; }
        
            public float RateCode{ get; set; }

            public float PassengerCount{ get; set; }

            public float TripTime{ get; set; }

            public float TripDistance{ get; set; }
        }

        #pragma warning disable 649
        private  class RawPrediction
        {
            public float Target;
            public float[] Estimate;
        }
        #pragma warning restore 649

        private class FlatPrediction
        {
            public float Target;
            public float Estimate;
        }
    }
}
