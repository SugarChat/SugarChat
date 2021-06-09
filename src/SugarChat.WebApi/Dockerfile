FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/SugarChat.WebApi/SugarChat.WebApi.csproj", "SugarChat.WebApi/"]
RUN dotnet restore "src/SugarChat.WebApi/SugarChat.WebApi.csproj"
COPY . .
WORKDIR "/src/SugarChat.WebApi"
RUN dotnet build "SugarChat.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SugarChat.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SugarChat.WebApi.dll"]
