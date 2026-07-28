FROM node:24-alpine AS frontend-build

WORKDIR /src/Frontend

COPY Frontend/package.json Frontend/package-lock.json ./

RUN npm ci

COPY Frontend/ ./

RUN npm run build


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build

WORKDIR /src

COPY PersonalSite.Api/PersonalSite.Api.csproj PersonalSite.Api/

RUN dotnet restore PersonalSite.Api/PersonalSite.Api.csproj

COPY PersonalSite.Api/ PersonalSite.Api/

WORKDIR /src/PersonalSite.Api

RUN dotnet publish \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=backend-build /app/publish .
COPY --from=frontend-build /src/Frontend/dist ./wwwroot

ENTRYPOINT ["dotnet", "PersonalSite.Api.dll"]