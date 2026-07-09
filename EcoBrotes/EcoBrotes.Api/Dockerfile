# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY EcoBrotes/EcoBrotes.Api/EcoBrotes.Api.csproj EcoBrotes/EcoBrotes.Api/
COPY EcoBrotes/EcoBrotes.Application/EcoBrotes.Application.csproj EcoBrotes/EcoBrotes.Application/
COPY EcoBrotes/EcoBrotes.Domain/EcoBrotes.Domain.csproj EcoBrotes/EcoBrotes.Domain/
COPY EcoBrotes/EcoBrotes.Infrastructure/EcoBrotes.Infrastructure.csproj EcoBrotes/EcoBrotes.Infrastructure/
RUN dotnet restore EcoBrotes/EcoBrotes.Api/EcoBrotes.Api.csproj
COPY . .
WORKDIR /src/EcoBrotes/EcoBrotes.Api
RUN dotnet build EcoBrotes.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish EcoBrotes.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY EcoBrotes/EcoBrotes.Api/entrypoint.sh .
RUN chmod +x entrypoint.sh
# Create the log file owned by the non-root 'app' user so it can write logs
# without granting world-writable permissions.
RUN mkdir -p /app/logs && touch /app/logs.txt && chown -R app:app /app/logs /app/logs.txt
# Run as the non-root user provided by the .NET base image
USER app
ENTRYPOINT ["./entrypoint.sh"]