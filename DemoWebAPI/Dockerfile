FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["DemoWebAPI/DemoWebAPI.csproj", "DemoWebAPI/"]
RUN dotnet restore "DemoWebAPI/DemoWebAPI.csproj"
COPY . .
WORKDIR "/src/DemoWebAPI"
RUN dotnet build "DemoWebAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DemoWebAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DemoWebAPI.dll"]