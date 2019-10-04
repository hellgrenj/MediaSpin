FROM mcr.microsoft.com/dotnet/core-nightly/sdk:3.0 AS build-env
WORKDIR /app
# Copy everything and build
COPY ./src/ ./
RUN dotnet restore "./Server/visualizer2.Server.csproj"
RUN dotnet publish "./Server/visualizer2.Server.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/core-nightly/aspnet:3.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "visualizer2.Server.dll"]
