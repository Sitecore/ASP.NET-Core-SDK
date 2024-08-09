[CmdletBinding()]
Param (
    [Parameter(HelpMessage = "Path to the XML summary file of code coverage.")]
    [string]$CoverageReportPath,
	[Parameter(HelpMessage = "Path to the folder with TRX results.")]
    [string]$ResultFolderPath
)

$result = [System.Text.StringBuilder]::new()

# Test Result Summary
[void]$result.AppendLine("# :vertical_traffic_light: Test Results")
[void]$result.AppendLine("| Assembly | :arrow_forward: | :white_check_mark: | :x: | :hash: |")
[void]$result.AppendLine("|----------|-----------------|--------------------|-----|--------|")
$totalExecuted = 0
$totalPassed = 0
$totalFailed = 0
$total = 0
foreach ($trx in (Get-ChildItem $ResultFolderPath -Include *.trx)) {
	$testrun = ([xml](Get-Content ([string]$trx).Replace("[", "``[").Replace("]", "``]"))).TestRun
	$counters = $testrun.ResultSummary.Counters
	if ($counters.Total -gt 0) {
		$assembly = $testrun.TestDefinitions.ChildNodes[0].TestMethod.CodeBase.Split("/")[-1].Replace(".dll", "")
		[void]$result.Append("| $assembly ")
		[void]$result.Append("| $($counters.Executed) ")
		$totalExecuted = $totalExecuted + $counters.Executed
		[void]$result.Append("| $($counters.Passed) ")
		$totalPassed = $totalPassed + $counters.Passed
		[void]$result.Append("| $($counters.Failed) ")
		$totalFailed = $totalFailed + $counters.Failed
		[void]$result.Append("| $($counters.Total) ")
		$total = $total + $counters.Total
		[void]$result.AppendLine("|")
	}
}
[void]$result.AppendLine("| Total | $totalExecuted | $totalPassed | $totalFailed | $total |")

# Coverage Report Summary
$report = ([xml](Get-Content $CoverageReportPath)).CoverageReport
$summary = $report.Summary
[void]$result.AppendLine("# :bar_chart: Code Coverage")

[void]$result.AppendLine("## Summary")
[void]$result.AppendLine("| Lines | Branches | Methods |")
[void]$result.AppendLine("|-------|----------|---------|")
[void]$result.Append("| $($summary.Linecoverage)% ($($summary.Coveredlines) / $($summary.Coverablelines)) ")
[void]$result.Append("| $($summary.Branchcoverage)% ($($summary.Coveredbranches) / $($summary.Totalbranches)) ")
[void]$result.Append("| $($summary.Methodcoverage)% ($($summary.Coveredmethods) / $($summary.Totalmethods)) ")
[void]$result.AppendLine("|")

[void]$result.AppendLine("## Assembly Details")
[void]$result.AppendLine("| Assembly | Lines | Branches | Methods |")
[void]$result.AppendLine("|----------|-------|----------|---------|")
foreach ($assembly in $report.Coverage.ChildNodes) {
	[void]$result.Append("| $($assembly.name) ")
	[void]$result.Append("| $($assembly.coverage)% ($($assembly.coveredlines) / $($assembly.coverablelines)) ")
	[void]$result.Append("| $($assembly.branchcoverage)% ($($assembly.coveredbranches) / $($assembly.totalbranches)) ")
	[void]$result.Append("| $($assembly.methodcoverage)% ($($assembly.coveredmethods) / $($assembly.totalmethods)) ")
	[void]$result.AppendLine("|")
}

return $result.ToString()