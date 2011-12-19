#!/bin/bash
xbuild /target:Test /p:icu_ver=42 /p:Configuration=DebugMono /property:teamcity_build_checkoutDir=/home/daniel/Repos/Palaso/icu-dotnet  /property:teamcity_dotnet_nunitlauncher_msbuild_task="notthere" /property:BUILD_NUMBER="4.2.1.abcd" /property:Minor="1" build.mono.proj
