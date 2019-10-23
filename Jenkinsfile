#!groovy
// Copyright (c) 2017-2018 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

@Library('lsdev-pipeline-library') _

echo 'Testing...'
echo "buildCauses=${currentBuild.buildCauses}"
echo "GitHubPushCause=${currentBuild.getBuildCauses('java.com.cloudbees.jenkins.GitHubPushCause')}"
echo "hudson.model.Cause.UserIdCause=${currentBuild.getBuildCauses('hudson.model.Cause$UserIdCause')}"
userCause=currentBuild.getBuildCauses('hudson.model.Cause$UserIdCause')
if (userCause) {
	echo "userId=${userCause.userId}"
} else {
	echo "no userCause"
}
build_parent = currentBuild.rawBuild.parent
echo "build_parent=${build_parent}"
return

xplatformBuildAndRunTests {
	winNodeSpec = 'windows && supported && netcore && vs2017'
	winTool = 'msbuild15'
	linuxNodeSpec = 'linux64 && !packager && ubuntu && mono5 && netcore'
	linuxTool = 'mono-msbuild15'
	framework = 'netcore2.1'
	configuration = 'Release'
	uploadNuGet = true
}
