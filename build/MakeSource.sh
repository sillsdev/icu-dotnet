#!/bin/bash
xbuild /target:SourcePackage /p:icu_ver=42 /p:Configuration=DebugMono /property:teamcity_build_checkoutDir=`pwd`/.. /property:BUILD_NUMBER="0.1.abcd" /property:BuildCounter=1 build.mono.proj
