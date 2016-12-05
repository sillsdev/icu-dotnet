Overview
========
icu-dotnet is the C# wrapper for a subset of [ICU4C] (http://site.icu-project.org/home#TOC-What-is-ICU-t "ICU for C").
>ICU is a mature, widely used set of C/C++ and Java libraries providing Unicode and Globalization support for software applications. ICU is widely portable and gives applications the same results on all platforms and between C/C++ and Java software.

## Status

| OS      | Status |
| ------- | ------ |
| Linux   | [![Build Status](https://jenkins.lsdev.sil.org/view/IcuDotNet/view/All/job/IcuDotNet-Linux-any-master-release/badge/icon)](https://jenkins.lsdev.sil.org/view/IcuDotNet/view/All/job/IcuDotNet-Linux-any-master-release/) |
| Windows | [![Build Status](https://jenkins.lsdev.sil.org/view/IcuDotNet/view/All/job/IcuDotNet-Win-any-master-release/badge/icon)](https://jenkins.lsdev.sil.org/view/IcuDotNet/view/All/job/IcuDotNet-Win-any-master-release/) |

## Building

icu-dotnet can be built with Visual Studio or MonoDevelop, but at least initially it might be
easier to build from the command line because that will download all necessary dependencies.

### Linux

You can build and run the unit tests by running:

    build/TestBuild.sh

If you run into issues you might want to try with a newer mono version or with our custom `mono-sil` package from [packages.sil.org](http://packages.sil.org/)

### Windows

Build and run the unit tests by running:

    msbuild /t:Test build/icu-dotnet.proj

## ICU versions

### Linux
icu-dotnet links with the full versions of ICU shared objects provided by the distribution.  As of 2015, Ubuntu Trusty uses version ICU 52.  When building icu-dotnet, the corresponding version of ICU is specified with the environment variable *icu_ver*.

### Windows
Rather than using the full version of ICU (which can be ~25 MB), a custom minimum build can be used.  The instructions based on this [reference](http://qt-project.org/wiki/Compiling-ICU-with-MSVC) for creating the minimum ICU 54.1 build are below

1. [Download](http://site.icu-project.org/download/54 "ICU 54.1") ICU source
2. [Customize](http://apps.icu-project.org/datacustom/index.html) ICU Data library by selecting the following:
    - Collators/coll/ucadata.icu
    - Get ICU4C Data library and extract to *icu/source/data/in*
3. Modify the following ICU source files:
    - *icu/source/common/unicode/uconfig.h*
        ```C#
        #define UCONFIG_ONLY_COLLATION 1
        #define UCONFIG_NO_LEGACY_CONVERSION 1
        ```

    - *icu/source/i18n/sharedpluralrules.h*
        ```C#
        virtual ~SharedPluralRules() { delete ptr; }
        ```
4. Build ICU VS solution in *icu/source/allinone/allinone.sln*.
    - Choose Configuration **Release** and Target Platform **Win32**.  There will be errors about "tstfiles.mk not found" that can be ignored.

#### What's in the minimum build
- Characters
- ErrorCodes
- Locale
- Normalizer
- Rules-based Collator
- Unicode set to pattern conversions
