FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY backend/API/RedMobilePedidos.API.csproj API/
COPY backend/Tests/RedMobilePedidos.Tests.csproj Tests/
RUN dotnet restore API/RedMobilePedidos.API.csproj

COPY backend/ .

WORKDIR /src/API
RUN dotnet publish RedMobilePedidos.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

RUN apt-get update && apt-get install -y \
    curl \
    wget \
    gnupg \
    fonts-liberation \
    --no-install-recommends \
    && wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | gpg --dearmor -o /usr/share/keyrings/google-chrome.gpg \
    && echo "deb [arch=amd64 signed-by=/usr/share/keyrings/google-chrome.gpg] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google-chrome.list \
    && apt-get update \
    && apt-get install -y google-chrome-stable --no-install-recommends \
    && rm -rf /var/lib/apt/lists/*

ENV PUPPETEER_EXECUTABLE_PATH=/usr/bin/google-chrome-stable
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/api/ping || exit 1

ENTRYPOINT ["dotnet", "RedMobilePedidos.API.dll"]