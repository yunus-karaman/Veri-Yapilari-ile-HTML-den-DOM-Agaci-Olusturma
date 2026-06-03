FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY DomParser.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet build --configuration Release --no-restore

FROM python:3.11-slim AS runtime
WORKDIR /app

COPY --from=build /src ./

EXPOSE 4174
CMD ["python", "local_server.py"]
