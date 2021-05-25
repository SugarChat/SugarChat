FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["src/SugarChat.SignalR.Server/SugarChat.SignalR.Server.csproj", "src/SugarChat.SignalR.Server/"]
COPY ["src/SugarChat.Push.SignalR/SugarChat.Push.SignalR.csproj", "src/SugarChat.Push.SignalR/"]
COPY ["src/SugarChat.Core/SugarChat.Core.csproj", "src/SugarChat.Core/"]
COPY ["src/SugarChat.Message/SugarChat.Message.csproj", "src/SugarChat.Message/"]
COPY ["src/SugarChat.Shared/SugarChat.Shared.csproj", "src/SugarChat.Shared/"]
RUN dotnet restore "src/SugarChat.SignalR.Server/SugarChat.SignalR.Server.csproj"
COPY . .
WORKDIR "/src/SugarChat.SignalR.Server"
RUN dotnet build "SugarChat.SignalR.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SugarChat.SignalR.Server.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SugarChat.SignalR.Server.dll"]