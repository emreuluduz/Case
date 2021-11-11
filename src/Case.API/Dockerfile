FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine as build-ENV
COPY /Case.Shared ./app/Case.Shared
COPY /Case.Core ./app/Case.Core
COPY /Case.Infrastructure ./app/Case.Infrastructure
COPY /Case.Application ./app/Case.Application
COPY /Case.API ./app/Case.API
WORKDIR /app/Case.API
RUN dotnet publish -c Release -r linux-musl-x64 -o publish-folder

FROM mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine as runtime
COPY --from=build-env /app/Case.API/publish-folder ./
RUN apk add --update \
    iputils \
    curl \
    icu-libs \
    && rm -rf /var/cache/apk/*
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["./Case.API"]