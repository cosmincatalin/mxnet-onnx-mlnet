FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY inference/*.csproj ./
RUN dotnet restore --verbosity normal

COPY inference/*.cs ./
RUN dotnet publish -c Release -o out -r win10-x64

FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
COPY data/test.csv ./
COPY models/model.onnx ./
COPY lib/* ./
CMD ["dotnet", "inference.dll", "model.onnx", "test.csv"]