FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build-env
WORKDIR /app/src

# copy csproj and restore as distinct layers
COPY ./src/tracker.csproj .
RUN dotnet restore

# copy everything else and build
COPY ./src/ .
RUN dotnet publish -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim
WORKDIR /app
# install node.js
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN apt-get install -y nodejs
COPY --from=build-env /app/src/out ./
COPY --from=build-env /app/src/NodeScripts ./NodeScripts
RUN mkdir Data
WORKDIR /app/NodeScripts
RUN ls -la
RUN npm install
WORKDIR /app
ENTRYPOINT ["dotnet", "./tracker.dll"]