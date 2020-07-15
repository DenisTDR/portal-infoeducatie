#!/usr/bin/env bash
rm -rf ../build/*
dotnet publish --output "../build" --configuration release 
