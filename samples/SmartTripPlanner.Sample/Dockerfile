#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["samples/SmartTripPlanner.Sample/SmartTripPlanner.Sample.csproj", "samples/SmartTripPlanner.Sample/"]
COPY ["src/SmartTripPlanner.ChargePoints/SmartTripPlanner.ChargePoints.csproj", "src/SmartTripPlanner.ChargePoints/"]
COPY ["src/SmartTripPlanner.Core/SmartTripPlanner.Core.csproj", "src/SmartTripPlanner.Core/"]
COPY ["src/SmartTripPlanner.GoogleRoutes/SmartTripPlanner.GoogleRoutes.csproj", "src/SmartTripPlanner.GoogleRoutes/"]
RUN dotnet restore "./samples/SmartTripPlanner.Sample/SmartTripPlanner.Sample.csproj"
COPY . .
WORKDIR "/src/samples/SmartTripPlanner.Sample"
RUN dotnet build "./SmartTripPlanner.Sample.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SmartTripPlanner.Sample.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartTripPlanner.Sample.dll"]