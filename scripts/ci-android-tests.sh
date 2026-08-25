#!/usr/bin/env bash
# Copyright (c) 2026 SIL Global
# Run icu.net Android device tests (CI / Linux). Requires a booted emulator (adb devices).
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT="$REPO_ROOT/source/icu.net.android.tests/icu.net.android.tests.csproj"

export IcuDotNetIncludeAndroid=true

# Android support is limited to collation for now. CI intentionally excludes the rest of
# icu.net.tests; only collation tests and Android-specific smoke/diagnostic tests are run.
ANDROID_TEST_FILTER='FullyQualifiedName~Icu.Tests.Collation|FullyQualifiedName~icu.net.android.tests.Tests'

echo "ANDROID_HOME=${ANDROID_HOME:-}"
echo "ANDROID_NDK_HOME=${ANDROID_NDK_HOME:-}"
echo "Connected devices:"
adb devices
echo ""
echo "NOTE: Android CI runs only collation tests and Android-specific tests."
echo "      Other icu.net.tests are excluded; only these subsets are expected to pass."
echo "      Filter: ${ANDROID_TEST_FILTER}"
echo ""

dotnet restore "$PROJECT" -p:IcuDotNetIncludeAndroid=true
dotnet test "$PROJECT" -f net10.0-android -c Release \
  -p:IcuDotNetIncludeAndroid=true \
  --no-restore \
  --filter "$ANDROID_TEST_FILTER" \
  --logger "trx;LogFileName=test-results.trx"
