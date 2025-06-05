FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY McCoy/McCoy.csproj McCoy/
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore McCoy/McCoy.csproj

COPY . .
RUN dotnet publish McCoy/McCoy.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "McCoy.dll"]
