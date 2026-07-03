#Requires -Version 5.1
<#
.SYNOPSIS
    Cross-compile ICU4C for Android (one-time setup, like brew install icu4c on macOS).

.DESCRIPTION
    Invokes scripts/build-icu-android.sh via Git Bash or WSL bash.
    Output (gitignored): output/android-icu/{x86_64,arm64-v8a}/libicu*.so*

    Prerequisites:
    - Android NDK (ANDROID_NDK_HOME or ANDROID_HOME/ndk/<version>)
    - Git Bash or WSL with: bash, curl, tar, make

.EXAMPLE
    .\scripts\build-icu-android.ps1
    .\scripts\build-icu-android.ps1 -Arch x86_64
    .\scripts\build-icu-android.ps1 -Arch x86_64,arm64-v8a
#>
[CmdletBinding()]
param(
    [string] $Arch = 'x86_64',
    [string] $IcuVersion = '72.1',
    [int] $ApiLevel = 21,
    [switch] $Clean
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$bashScript = Join-Path $PSScriptRoot 'build-icu-android.sh'

function Find-Bash {
    $candidates = @(
        (Get-Command bash -ErrorAction SilentlyContinue)?.Source,
        'C:\Program Files\Git\bin\bash.exe',
        'C:\Program Files (x86)\Git\bin\bash.exe'
    ) | Where-Object { $_ -and (Test-Path $_) }
    return $candidates | Select-Object -First 1
}

$bash = Find-Bash
if (-not $bash) {
    Write-Error @"
bash not found. Install Git for Windows or use WSL, then re-run this script.
Alternatively run: bash scripts/build-icu-android.sh --arch=$Arch
"@
}

$args = @(
    $bashScript
    "--arch=$Arch"
    "--api=$ApiLevel"
    "--icu-version=$IcuVersion"
)
if ($Clean) { $args += '--clean' }

Write-Host "Running: $bash $($args -join ' ')"
& $bash @args
exit $LASTEXITCODE
