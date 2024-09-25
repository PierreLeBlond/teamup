# Adjust DOTNET_OS_VERSION as desired
ARG DOTNET_OS_VERSION="-alpine"
ARG DOTNET_SDK_VERSION=8.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_VERSION}${DOTNET_OS_VERSION} AS build
WORKDIR /src

# copy everything
COPY . ./


# restore as distinct layers
RUN dotnet restore
# build and publish a release
RUN dotnet publish -c Release -o /app
# create the database
RUN dotnet tool restore
RUN dotnet ef database update

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_SDK_VERSION}
ENV ASPNETCORE_URLS http://+:8080
ENV ASPNETCORE_ENVIRONMENT Production
EXPOSE 8080
WORKDIR /app
COPY --from=build /app .
COPY --from=build src/webapp.db .
ENTRYPOINT [ "dotnet", "Webapp.dll" ]
