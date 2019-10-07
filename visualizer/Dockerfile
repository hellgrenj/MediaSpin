FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build-env
WORKDIR /app
# Copy everything and build
COPY ./src/ ./
RUN dotnet restore "./visualizer.Server/visualizer.Server.csproj"
RUN dotnet publish "./visualizer.Server/visualizer.Server.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim 
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "visualizer.Server.dll"]