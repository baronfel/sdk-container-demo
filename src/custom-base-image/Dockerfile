FROM mcr.microsoft.com/dotnet/runtime:8.0 as base

RUN apt-get update && apt-get install -y \
    libfreetype6 \
    libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*
