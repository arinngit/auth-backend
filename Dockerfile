FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY AuthenticationMicroservice.sln ./
COPY AuthenticationMicroservice.Api/*.csproj AuthenticationMicroservice.Api/
COPY AuthenticationMicroservice.Business/*.csproj AuthenticationMicroservice.Business/
COPY AuthenticationMicroservice.Infrastructure/*.csproj AuthenticationMicroservice.Infrastructure/
COPY AuthenticationMicroservice.Contracts/*.csproj AuthenticationMicroservice.Contracts/
COPY AuthenticationMicroservice.Domain/*.csproj AuthenticationMicroservice.Domain/
COPY AuthenticationMicroservice.DataAccess/*.csproj AuthenticationMicroservice.DataAccess/
COPY AuthenticationMicroservice.Tests/*.csproj AuthenticationMicroservice.Tests/

RUN dotnet restore

COPY . .

WORKDIR /src/AuthenticationMicroservice.Api

RUN dotnet publish AuthenticationMicroservice.Api.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/out ./

EXPOSE 5154

ENTRYPOINT ["/bin/sh", "-c", "\
  echo '⏳ Waiting for PostgreSQL...'; \
  for i in $(seq 1 30); do \
  timeout 1 bash -c 'cat < /dev/null > /dev/tcp/postgres/5432' 2>/dev/null && break; \
  echo 'Postgres not ready yet... retrying'; \
  sleep 1; \
  done; \
  echo 'PostgreSQL is up - starting app'; \
  dotnet AuthenticationMicroservice.Api.dll"]
