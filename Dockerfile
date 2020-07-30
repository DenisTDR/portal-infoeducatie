FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build-env
WORKDIR /app/

COPY ./InfoEducatie.sln /app/src/InfoEducatie.sln
COPY ./MCMS/MCMS/MCMS.csproj /app/src/MCMS/MCMS/MCMS.csproj
COPY ./MCMS/MCMS.Base/MCMS.Base.csproj /app/src/MCMS/MCMS.Base/MCMS.Base.csproj
COPY ./MCMS/MCMS.Common/MCMS.Common.csproj /app/src/MCMS/MCMS.Common/MCMS.Common.csproj
COPY ./MCMS/MCMS.Files/MCMS.Files.csproj /app/src/MCMS/MCMS.Files/MCMS.Files.csproj
COPY ./MCMS/MCMS.Emailing/MCMS.Emailing.csproj /app/src/MCMS/MCMS.Emailing/MCMS.Emailing.csproj
COPY ./InfoEducatie.Main/InfoEducatie.Main.csproj /app/src/InfoEducatie.Main/InfoEducatie.Main.csproj
COPY ./InfoEducatie.Contest/InfoEducatie.Contest.csproj /app/src/InfoEducatie.Contest/InfoEducatie.Contest.csproj

WORKDIR /app/src/

RUN dotnet restore

COPY ./ /app/src

RUN dotnet publish --output "/app/bin" --configuration release 


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as runtime-env
RUN apt-get update && apt-get install -y \
    sudo \
    libgdiplus \
#    net-tools \
 && rm -rf /var/lib/apt/lists/*

WORKDIR /app/bin
EXPOSE 4450
ENV NETCORE_USER_UID 69

COPY docker-entrypoint.sh /usr/bin/docker-entrypoint.sh
RUN chmod +x /usr/bin/docker-entrypoint.sh
CMD ["docker-entrypoint.sh"]

COPY --from=build-env /app/bin /app/bin

