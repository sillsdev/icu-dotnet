#Requires -Version 5.1
<#
.SYNOPSIS
    Run icu.net Android device smoke tests via dotnet test.

.DESCRIPTION
    Prerequisites:
    - .NET 10 SDK with MAUI Android workload: dotnet workload install maui-android
    - Android SDK (ANDROID_HOME set) with an emulator running or USB device attached
    - Verify device connectivity: adb devices

.EXAMPLE
    .\scripts\run-android-tests.ps1
    .\scripts\run-android-tests.ps1 -Configuration Debug
#>
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot 'source\icu.net.android.tests\icu.net.android.tests.csproj'

function Find-Adb {
    if ($env:ANDROID_HOME) {
        $adb = Join-Path $env:ANDROID_HOME 'platform-tools\adb.exe'
        if (Test-Path $adb) { return $adb }
    }
    $adbOnPath = Get-Command adb -ErrorAction SilentlyContinue
    if ($adbOnPath) { return $adbOnPath.Source }
    return $null
}

$adb = Find-Adb
if (-not $adb) {
    Write-Error @"
adb not found. Install the Android SDK and set ANDROID_HOME, or add platform-tools to PATH.
Example: dotnet workload install maui-android
"@
}

$devices = & $adb devices 2>&1 | Select-Object -Skip 1 | Where-Object { $_ -match '\t' -and $_ -notmatch 'offline' }
if (-not $devices) {
    Write-Error @"
No Android emulator or device detected. Start an emulator or connect a device, then run:
  adb devices
"@
}

Write-Host "Using adb: $adb"
Write-Host "Connected devices:"
$devices | ForEach-Object { Write-Host "  $_" }

Write-Host "Running Android device tests ($Configuration)..."
dotnet test $project -f net10.0-android -c $Configuration
exit $LASTEXITCODE
