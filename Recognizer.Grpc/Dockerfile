#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Recognizer.Grpc/Recognizer.Grpc.csproj", "Recognizer.Grpc/"]
RUN dotnet restore "Recognizer.Grpc/Recognizer.Grpc.csproj"
COPY . .
WORKDIR "/src/Recognizer.Grpc"
RUN dotnet build "Recognizer.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Recognizer.Grpc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Recognizer.Grpc.dll"]