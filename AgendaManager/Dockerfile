FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5158

ENV ASPNETCORE_URLS=http://+:5158

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["AgendaManager/AgendaManager.csproj", "AgendaManager/"]
RUN dotnet restore "AgendaManager/AgendaManager.csproj"
COPY . .
WORKDIR "/src/AgendaManager"
RUN dotnet build "AgendaManager.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "AgendaManager.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgendaManager.dll"]
