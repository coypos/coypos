FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CoyposServer/CoyposServer.csproj", "CoyposServer/"]
RUN dotnet restore "CoyposServer/CoyposServer.csproj"
COPY . .
WORKDIR "/src/CoyposServer"
RUN dotnet build "CoyposServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoyposServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoyposServer.dll"]
