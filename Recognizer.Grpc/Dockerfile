#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

WORKDIR /app/

RUN apt-get update && apt-get install -y \
    libdlib-dev \
    libopenblas-dev \
    liblapack-dev \
    libx11-6 \
    git \
    gnupg curl apt-transport-https \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

RUN curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add -

RUN sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-debian-stretch-prod stretch main" > /etc/apt/sources.list.d/microsoft.list'

RUN apt-get update \
    && apt-get install -y \
    powershell \ 
    && apt-get clean && rm -rf /var/lib/apt/lists/*

WORKDIR /app/Dlib/
RUN git clone https://github.com/takuya-takeuchi/DlibDotNet

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Recognizer.Grpc/Recognizer.Grpc.csproj", "Recognizer.Grpc/"]
RUN dotnet restore "Recognizer.Grpc/Recognizer.Grpc.csproj"
COPY . .
WORKDIR "/src/Recognizer.Grpc"
RUN dotnet build "Recognizer.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Recognizer.Grpc.csproj" -c Release -o /app/publish /p:UseAppHost=true

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

WORKDIR /app/Dlib/DlibDotNet/
RUN ./Initialize.sh

WORKDIR /app/Dlib/DlibDotNet/src/DlibDotNet.Native/
CMD pwsh Build.ps1 <Release/Debug> <cpu/cuda/mkl> <32/64> <desktop/uwp> <optional: cuda version [92/100/101]

WORKDIR /app/Dlib/DlibDotNet/src/DlibDotNet.Native.Dnn
CMD pwsh ./Build.ps1 <Release/Debug> <cpu/cuda/mkl> <32/64> <desktop/uwp> <optional: cuda version [92/100/101]

WORKDIR /app/
RUN rm -rf /app/Dlib/
RUN rm -rf /app/runtimes/centos-x64
RUN rm -rf /app/runtimes/osx-x64
RUN rm -rf /app/runtimes/win
RUN rm -rf /app/runtimes/win-x86
RUN rm -rf /app/runtimes/win-x64
RUN apt-get -s remove powershell
RUN apt-get -s remove git
RUN apt-get -s remove curl
RUN apt-get -s remove apt-transport-https
RUN apt autoremove

ENTRYPOINT ["dotnet", "Recognizer.Grpc.dll"]