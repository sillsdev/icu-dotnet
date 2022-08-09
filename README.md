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

``` csharp
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

To build the current version of icu-dotnet you'll need .net 6.0 installed.

icu-dotnet can be built from the command line as well as Visual Studio or JetBrains Rider.

### Windows and Linux

You can build and run the unit tests by running:

```bash
dotnet test source/icu.net.sln
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
recommended to install the version provided by the distribution.  As of 2016,
Ubuntu Trusty uses version ICU 52 and Ubuntu Xenial 55.

### Windows

Rather than using the full version of ICU (which can be ~25 MB), a custom minimum
build can be used. It can be installed by the
[Icu4c.Win.Min](https://www.nuget.org/packages/Icu4c.Win.Min/) nuget package.
The full version of ICU is also available as
[Icu4c.Win.Full.Lib](https://www.nuget.org/packages/Icu4c.Win.Full.Lib/) and
[Icu4c.Win.Full.Bin](https://www.nuget.org/packages/Icu4c.Win.Full.Bin/).

#### What's in the minimum build

- Characters
- ErrorCodes
- Locale
- Normalizer
- Rules-based Collator
- Unicode set to pattern conversions

## Troubleshooting

- make sure you added the nuget packages `icu.net` and `Icu4c.Win.Min`
  (or `Icu4c.Win.Full`).
- the binaries of the nuget packages need to be copied to your output directory.
  For `icu.net` this happens by the assembly reference that the package
  adds to your project. The binaries of `Icu4c.Win.Min` are only relevant on
  Windows. They will get copied by the `Icu4c.Win.Min.targets` file included
  in the nuget package.

The package installer should have added an import to the `*.csproj` file similar to the following:

```xml
<Import Project="..\..\packages\Icu4c.Win.Min.54.1.31\build\Icu4c.Win.Min.targets"
    Condition="Exists('..\..\packages\Icu4c.Win.Min.54.1.31\build\Icu4c.Win.Min.targets')" />
```

## Contributing

We love contributions! The library mainly contains the functionality we need for our products. If you
miss something that is part of ICU4C but not yet wrapped in icu.net, add it and create a pull request.

If you find a bug - create an [issue on GitHub](https://github.com/sillsdev/icu-dotnet/issues/new/choose),
then preferably fix it and create a pull request!
