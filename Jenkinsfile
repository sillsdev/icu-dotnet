#!groovy
// Copyright (c) 2017-2018 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

@Library('lsdev-pipeline-library') _

xplatformBuildAndRunTests {
	winNodeSpec = 'windows && supported && netcore && vs2017'
	winTool = 'msbuild15'
	linuxNodeSpec = 'linux64 && !packager && ubuntu && mono5 && netcore'
	linuxTool = 'mono-msbuild15'
	framework = 'netcore3.1'
	configuration = 'Release'
	uploadNuGet = true
}
