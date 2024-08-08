[CmdletBinding()]
Param (
    [Parameter(HelpMessage = "Path to the file to append the version elements in.")]
    [string]$Path,
    [Parameter(HelpMessage = "Previous version to calculate the next version on. Must be like '1.0.0'.")]
    [string]$PreviousVersion,
    [Parameter(HelpMessage = "Commit Message")]
    [string]$Message
)

function Get-Version
{
	param (
		[string]$PreviousVersion,
		[string]$Message
	)

	$versionNumbers = $PreviousVersion -split "\."
	$major = [int]$versionNumbers[0]
	$minor = [int]$versionNumbers[1]
	$patch = [int]$versionNumbers[2]

	if ($Env:GITHUB_REF -like "refs/pull/*/feat/*" -or $Message -like "FEAT: *") {
		$minor++
		$patch = 0
	} elseif ($Env:GITHUB_REF -like "refs/pull/*/new/*" -or $Message -like "NEW: *") {
		$major++
		$minor = 0
		$patch = 0
	} else {
		$patch++
	}

	return "$major.$minor.$patch"
}

function Get-VersionSuffix
{
	if ($Env:GITHUB_REF -like "refs/pull/*") {
		$prId = $Env:GITHUB_REF -replace "refs/pull/(\d+)/.*", '$1'
		$versionSuffix = "pr.$prId.$Env:GITHUB_RUN_NUMBER"
	} else {
		$versionSuffix = ""
	}

	return $versionSuffix
}

function Set-Version
{
	param (
		[string]$Path,
		[string]$Version,
		[string]$Suffix
	)

	$xml = [xml](Get-Content -Path $Path)
	$properties = $xml.Project.PropertyGroup

	$assemblyVersionElement = $xml.CreateElement("AssemblyVersion")
	$assemblyVersionElement.InnerText = "$Version.$Env:GITHUB_RUN_NUMBER"

	$versionElement = $xml.CreateElement("VersionPrefix")
	$versionElement.InnerText = $Version

	$fileVersionElement = $xml.CreateElement("FileVersion")
	$fileVersionElement.InnerText = "$Version.$Env:GITHUB_SHA"

	if (![string]::IsNullOrEmpty($Suffix)) {
		$suffixElement = $xml.CreateElement("VersionSuffix")
		$suffixElement.InnerText = $Suffix
		[void]$properties.AppendChild($suffixElement)
	}

	[void]$properties.AppendChild($assemblyVersionElement)
	[void]$properties.AppendChild($versionElement)
	[void]$properties.AppendChild($fileVersionElement)

	[void]$xml.Save($Path)
}

$newVersion = Get-Version $PreviousVersion $Message
$suffix = Get-VersionSuffix
Set-Version $Path $newVersion $suffix

"# :arrow_up_small: Version" >> $Env:GITHUB_STEP_SUMMARY
$newVersion >> $Env:GITHUB_STEP_SUMMARY

return "newVersion=$newVersion"