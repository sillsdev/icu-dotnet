# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).

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

- In Character class, added all enums from Unicode's uchar.h that were missing:
  UBidiPairedBracketType, UBlockCode, UEastAsianWidth, UPropertyNameChoice, UJoiningType,
  UJoiningGroup, UGraphemeClusterBreak, UWordBreakValues, USentenceBreak, ULineBreak,
  UHangulSyllableType, UIndicPositionalCategory, UIndicSyllabicCategory, UIndicConjunctBreak,
  UVerticalOrientation, UIdentifierStatus, UIdentifierType.

### Deprecated

- In Character class, added \[Obsolete\] attribute to enum members UDecompositionType.COUNT and
  UNumericType.COUNT.

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
- additionally look in lib/x86 and lib/x64 as well as lib/win-*
  and lib/linux-* for ICU binaries (#51)
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
  the ICU version. Now we follow [Semantic Versioning](http://semver.org/).

[Unreleased]: https://github.com/sillsdev/icu-dotnet/compare/v2.10.0...master

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
