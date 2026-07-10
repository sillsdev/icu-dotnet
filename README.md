# icu.net

## Overview

icu-dotnet is the C# wrapper for a subset of [ICU](https://icu.unicode.org/).

> ICU is a mature, widely used set of C/C++ and Java libraries providing Unicode and Globalization support
> for software applications. ICU is widely portable and gives applications the same results on all platforms
> and between C/C++ and Java software.

[![NuGet version (icu.net)](https://img.shields.io/nuget/v/icu.net.svg?style=flat-square)](https://www.nuget.org/packages/icu.net/)
[![Build, Test and Pack](https://github.com/sillsdev/icu-dotnet/actions/workflows/CI-CD.yml/badge.svg)](https://github.com/sillsdev/icu-dotnet/actions/workflows/CI-CD.yml)

## Usage

This library provides .NET classes and methods for (a subset of) the ICU C API. Please refer to the
[ICU API documentation](https://unicode-org.github.io/icu-docs/apidoc/released/icu4c/). In icu.net
you'll find classes that correspond to the C++ classes of ICU4C.

Although not strictly required it is recommended to call `Icu.Wrapper.Init()` at the start of
the application. This will allow to use icu.net from multiple threads
(c.f. [ICU Initialization and Termination](https://unicode-org.github.io/icu/userguide/icu/design.html#icu4c-initialization-and-termination)).
Similarly, it might be beneficial to call `Icu.Wrapper.Cleanup()` before exiting.

Sample code:

```csharp
    static class Program
    {
        public static void Main(string[] args)
        {
            Icu.Wrapper.Init();
            // Will output "NFC form of XA\u0308bc is XÄbc"
            Console.WriteLine($"NFC form of XA\\u0308bc is {Icu.Normalizer.Normalize("XA\u0308bc",
                Icu.Normalizer.UNormalizationMode.UNORM_NFC)}");
            Icu.Wrapper.Cleanup();
        }
    }
```

## Building

To build the current version of icu-dotnet you'll need .NET 8.0 or .NET 10.0 installed.

icu-dotnet can be built from the command line as well as Visual Studio or JetBrains Rider.

### Running Unit Tests

You can build and run the unit tests by running:

```bash
dotnet test source/icu.net.sln
```

or, if wanting to run tests on just one specific .net version (v8.0 in this example):

```bash
dotnet test source/icu.net.sln -p:TargetFramework=net8.0
```

### Android

> [!CAUTION]
> **Android support is limited.** Only **collation** APIs are supported for now. Other ICU
> functionality (normalization, break iteration, locale handling, and so on) is not yet
> available on Android. The Android test project references the full `icu.net.tests` suite,
> but CI and the helper scripts intentionally run only collation tests plus Android-specific
> smoke/diagnostic tests.

Prerequisites:

- .NET 10 SDK with the MAUI Android workload: `dotnet workload install maui-android`
- Android SDK (`ANDROID_HOME` set) with an emulator running or a USB device attached
- Android NDK (for building bundled ICU native libraries)

Build ICU for Android (outputs to `output/android-icu/`):

```bash
bash scripts/build-icu-android.sh --arch=x86_64
```

On Windows, use `scripts/build-icu-android.ps1` instead.

Run the filtered Android device test suite (collation + Android-specific tests only):

```bash
bash scripts/ci-android-tests.sh
```

On Windows, use `scripts/run-android-tests.ps1` instead. Both scripts apply the same test
filter as CI and print a reminder that only the filtered subset is expected to pass.

### Linux and macOS

It is important for `icu.net.dll.config` to be bundled with your application when not
running on Windows. If it doesn't copy reliably to the output directory, you might find
adding something like the following to your `csproj` file will resolve the issue. Note
that the version number in the path must match the version number of icu.net that is
referenced in the project.

```xml
<ItemGroup>
  <None Update="$(NuGetPackageRoot)\icu.net\2.9.0\contentFiles\any\any\icu.net.dll.config">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

### Docker

icu-dotnet depends on libc dynamic libraries at run time. If running within Docker, you may
need to install them, for example:

```Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:3.1

# Install system dependencies.
RUN apt-get update \
    && apt-get install -y \
        # icu.net dependency: libdl.so
        libc6-dev \
     && rm -rf /var/lib/apt/lists/*

...
```

## ICU versions

### Linux

icu-dotnet links with any installed version of ICU shared objects. It is
recommended to install the version provided by the distribution. For example:

| OS                      | ICU |
| ----------------------- | --- |
| Ubuntu 22.04 (Jammy)    | 70  |
| Ubuntu 24.04 (Noble)    | 74  |
| Ubuntu 26.04 (Resolute) | 78  |
| Debian 12 (Bookworm)    | 72  |
| Debian 13 (Trixie)      | 76  |

If the version provided by the Linux distribution doesn't match your needs,
[Microsoft's ICU package](https://www.nuget.org/packages/Microsoft.ICU.ICU4C.Runtime/)
includes builds for Linux.

### Windows

Rather than using the full version of ICU (which can be ~25 MB), a custom minimum
build can be used. It can be installed by the
[Icu4c.Win.Min](https://www.nuget.org/packages/Icu4c.Win.Min/) nuget package.
The full version of ICU is also available as
[Icu4c.Win.Full.Lib](https://www.nuget.org/packages/Icu4c.Win.Full.Lib/) and
[Icu4c.Win.Full.Bin](https://www.nuget.org/packages/Icu4c.Win.Full.Bin/).

Microsoft also makes the full version available as
[Microsoft.ICU.ICU4C.Runtime](https://www.nuget.org/packages/Microsoft.ICU.ICU4C.Runtime/).

#### What's in the minimum build

- Characters
- ErrorCodes
- Locale
- Normalizer
- Rules-based Collator
- Unicode set to pattern conversions

### macOS

macOS doesn't come preinstalled with all the normal icu4c libraries. They must be
installed separately via a package manager such as
[Homebrew](https://brew.sh/) or [MacPorts](https://www.macports.org/).

```bash
# Homebrew (more common)
brew install icu4c

# MacPorts
sudo port install icu
```

icu.net automatically searches the standard Homebrew and MacPorts installation
directories, so no extra configuration is needed after installing via either
package manager.

## Troubleshooting

- make sure you added the nuget package `icu.net` and have native ICU libraries available.
- the binaries of the nuget packages need to be copied to your output directory.
  For `icu.net` this happens by the assembly reference that the package
  adds to your project. The binaries of `Icu4c.Win.Min` are only relevant on
  Windows. They will get copied by the `Icu4c.Win.Min.targets` file included
  in the nuget package.

On Windows, the package installer should have added an import to the `*.csproj` file similar
to the following:

```xml
<Import Project="..\..\packages\Icu4c.Win.Min.54.1.31\build\Icu4c.Win.Min.targets"
    Condition="Exists('..\..\packages\Icu4c.Win.Min.54.1.31\build\Icu4c.Win.Min.targets')" />
```

## Contributing

We love contributions! The library mainly contains the functionality we need for our products. If you
miss something that is part of ICU4C but not yet wrapped in icu.net, add it and create a pull request.

If you find a bug - create an [issue on GitHub](https://github.com/sillsdev/icu-dotnet/issues/new/choose),
then preferably fix it and create a pull request!
