using Microsoft.ML;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Transforms;

namespace inference
{
    public class Predictor
    {
        private readonly MLContext _env;
        private readonly string _onnxFilePath;
        private readonly string _sampleDataPath;

        public Predictor(MLContext env, string onnxFilePath, string sampleDataPath)
        {
            _env = env;
            _onnxFilePath = onnxFilePath;
            _sampleDataPath = sampleDataPath;
        }
        
        public PredictionFunction<SearchData, FlatPrediction> GetPredictor()
        {
            var reader = TextLoader.CreateReader(_env,
                ctx => (
                    RateCode: ctx.LoadFloat(1),
                    PassengerCount: ctx.LoadFloat(2),
                    TripTime: ctx.LoadFloat(3),
                    TripDistance: ctx.LoadFloat(4),
                    Target: ctx.LoadFloat(6)),
                separator: ',',
                hasHeader: true);
            var data = reader.Read(new MultiFileSource(_sampleDataPath));
            
            var pipeline = new ColumnConcatenatingEstimator(_env, "Features", "RateCode", "PassengerCount", "TripTime", "TripDistance")
                .Append(new ColumnSelectingEstimator(_env, "Features", "Target"))
                .Append(new OnnxScoringEstimator(_env, _onnxFilePath, "Features", "Estimate"))
                .Append(new ColumnSelectingEstimator(_env, "Target", "Estimate"))
                .Append(new CustomMappingEstimator<RawPrediction, FlatPrediction>(_env, contractName: "OnnxPredictionExtractor",
                    mapAction: (input, output) =>
                    {
                        output.Target = input.Target;
                        output.Estimate = input.Estimate[0];
                    }));

            var transformer = pipeline.Fit(data.AsDynamic);
            return transformer.MakePredictionFunction<SearchData, FlatPrediction>(_env);
        }
    }
}