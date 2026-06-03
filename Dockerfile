FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY DomParser.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet build --configuration Release --no-restore

FROM nginx:1.27-alpine AS runtime

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /src/index.html /usr/share/nginx/html/index.html
COPY --from=build /src/styles.css /usr/share/nginx/html/styles.css
COPY --from=build /src/script.mjs /usr/share/nginx/html/script.mjs
COPY --from=build /src/dom-core.mjs /usr/share/nginx/html/dom-core.mjs

EXPOSE 4174
