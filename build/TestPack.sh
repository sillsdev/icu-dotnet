#!/bin/bash
set -e
cd "$(dirname "$0")"
CONFIGURATION=${1:-Debug}
SLN=../source/icu.net.sln
dotnet build --configuration "$CONFIGURATION" $SLN
dotnet test -p:TargetFramework=net8.0 --configuration "$CONFIGURATION" --no-build $SLN
dotnet test -p:TargetFramework=net10.0 --configuration "$CONFIGURATION" --no-build $SLN
if [ "$OS" = "Windows_NT" ]; then
	dotnet test -p:TargetFramework=net462 --configuration "$CONFIGURATION" --no-build $SLN
fi
dotnet pack --configuration "$CONFIGURATION" --no-build --include-symbols $SLN
