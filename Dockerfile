FROM microsoft/dotnet:2.1.500-sdk-nanoserver-1803
COPY inference/*.csproj /
RUN dotnet restore --force --no-cache --verbosity normal
COPY inference/*.cs /
COPY data/test.csv /
COPY models/model.onnx /
RUN dotnet publish -c Release --force -o out -r win10-x64
WORKDIR /out
CMD ["dotnet", "inference.dll", "/model.onnx", "/test.csv"]