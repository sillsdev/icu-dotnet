# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/)
and this project adheres to [Semantic Versioning](https://semver.org/).

<!-- Available types of changes:
### Added
### Changed
### Fixed
### Deprecated
### Removed
### Security
-->

## [Unreleased]

### Added

- NuGet package now includes the XML documentation file, enabling IntelliSense summaries in Visual Studio.
- In Character class, added all enums from Unicode's uchar.h that were missing:
  UBidiPairedBracketType, UBlockCode, UEastAsianWidth, UPropertyNameChoice, UJoiningType,
  UJoiningGroup, UGraphemeClusterBreak, UWordBreakValues, USentenceBreak, ULineBreak,
  UHangulSyllableType, UIndicPositionalCategory, UIndicSyllabicCategory, UIndicConjunctBreak,
  UVerticalOrientation, UIdentifierStatus, UIdentifierType.
- Added net10.0 target framework.

### Fixed

- Fixed `Transliterator.Transliterate` throwing `OverflowException` for characters that expand
  greatly during transliteration (e.g. U+FDFA ﷺ): the method now retries with a larger buffer
  on `BUFFER_OVERFLOW_ERROR` instead of immediately throwing.
- Fixed `Wrapper.ConfineIcuVersions` being ignored during library discovery: `CheckDirectoryForIcuBinaries`
  now filters candidates to the confined version range before selecting the highest match.
- Fixed macOS crash at process exit (.NET 6+): `u_cleanup()` and `NativeLibrary.Free` are now
  skipped on macOS so dyld does not fire ICU's destructor against already-cleaned state. Also
  fixed an independent ordering bug on all platforms: `u_cleanup()` was previously called after
  `ResetIcuVersionInfo()`, causing the runtime to look up the nonexistent symbol `u_cleanup_0`
  and silently skip the call.
- Fixed ICU library discovery on macOS: `LocateIcuLibrary` now falls back to Homebrew
  (`/opt/homebrew/opt/icu4c/lib` on Apple Silicon, `/usr/local/opt/icu4c/lib` on Intel) and
  MacPorts (`/opt/local/lib`) when no bundled ICU is found. Bundled ICU (in the assembly
  directory or `runtimes/` subdirectories) is always preferred over system installations.
- Removed no-op `LD_LIBRARY_PATH` manipulation on macOS. (Changing it to the mac-specific
  `DYLD_LIBRARY_PATH` would also be a no-op, because SIP strips all `DYLD_*` variables from
  protected processes at launch, so setting it at runtime has no effect.)
- Fixed `umsg_open` `locale` parameter marshaling from Unicode to ANSI, correcting ICU message
  formatting on macOS where the locale string was being passed as wide characters.
- Fixed `SafeEnumeratorHandle` and `Transliterator.SafeTransliteratorHandle` finalizers to
  silently swallow exceptions during .NET shutdown, when ICU may no longer be accessible.
- Fixed test teardown instability on macOS: `NativeMethodsHelperTests` now deletes dummy ICU
  files before resetting version state (preventing "Can't load ICU library (version 90)" failures
  in subsequent tests); `IcuWrapperTests` now skips `ConfineIcuVersions` on macOS, where
  `NativeLibrary.Free` is omitted so the library stays resident and version constraints must not
  be reset against it.
- Fixed `IsInitialized` not being reset on cleanup paths that skip `u_cleanup()`: it was a side
  effect of `u_cleanup()` rather than an explicit step, so any code path that skipped
  `u_cleanup()` (e.g. macOS on .NET 6+) would leave `IsInitialized = true` after cleanup.
  `IsInitialized = false` is now set unconditionally in `Cleanup()` and removed from `u_cleanup()`.
- Fixed `MessageFormatter.Format` crashing on ARM64 (.NET only): it now throws
  `PlatformNotSupportedException` instead. The AAPCS64 calling convention passes variadic
  float arguments through integer registers, incompatible with .NET's fixed-slot P/Invoke
  marshaling of the variadic C function `umsg_format`.
- Fixed `Transliterator.GetDisplayName` and `GetIdsAndNames` on ARM64: both now catch the
  `PlatformNotSupportedException` from `umsg_format` and fall back to the English
  "source to target" display name form. On non-ARM64 Unix with ICU 74+, where a calling-convention
  mismatch produces empty output rather than a crash, the existing `IsNullOrEmpty` fallback now
  covers macOS in addition to Linux.
- Fixed `Transliterator.GetDisplayName` returning empty display names on Linux ICU 74+.
  `umsg_format` is a variadic C function; on Linux ICU 74+ a calling-convention mismatch
  causes the `double` argument to be read as 0, producing empty output from the
  `TransliteratorNamePattern` choice format. The method now falls back to constructing
  the display name directly from the localized source and target script names.
- Fixed regex patterns in `NativeMethodsHelper` for Linux (`libicu*.so.*`) and macOS
  (`libicu*.dylib`): unescaped `.` matched any character instead of a literal dot.
- Fixed `NativeMethodsHelper` combined regex: `$` end-anchor now applies to all three
  platform alternatives, not only the macOS branch.
- Fixed `NativeMethodsHelper` regex patch-version segments: `(\.[0-9])*` changed to
  `(\.[0-9]+)*` to allow multi-digit patch components.
- Fixed `TimeZoneTests.GetTZVersionTest`: version pattern is now anchored (`^[0-9]{4}[a-z]$`)
  so it validates the full string rather than a substring.

### Deprecated

- In Character class, added \[Obsolete\] attribute to enum members UDecompositionType.COUNT and
  UNumericType.COUNT.

### Security

- Upgraded `Microsoft.Extensions.DependencyModel` from 2.0.4 to 10.0.9 on non-.NET-Framework
  targets, eliminating the transitive dependency on `Newtonsoft.Json` 9.0.1 (high severity
  vulnerability). The `net451` target retains `Microsoft.Extensions.DependencyModel` 2.1.0 (the
  newest version with net451 support) and pins the latest `Newtonsoft.Json`.

## [3.0.1] - 2025-02-21

### Fixed

- Update CI to use supported Ubuntu and macOS runner versions

## [3.0.0] - 2024-11-21

### Added

- Added support for netstandard2.0

### Changed

- Exception messages on .NET 6+ contain more information when dynamic library loading fails
- Update dependencies to the latest stable versions

### Fixed

- Fixed a bug when using a library compiled against icu-dotnet netstandard1.6, when your project referenced a different version of icu-dotnet

### Removed

- Removed support for [netstandard1.6](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-1-6#select-net-standard-version)
- Removed Icu.SortKey class which was only in the netstandard1.6 version of the dll

## [2.10.0] - 2024-06-17

### Added

- Support macOS

### Changed

- Move .NET 6.0 builds to .NET 8.0
- Update some GitHub Actions versions

## [2.9.0] - 2023-02-15

### Added

- Support .net 6.0

### Fixed

- Fixed crash in `Wrapper.Cleanup` (#176)

## [2.8.1] - 2022-07-08

### Fixed

- Fix bug in `UnicodeSet.ToCharacters()` with upper Unicode planes (LT-21010)

## [2.8.0] - 2022-06-24

### Added

- Added `Wrapper.SetPreferredIcu4cDirectory()` method to specify a
  directory where to preferably look for icu4c

### Changed

- Increased maximum supported version to 90 (#167)

### Fixed

- Fix a problem confining ICU version if it's located in a different
  directory. See `Wrapper.SetPreferredIcu4cDirectory()`.
- Also check in `runtimes/win7-*/native` for ICU binaries
- Include `icu.net.dll.config` file in nuget package. This is important for running on MacOSX.
- Fix construction of locale with language and keywords (cbersch)
- Fix passing locale to ubrk_open (cbersch)
- Fix race condition during initialization of native methods container (cbersch)
- Change .NET Standard target to reference System.ValueTuple 4.4 instead of 4.5

## [2.7.1] - 2021-03-04

### Fixed

- Fix CI builds

## [2.7.0] - 2021-03-04

### Added

- Add build number to AssemblyFileVersion
- Add basic non-static `Transliterator` class with transliterate functionality (tylerpayne)
- Add `Icu.Wrapper.Verbose` property to assist in diagnosing load problems
- Add OSX support for loading icu libraries

### Fixed

- Speed up `BreakIterator.GetBoundaries` (#127; atlastodor)
- Fix `SortKey.ToString`
- Fix return type of `GetCombiningClass` to match C++ API

## [2.6.0] - 2019-09-27

### Added

- Add `TimeZone` class (#108; j-troc)
- Create nuget symbol package
- Add `BiDi` class (#121; jeffska)

### Fixed

- Crash on Linux disposing `RuleBasedCollator` (#124)

## [2.5.4] - 2019-01-09

### Fixed

- Normalization of strings that failed to decompose under certain conditions (#106)
- Throw only on errors, not on errorcode that has `WARNING` in name if `throwOnWarnings == false`

## [2.5.3] - 2018-12-17

### Fixed

- remove double call of dispose when disposing `RuleBasedCollator`
- Fix `BreakIterator.SetText` if break iterator hasn't been initialized before (emrobinson)
- Fix random `AccessViolationException` in break iterator (#81) (emrobinson)

## [2.5.2] - 2018-12-10

### Changed

- `AssemblyVersion` only changes when major or minor version number changes (instead
  of `AssemblyFileVersion` accidentally introduced in previous patch version). This is
  necessary so that the assembly signature doesn't change and icu.net.dll referenced in
  a project can be replaced with a bugfix version without requiring to change the
  binding redirect.

### Fixed

- Ignore exceptions that might occur when releasing `SafeRuleBasedCollatorHandle` (but
  generate a ReleaseHandleFailed Managed Debugging Assistant).

## [2.5.1] - 2018-11-28

### Changed

- `AssemblyFileVersion` only changes when major or minor version number changes.

### Fixed

- Set ErrorCode to `ErrorCode.NoErrors` before calling native methods. This fixes some
  strange and hard-to-debug errors.

## [2.5.0] - 2018-11-26

### Added

- now supports case folding tokenizer (#88)
- additional Character methods: CharDirection, GetIntPropertyValue, ToLower, ToTitle, ToUpper,
  IsLetter, IsMark, IsSeparator
- partially implemented Normalizer2 class
- partially implemented ResourceBundle class
- partially implemented CodepageConversion class
- partially implemented MessageFormatter and Transliterator classes
- add BreakIterator.GetEnumerator() method and BreakEnumerator class to allow
  enumerating over word segments as described in the ICU user guide (the
  existing method BreakIterator.Split ignores spaces and punctuation)
- Wrapper.MinSupportedIcuVersion and Wrapper.MaxSupportedIcuVersion constants

### Changed

- output error on Linux if unmanaged libraries can't be loaded
- allow to confine version number after initialization. In this case we internally
  do a reset and re-initialize with the new version number.

### Fixed

- icu.net.dll for netstandard1.6 now has the correct version number (#72)

## [2.4.0] - 2018-10-24

### Known bug

- icu.net.dll for netstandard1.6 has the wrong version number (always 1.0.0) (#72)

### Fixed

- Fix crash if filename contains minor version number, e.g. `libicuuc.so.60.1`

### Changed

- Update UProperty to match ICU 62

### Added

- Support for Tizen (Tomasz Zalewski; issue #82)

## [2.3.4] - 2018-08-27

### Fixed

- Change PlatformTarget to AnyCPU (issue #70). The wrong x86 target sneaked in
  with the changes for version 2.3.3.

## [2.3.3] - 2018-07-03

### Changed

- Allow ICU up to version 70

## [2.3.2] - 2018-03-14

### Changed

- Update dependency information in nuget package.

## [2.3.1] - 2018-03-14

### Changed

- Remove dependency on `System.Runtime.InteropServices.RuntimeInformation` for 4.6.1
  assembly. It has problems when running under Mono 4.

## [2.3.0] - 2018-02-28

### Added

- Add Wrapper.Init() method to allow initialization of ICU for
  multi-threaded applications (#54)
- Implement `BreakIterator.Clone()` method (#56) to allow break iterator to be
  used in multi-threaded applications

### Changed

- Create netstandard package (#37/#59, conniey). This allows to use the package with .NET Core
  as well as any other .NET version compatible with .NET Standard 1.6. Additionally we still
  include the binaries for .NET 4.0 and .NET 4.5.1.
- Improved warning message if ICU is not initialized.
- Enhanced readme.

### Fixed

- Fix signature of u_charType (#54)
- Don't depend on libc6-dev package (#62)

## [2.2.0] - 2017-09-29

### Fixed

- fix buffer overflow in Normalize() (#47)

### Changed

- Assembly marked as CLSCompliant (#33)
- additionally look in lib/x86 and lib/x64 as well as lib/win-\*
  and lib/linux-\* for ICU binaries (#51)
- Add minimal support of regular expressions (#32, MURATA Makoto)

## [2.1.0] - 2017-03-17

### Fixed

- implement `IDisposable` in collators

### Changed

- Implement `RuleBasedBreakIterator` class (Connie Yau)
- Make `BreakIterator` closer to `Icu::BreakIterator` (Connie Yau)
- Enable and fix XML documentation (MURATA Makoto, Connie Yau)
- support 64-bit ICU4C (#14 and #30). The unmanaged binaries can either be
  directly in the output directory next to `icu.net.dll`, or in a `x64`
  subdirectory (the 32-bit binaries in a `x86` subdirectory).

## [2.0.1] - 2016-12-19

### Fixed

- Prefer local directory when loading unmanaged ICU binaries. This addresses
  [#20](https://github.com/sillsdev/icu-dotnet/issues/20).
- Fix `CollationStrength.Identical` value to match value used by unmanaged
  binaries.
- Fix casing of a few native methods.

### Changed

- Call native cleanup from Wrapper.Cleanup
- Reset ICU version and method pointers on cleanup.

### Removed

- Removed obsolete debian packaging files

## [2.0.0] - 2016-12-08

### Changed

- Dynamically load ICU binaries, thus allowing to work with any ICU version
- Cross-platform nuget package that is known to work on Windows and Linux
- ICU binaries moved to separate nuget packages (`Icu4C.Win.*`)
- Change versioning scheme. Previously the versions for the nuget package included
  the ICU version. Now we follow [Semantic Versioning](https://semver.org/).

[Unreleased]: https://github.com/sillsdev/icu-dotnet/compare/v3.0.1...HEAD
[3.0.1]: https://github.com/sillsdev/icu-dotnet/compare/v3.0.0...v3.0.1
[3.0.0]: https://github.com/sillsdev/icu-dotnet/compare/v2.10.0...v3.0.0
[2.10.0]: https://github.com/sillsdev/icu-dotnet/compare/v2.9.0...v2.10.0
[2.9.0]: https://github.com/sillsdev/icu-dotnet/compare/v2.8.1...v2.9.0
[2.8.1]: https://github.com/sillsdev/icu-dotnet/compare/v2.8.0...v2.8.1
[2.8.0]: https://github.com/sillsdev/icu-dotnet/compare/v2.7.1...v2.8.0
[2.7.1]: https://github.com/sillsdev/icu-dotnet/compare/v2.7.0...v2.7.1
[2.7.0]: https://github.com/sillsdev/icu-dotnet/compare/v2.6.0...v2.7.0
[2.6.0]: https://github.com/sillsdev/icu-dotnet/compare/v2.5.4...v2.6.0
[2.5.4]: https://github.com/sillsdev/icu-dotnet/compare/v2.5.3...v2.5.4
[2.5.3]: https://github.com/sillsdev/icu-dotnet/compare/v2.5.2...v2.5.3
[2.5.2]: https://github.com/sillsdev/icu-dotnet/compare/v2.5.1...v2.5.2
[2.5.1]: https://github.com/sillsdev/icu-dotnet/compare/v2.5.0...v2.5.1
[2.5.0]: https://github.com/sillsdev/icu-dotnet/compare/v2.4.0...v2.5.0
[2.4.0]: https://github.com/sillsdev/icu-dotnet/compare/v2.3.4...v2.4.0
[2.3.4]: https://github.com/sillsdev/icu-dotnet/compare/v2.3.3...v2.3.4
[2.3.3]: https://github.com/sillsdev/icu-dotnet/compare/v2.3.2...v2.3.3
[2.3.2]: https://github.com/sillsdev/icu-dotnet/compare/v2.3.1...v2.3.2
[2.3.1]: https://github.com/sillsdev/icu-dotnet/compare/v2.3.0...v2.3.1
[2.3.0]: https://github.com/sillsdev/icu-dotnet/compare/v2.2.0...v2.3.0
[2.2.0]: https://github.com/sillsdev/icu-dotnet/compare/v2.1.0...v2.2.0
[2.1.0]: https://github.com/sillsdev/icu-dotnet/compare/v2.0.1...v2.1.0
[2.0.1]: https://github.com/sillsdev/icu-dotnet/compare/v2.0.0...v2.0.1
[2.0.0]: https://github.com/sillsdev/icu-dotnet/compare/40ff102..v2.0.0
