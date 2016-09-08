param($installPath, $toolsPath, $package, $project)

$icuRegex = "icu(dt|in|uc)[0-9]+\.dll"

foreach ($dll in $project.ProjectItems) {

	if ($dll.Name -notmatch $icuRegex) {
		continue
	}

	# Setting BuildAction = None
	# Reference: http://stackoverflow.com/a/7427431/4220757
	$buildAction = $dll.Properties.Item("BuildAction")
	$buildAction.Value = 0

	# Set CopyToOutputDirectory to "Copy if Newer"
	# Reference: http://stackoverflow.com/questions/23503145/why-does-the-verbiage-for-the-copy-to-output-directory-selection-change-betwee/23511515#23511515
	$copyToOutput = $dll.Properties.Item("CopyToOutputDirectory")
	$copyToOutput.Value = 2
}