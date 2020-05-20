FROM mcr.microsoft.com/dotnet/core/sdk:3.1.100-buster AS build-env
WORKDIR /app/src

# copy csproj and restore as distinct layers
COPY ./src/storage.csproj .
RUN dotnet restore

# copy everything else and build
COPY ./src/ .
RUN dotnet publish -c Release -o out

#build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/src/out ./
COPY --from=build-env /app/src/Persistence/Migrations ./
ENTRYPOINT ["dotnet", "./storage.dll"]