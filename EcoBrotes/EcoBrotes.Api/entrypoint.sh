#!/bin/bash
set -e

echo "Starting API..."
exec dotnet EcoBrotes.Api.dll
