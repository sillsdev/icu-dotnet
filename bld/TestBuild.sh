#!/bin/bash
xbuild /target:Build /p:icu_ver=42 /p:Configuration=DebugMono /property:teamcity_build_checkoutDir=`pwd`/..  /property:teamcity_dotnet_nunitlauncher_msbuild_task="notthere" /property:BUILD_NUMBER="0.1.abcd" /property:Minor="1" build.mono.proj
