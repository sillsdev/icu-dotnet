#!/bin/bash
cd "$(dirname "$0")"
msbuild /t:${2:-Test} /p:Configuration=${1:-Debug} /p:BUILD_NUMBER="0.1.0.abcd" icu-dotnet.proj
