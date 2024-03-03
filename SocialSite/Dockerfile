FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SocialSite/SocialSite.csproj", "SocialSite/"]
COPY ["SocialModel/SocialModel.csproj", "SocialModel/"]
RUN dotnet restore "SocialSite/SocialSite.csproj"
COPY . .
WORKDIR "/src/SocialSite"
RUN dotnet build "SocialSite.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SocialSite.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocialSite.dll"]
