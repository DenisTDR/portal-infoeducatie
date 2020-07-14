#!/bin/sh

set -e

groupadd --gid ${NETCORE_USER_UID} netcore && useradd --uid ${NETCORE_USER_UID} -g netcore netcore

# chmod -R u=rwX,g=rX,o= /app/
# chown netcore:netcore /app/
# chown -RL netcore:netcore /app/bin/

mkdir -p ${PERSISTED_KEYS_DIRECTORY}
chmod -R u=rwX,g=rX,o= /app/persisted
chown -RL netcore:netcore /app/persisted

# apt-get update && apt-get install -y sudo net-tools  && rm -rf /var/lib/apt/lists/*
cd /app/bin
# sudo -E -u netcore dotnet API.StartApp.dll --migrate true --seed true
# exec sudo -E -u netcore dotnet API.StartApp.dll
exec sudo -E -u netcore dotnet InfoEducatie.Main.dll