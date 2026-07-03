#Requires -Version 5.1
<#
.SYNOPSIS
    Probe an attached Android device/emulator for system ICU libraries (no app rebuild).

.DESCRIPTION
    Reports API level, ABI, and whether platform ICU files exist in typical system paths.
    Useful to confirm that dlopen of libicuuc.so is not available to apps on modern Android.

.EXAMPLE
    .\scripts\probe-android-icu.ps1
#>
[CmdletBinding()]
param(
    [string] $PackageName = 'com.sil.icu.android.tests'
)

$ErrorActionPreference = 'Stop'

function Find-Adb {
    if ($env:ANDROID_HOME) {
        $adb = Join-Path $env:ANDROID_HOME 'platform-tools\adb.exe'
        if (Test-Path $adb) { return $adb }
    }
    $adbOnPath = Get-Command adb -ErrorAction SilentlyContinue
    if ($adbOnPath) { return $adbOnPath.Source }
    return $null
}

function Invoke-AdbShell([string] $Command) {
    & $adb shell $Command 2>&1 | ForEach-Object { "$_" }
}

function Test-RemotePath([string] $Path) {
    $result = Invoke-AdbShell "test -e `"$Path`" && echo exists || echo missing"
    return ($result -match 'exists')
}

function Get-RemoteListing([string] $Path) {
    if (-not (Test-RemotePath $Path)) {
        return @("(path not found)")
    }
    $lines = Invoke-AdbShell "ls -la `"$Path`" 2>/dev/null"
    if (-not $lines) { return @("(empty)") }
    return $lines
}

function Find-RemoteIcuLibs([string[]] $Dirs) {
    $found = @()
    foreach ($dir in $Dirs) {
        if (-not (Test-RemotePath $dir)) { continue }
        $matches = Invoke-AdbShell "ls `"$dir`" 2>/dev/null | grep -E 'libicu(uc|i18n|data|)\.so' || true"
        foreach ($line in $matches) {
            if ($line -and $line -notmatch '^\s*$') {
                $found += "$dir/$line"
            }
        }
    }
    return $found
}

$adb = Find-Adb
if (-not $adb) {
    Write-Error "adb not found. Set ANDROID_HOME or add platform-tools to PATH."
}

$devices = & $adb devices 2>&1 | Select-Object -Skip 1 | Where-Object { $_ -match '\t' -and $_ -notmatch 'offline' }
if (-not $devices) {
    Write-Error "No Android device/emulator connected. Run: adb devices"
}

Write-Host "=== Android ICU probe ===" -ForegroundColor Cyan
Write-Host "adb: $adb"
Write-Host "device: $($devices[0])"
Write-Host ""

$sdk = (Invoke-AdbShell 'getprop ro.build.version.sdk' | Select-Object -First 1).Trim()
$release = (Invoke-AdbShell 'getprop ro.build.version.release' | Select-Object -First 1).Trim()
$abi = (Invoke-AdbShell 'getprop ro.product.cpu.abi' | Select-Object -First 1).Trim()
$abilist = (Invoke-AdbShell 'getprop ro.product.cpu.abilist' | Select-Object -First 1).Trim()

Write-Host "--- Device ---"
Write-Host "API level: $sdk (Android $release)"
Write-Host "Primary ABI: $abi"
Write-Host "ABI list: $abilist"
Write-Host ""

Write-Host "--- ICU data (/system/usr/icu) ---"
Get-RemoteListing '/system/usr/icu' | ForEach-Object { Write-Host "  $_" }
Write-Host ""

$libDirs = @(
    '/system/lib64',
    '/system/lib',
    '/apex/com.android.i18n/lib64',
    '/apex/com.android.i18n/lib',
    '/apex/com.android.runtime/lib64',
    '/apex/com.android.runtime/lib',
    '/apex/com.android.art/lib64',
    '/apex/com.android.art/lib'
)

Write-Host "--- ICU-related .so in system/apex lib dirs ---"
$icuLibs = Find-RemoteIcuLibs $libDirs
if ($icuLibs.Count -eq 0) {
    Write-Host "  (none found in searched paths)"
} else {
    $icuLibs | ForEach-Object { Write-Host "  $_" }
}
Write-Host ""

Write-Host "--- App native lib dir ($PackageName) ---"
$appLibDir = (Invoke-AdbShell "run-as $PackageName ls lib 2>/dev/null || echo" | Select-Object -First 1)
if ($appLibDir -match 'not debuggable|Permission denied|No such file') {
    Write-Host "  (app not installed, not debuggable, or run-as unavailable)"
    $nativeDir = Invoke-AdbShell "pm path $PackageName 2>/dev/null"
    if ($nativeDir) {
        Write-Host "  apk: $nativeDir"
    }
} else {
    $abiFolder = if ($abi -match '64') { 'x86_64', 'arm64-v8a' } else { 'x86', 'armeabi-v7a' }
    foreach ($folder in $abiFolder) {
        Write-Host "  lib/$folder :"
        Get-RemoteListing "lib/$folder" | ForEach-Object { Write-Host "    $_" }
    }
}

Write-Host ""
Write-Host "--- Notes ---" -ForegroundColor Yellow
Write-Host "icu.net expects versioned libicuuc.so.N and libicui18n.so.N in the app native lib directory."
Write-Host "Since API 24, apps generally cannot dlopen platform libicuuc.so / libicui18n.so."
Write-Host "Bundled ICU (see scripts/build-icu-android.ps1) is the supported path for icu.net on Android."
