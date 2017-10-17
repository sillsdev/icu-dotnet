# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).

[comment]: <> (Available types of changes:
### Added
### Changed
### Fixed
### Deprecated
### Removed
### Security
)

## [Unreleased]

### Fixed

- Fix signature of u_charType (#54)

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
