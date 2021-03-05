# Overview

icu-dotnet is the C# wrapper for a subset of [ICU4C](http://site.icu-project.org/home#TOC-What-is-ICU-) "ICU for C".
>ICU is a mature, widely used set of C/C++ and Java libraries providing Unicode and Globalization support for software applications. ICU is widely portable and gives applications the same results on all platforms and between C/C++ and Java software.

## Status

| OS      | Status |
| ------- | ------ |
| Linux   | [![Build Status](https://jenkins.lsdev.sil.org/view/Icu/view/All/job/IcuDotNet-Linux-any-master-release/badge/icon)](https://jenkins.lsdev.sil.org/view/Icu/view/All/job/IcuDotNet-Linux-any-master-release/) |
| Windows | [![Build Status](https://jenkins.lsdev.sil.org/view/Icu/view/All/job/IcuDotNet-Win-any-master-release/badge/icon)](https://jenkins.lsdev.sil.org/view/Icu/view/All/job/IcuDotNet-Win-any-master-release/) |

## Usage

This library provides .NET classes and methods for (a subset of) the ICU C API. Please refer to the
[ICU API documentation](http://icu-project.org/apiref/icu4c/). In icu.net you'll find classes that
correspond to the C++ classes of ICU4C.

Although not strictly required it is recommended to call `Icu.Wrapper.Init()` at the start of
the application. This will allow to use icu.net from multiple threads
(c.f. [ICU Initialization and Termination](http://userguide.icu-project.org/design#TOC-ICU-Initialization-and-Termination)).
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

icu-dotnet can be built with Visual Studio or MonoDevelop, but at least initially it might be
easier to build from the command line because that will download all necessary dependencies.

### Linux

You can build and run the unit tests by running:

    build/TestBuild.sh

If you run into issues you might want to try with a newer mono version or with our custom `mono-sil` package from [packages.sil.org](http://packages.sil.org/)

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

### Windows

Build and run the unit tests by running:

    msbuild /t:Test build/icu-dotnet.proj

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

If you find a bug - create an issue on GitHub, then preferably fix it and create a pull request!
