FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

COPY ./InfoEducatie.Base/InfoEducatie.Base.csproj /app/src/InfoEducatie.Base/InfoEducatie.Base.csproj
COPY ./InfoEducatie.Main/InfoEducatie.Main.csproj /app/src/InfoEducatie.Main/InfoEducatie.Main.csproj
COPY ./InfoEducatie.Contest/InfoEducatie.Contest.csproj /app/src/InfoEducatie.Contest/InfoEducatie.Contest.csproj

WORKDIR /app/src/InfoEducatie.Main

ARG ENV_TYPE=CI_BUILD
COPY InfoEducatie.Main/nuget.config /root/.nuget/NuGet/NuGet.Config
RUN dotnet restore

COPY ./ /app/src

RUN dotnet publish --output "/app/bin" --configuration release 


FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime-env
RUN apt-get update && apt-get install -y \
    sudo \
    curl \
    libgdiplus \
#    net-tools \
 && rm -rf /var/lib/apt/lists/*

WORKDIR /app/bin
EXPOSE 80
ENV NETCORE_USER_UID 1000

COPY docker-entrypoint.sh /usr/bin/docker-entrypoint.sh
RUN chmod +x /usr/bin/docker-entrypoint.sh
CMD ["docker-entrypoint.sh"]

HEALTHCHECK --start-period=5s --interval=5s CMD curl -f -s localhost:80/health > /dev/null || exit 1

COPY --from=build-env /app/bin /app/bin

