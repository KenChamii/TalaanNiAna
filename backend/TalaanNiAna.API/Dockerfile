# Build context is the backend folder, so every project is reachable:
#   docker build -f TalaanNiAna.API/Dockerfile -t talaanniana-api ./backend

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Project files first, so restore is cached until a dependency actually changes.
COPY TalaanNiAna.Domain/*.csproj TalaanNiAna.Domain/
COPY TalaanNiAna.Application/*.csproj TalaanNiAna.Application/
COPY TalaanNiAna.Infrastructure/*.csproj TalaanNiAna.Infrastructure/
COPY TalaanNiAna.API/*.csproj TalaanNiAna.API/
RUN dotnet restore TalaanNiAna.API/TalaanNiAna.API.csproj

COPY . .
RUN dotnet publish TalaanNiAna.API/TalaanNiAna.API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Npgsql probes for Kerberos before falling back to password auth. Without this
# the connection still works, but every boot logs a missing-library error.
RUN apt-get update \
 && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
 && rm -rf /var/lib/apt/lists/*

# Render routes to whatever PORT it assigns; 8080 is the default elsewhere.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Not root. The app writes nothing to disk, so it needs no ownership anywhere.
USER $APP_UID

COPY --from=build /app .
ENTRYPOINT ["dotnet", "TalaanNiAna.API.dll"]
