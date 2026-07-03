#!/usr/bin/env bash
# Copyright (c) 2026 SIL Global
# Run icu.net Android device tests (CI / Linux). Requires a booted emulator (adb devices).
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT="$REPO_ROOT/source/icu.net.android.tests/icu.net.android.tests.csproj"

export IcuDotNetIncludeAndroid=true

echo "ANDROID_HOME=${ANDROID_HOME:-}"
echo "ANDROID_NDK_HOME=${ANDROID_NDK_HOME:-}"
echo "Connected devices:"
adb devices

dotnet restore "$PROJECT" -p:IcuDotNetIncludeAndroid=true
dotnet test "$PROJECT" -f net10.0-android -c Release \
  -p:IcuDotNetIncludeAndroid=true \
  --no-restore \
  --filter "FullyQualifiedName~icu.net.android.tests.Tests" \
  --logger "trx;LogFileName=test-results.trx"
