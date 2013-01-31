call "c:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\vcvarsall.bat"

pushd c:\src\sil\solid\build
MSbuild /target:installer /property:teamcity_build_checkoutDir=c:\src\sil\solid  /property:teamcity_dotnet_nunitlauncher_msbuild_task="notthere" /property:BUILD_NUMBER="0.1.345.abcd" /property:Minor="1"
popd
PAUSE

#/verbosity:detailed