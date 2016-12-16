# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).

## [Unreleased]

### Fixed
- Prefer local directory when loading unmanaged ICU binaries. This addresses
  [#20](https://github.com/sillsdev/icu-dotnet/issues/20).
- Fix `CollationStrength.Identical` value to match value used by unamanaged
  binaries.
- Fix casing of a few native methods.
- Fix loading of native binaries from path with a space in the name.

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
