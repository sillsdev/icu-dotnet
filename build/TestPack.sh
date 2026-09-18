#!/bin/bash
cd "$(dirname "$0")"
CONFIGURATION=${1:-Debug}
dotnet build --configuration "$CONFIGURATION" ../source/icu.net.sln
dotnet test --configuration "$CONFIGURATION" --no-build ../source/icu.net.sln
dotnet pack --configuration "$CONFIGURATION" --no-build --include-symbols ../source/icu.net.sln
