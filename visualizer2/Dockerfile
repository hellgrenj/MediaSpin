FROM mcr.microsoft.com/dotnet/core/sdk:3.1.100-buster AS build-env
WORKDIR /app
# install node.js
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN apt-get install -y nodejs
# Copy everything and build
COPY ./src/ ./
RUN dotnet restore "./visualizer2.csproj"
RUN dotnet publish "./visualizer2.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "visualizer2.dll"]