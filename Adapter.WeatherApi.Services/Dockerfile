FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY . .
RUN dotnet restore "Adapter.WeatherApi.Services/Adapter.WeatherApi.Services.csproj"

COPY . .
WORKDIR "/src/Adapter.WeatherApi.Services"
RUN dotnet build "Adapter.WeatherApi.Services.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Adapter.WeatherApi.Services.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS http://*:10500
EXPOSE 10500

ENTRYPOINT ["dotnet", "Adapter.WeatherApi.Services.dll"]