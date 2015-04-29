Overview
========
icu-dotnet is the C# wrapper for a subset of [ICU4C] (http://site.icu-project.org/home#TOC-What-is-ICU-t "ICU for C").
>ICU is a mature, widely used set of C/C++ and Java libraries providing Unicode and Globalization support for software applications. ICU is widely portable and gives applications the same results on all platforms and between C/C++ and Java software.

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
5. Upload dlls from *icu/bin* to [downloads.palaso.org](http://downloads.palaso.org/icu/icu4c-54.1-win32-min/ "icu4c-54.1-win32-min")

#### What's in the minimum build
- BreakIterator
- Characters
- ErrorCodes
- Locale
- Normalizer
- Rules-based Collator
- Unicode set to pattern conversions

