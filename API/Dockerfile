FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS http://+:5000

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY API/API.csproj API/
COPY Infrastructure.Implementation.MongoDB/Infrastructure.Implementation.MongoDB.csproj Infrastructure.Implementation.MongoDB/
COPY CoreDomain/CoreDomain.csproj CoreDomain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
RUN dotnet restore API/API.csproj
COPY . .
WORKDIR /src/API
RUN dotnet build API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "API.dll"]
