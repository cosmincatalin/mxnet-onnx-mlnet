### Usage

 * Build image `docker build -t mxnet-onnx-mlnet --no-cache .`
 * Run container based on the built image `docker run -p 5000:80 mxnet-onnx-mlnet`
 * Test with a cURL call `curl -H "Content-Type: application/json" -d "{'RateCode':1.0,"PassengerCount":1.0,"TripTime":1.0,"TripDistance":1.0}" http://localhost:5000`