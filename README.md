# MXNet to ONNX to ML.NET

This is an example project to show how an Apache MXNet MLP model can be exported to the ONNX format and subsequently be used in ML.NET. The challenge was running inference in ML.NET because at the time of writing, the ONNX Transformer was only supported on Windows [ref](https://blogs.msdn.microsoft.com/dotnet/2018/10/08/announcing-ml-net-0-6-machine-learning-net/).

# Requirements

For the _Modeling_ part you need Docker on Linux conatiners.

For the _Inference_ part you need Docker on Windows containers.

If you do not have a Windows box, you can just run the modeling part.

## Modeling

The data for this example comes from the [New York taxi fare dataset](https://www.kaggle.com/c/new-york-city-taxi-fare-prediction/data).

The model that will be created will solve a regression problem. Note that I have not focused on creating a low error model, but focused more on the process.

The model artefacts are already in the repository and running the modelling step is not strictly required.

![MLP](model.png)

##### Usage

* Build image `docker build -t mxnet-onnx-mlnet-modeling -f Dockerfile.modeling .`
* Run container based on the built image:
 * \*nix: `docker run -p 8888:8888 -v $(pwd)/data:/notebook/data/:ro -v $(pwd)/models:/notebook/models mxnet-onnx-mlnet-modeling`
 * Windows: `docker run -p 8888:8888 -v $(pwd)/data:/notebook/data/:ro -v $(pwd)/models:/notebook/models mxnet-onnx-mlnet-modeling`

## Inference

Use the generated _ONNX_ model for running an inference web service.

##### Usage

 * Build image `docker build -t mxnet-onnx-mlnet-inference -f Dockerfile.inference .`
 * Run container based on the built image `docker run -p 5000:80 mxnet-onnx-mlnet-inference`
 * Test with a cURL call `curl -H "Content-Type: application/json" -d "{'RateCode':1.0,"PassengerCount":1.0,"TripTime":1.0,"TripDistance":1.0}" http://localhost:5000`