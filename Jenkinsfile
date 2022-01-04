#!groovy
// Copyright (c) 2017-2022 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

@Library('lsdev-pipeline-library') _

xplatformBuildAndRunTests {
	winNodeSpec = 'windows && supported && netcore3.1 && vs2019'
	winTool = 'msbuild16'
	linuxNodeSpec = 'linux64 && !packager && ubuntu && mono6 && netcore3.1'
	linuxTool = 'mono-msbuild16'
	framework = 'netcore3.1'
	configuration = 'Release'
	uploadNuGet = true
	nupkgPath = "output/*nupkg"
}
