# -------------------------------
# Stage 1: Build the .NET projects
# -------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files first and restore
COPY ["SignalR/SignalR.csproj", "SignalR/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

RUN dotnet restore "SignalR/SignalR.csproj"

# Copy everything else
COPY . .

# Build the SignalR project
WORKDIR /src/SignalR
RUN dotnet build "SignalR.csproj" -c $BUILD_CONFIGURATION -o /app/build

# -------------------------------
# Stage 2: Publish the SignalR
# -------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SignalR.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -------------------------------
# Stage 3: Runtime image
# -------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Expose ports: HTTP/SignalR, gRPC
EXPOSE 5020
EXPOSE 5021

# Copy published files
COPY --from=publish /app/publish .

# Entry point
ENTRYPOINT ["dotnet", "SignalR.dll"]
