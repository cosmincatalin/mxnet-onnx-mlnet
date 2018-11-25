FROM microsoft/dotnet:sdk AS build-env
COPY inference/*.csproj /src/
COPY inference/*.cs /src/
WORKDIR /src
RUN dotnet restore --verbosity normal
RUN dotnet publish -c Release -o /app -r win10-x64

FROM microsoft/dotnet:aspnetcore-runtime
COPY --from=build-env /app /app
COPY models/model.onnx /models
COPY lib/* /app/
WORKDIR /app
CMD ["dotnet", "inference.dll", "--onnx", "/models/model.onnx"]