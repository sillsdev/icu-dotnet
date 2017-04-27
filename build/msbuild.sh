#!/usr/bin/env bash

# .NET Core CLI does not currently support building against full .NET Framework
# on Linux.  Using this workaround https://github.com/dotnet/sdk/issues/335#issuecomment-291487227
# until the issue is solved.
if [[ -z "$DOTNETSDK" ]] ; then
    echo "DOTNETSDK variable needs to be set to location of .NET Core SDK (ie. /usr/share/dotnet/sdk/1.0.3)"
    exit 1
fi

echo $DOTNETSDK

export MSBuildExtensionsPath=$DOTNETSDK/
export CscToolExe=$DOTNETSDK/Roslyn/RunCsc.sh
export MSBuildSDKsPath=$DOTNETSDK/Sdks

msbuild "$@"