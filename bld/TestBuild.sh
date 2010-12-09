#!/bin/bash
xbuild /target:Build /property:teamcity_build_checkoutDir=/media/mono/icu-dotnet  /property:teamcity_dotnet_nunitlauncher_msbuild_task="notthere" /property:BUILD_NUMBER="4.2.1.abcd" /property:Minor="1"
