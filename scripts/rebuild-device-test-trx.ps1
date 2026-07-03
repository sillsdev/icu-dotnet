#Requires -Version 5.1
<#
.SYNOPSIS
    Rebuild a valid TRX file from DeviceRunners tcp-test-events.jsonl.

.DESCRIPTION
    DeviceRunners 0.1.0-preview.12 can abort TRX generation when test failure output
    contains XML-invalid control characters (e.g. UTF-16 debug text with NUL/ENQ).
    The JSONL event log is always well-formed, so we regenerate TRX for dotnet test.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $ResultsDir,

    [Parameter(Mandatory)]
    [string] $TrxFile
)

$ErrorActionPreference = 'Stop'

function Test-ValidXmlChar([int]$codePoint) {
    return ($codePoint -eq 0x9) -or ($codePoint -eq 0xA) -or ($codePoint -eq 0xD) -or
        (($codePoint -ge 0x20) -and ($codePoint -le 0xD7FF)) -or
        (($codePoint -ge 0xE000) -and ($codePoint -le 0xFFFD))
}

function Get-SanitizedXmlText([string]$text) {
    if ([string]::IsNullOrEmpty($text)) {
        return ''
    }

    $sb = New-Object System.Text.StringBuilder
    for ($i = 0; $i -lt $text.Length; $i++) {
        $ch = $text[$i]
        if ([char]::IsHighSurrogate($ch) -and ($i + 1) -lt $text.Length) {
            $low = $text[$i + 1]
            if ([char]::IsLowSurrogate($low)) {
                $codePoint = [char]::ConvertToUtf32($ch, $low)
                if (Test-ValidXmlChar $codePoint) {
                    [void]$sb.Append($ch)
                    [void]$sb.Append($low)
                }
                $i++
                continue
            }
        }

        if (Test-ValidXmlChar ([int][char]$ch)) {
            [void]$sb.Append($ch)
        }
    }

    return $sb.ToString()
}

function Get-TrxOutcome([string]$status) {
    switch ($status) {
        'Passed' { 'Passed' }
        'Failed' { 'Failed' }
        'Skipped' { 'NotExecuted' }
        'NotExecuted' { 'NotExecuted' }
        default { 'NotExecuted' }
    }
}

$eventsFile = Join-Path $ResultsDir 'tcp-test-events.jsonl'
if (-not (Test-Path $eventsFile)) {
    exit 0
}

$begin = $null
$end = $null
$results = New-Object System.Collections.Generic.List[object]

Get-Content -LiteralPath $eventsFile | ForEach-Object {
    if ([string]::IsNullOrWhiteSpace($_)) {
        return
    }

    $event = $_ | ConvertFrom-Json
    switch ($event.type) {
        'begin' { $begin = [datetimeoffset]::Parse($event.timestamp) }
        'end' { $end = [datetimeoffset]::Parse($event.timestamp) }
        'result' { $results.Add($event) }
    }
}

if ($results.Count -eq 0) {
    Write-Warning "No test results found in $eventsFile"
    exit 0
}

if (-not $begin) {
    $begin = [datetimeoffset]::Parse($results[0].timestamp)
}
if (-not $end) {
    $end = [datetimeoffset]::Parse($results[-1].timestamp)
}

$passed = @($results | Where-Object { $_.status -eq 'Passed' }).Count
$failed = @($results | Where-Object { $_.status -eq 'Failed' }).Count
$skipped = @($results | Where-Object { $_.status -in @('Skipped', 'NotExecuted') }).Count
$total = $results.Count
$executed = $passed + $failed

$trxDir = Split-Path -Parent $TrxFile
if ($trxDir) {
    New-Item -ItemType Directory -Force -Path $trxDir | Out-Null
}

$settings = New-Object System.Xml.XmlWriterSettings
$settings.Indent = $true
$settings.Encoding = New-Object System.Text.UTF8Encoding($false)
$settings.OmitXmlDeclaration = $false

$writer = [System.Xml.XmlWriter]::Create($TrxFile, $settings)
try {
    $ns = 'http://microsoft.com/schemas/VisualStudio/TeamTest/2010'
    $writer.WriteStartDocument()
    $writer.WriteStartElement('TestRun', $ns)
    $writer.WriteAttributeString('id', [guid]::NewGuid().ToString())
    $writer.WriteAttributeString('name', '')
    $writer.WriteAttributeString('runUser', '')

    $timeFormat = 'yyyy-MM-ddTHH:mm:ss.fffffffK'
    $writer.WriteStartElement('Times', $ns)
    $writer.WriteAttributeString('creation', $begin.ToString($timeFormat))
    $writer.WriteAttributeString('queuing', $begin.ToString($timeFormat))
    $writer.WriteAttributeString('start', $begin.ToString($timeFormat))
    $writer.WriteAttributeString('finish', $end.ToString($timeFormat))
    $writer.WriteEndElement()

    $writer.WriteStartElement('Results', $ns)
    foreach ($result in $results) {
        $outcome = Get-TrxOutcome $result.status
        $executionId = [guid]::NewGuid().ToString()
        $testId = [guid]::NewGuid().ToString()
        $startTime = [datetimeoffset]::Parse($result.timestamp)
        $duration = [timespan]::Zero
        if ($result.duration) {
            [void][timespan]::TryParse($result.duration, [ref]$duration)
        }
        $endTime = $startTime.Add($duration)

        $writer.WriteStartElement('UnitTestResult', $ns)
        $writer.WriteAttributeString('executionId', $executionId)
        $writer.WriteAttributeString('testId', $testId)
        $writer.WriteAttributeString('testName', (Get-SanitizedXmlText $result.displayName))
        $writer.WriteAttributeString('outcome', $outcome)
        $writer.WriteAttributeString('duration', $result.duration)
        $writer.WriteAttributeString('startTime', $startTime.ToString($timeFormat))
        $writer.WriteAttributeString('endTime', $endTime.ToString($timeFormat))
        $writer.WriteAttributeString('computerName', '')

        $writer.WriteStartElement('Output', $ns)
        if ($result.output) {
            $writer.WriteStartElement('StdOut', $ns)
            $writer.WriteString((Get-SanitizedXmlText $result.output))
            $writer.WriteEndElement()
        }

        if ($result.errorMessage -or $result.errorStackTrace) {
            $writer.WriteStartElement('ErrorInfo', $ns)
            if ($result.errorMessage) {
                $writer.WriteStartElement('Message', $ns)
                $writer.WriteString((Get-SanitizedXmlText $result.errorMessage))
                $writer.WriteEndElement()
            }
            if ($result.errorStackTrace) {
                $writer.WriteStartElement('StackTrace', $ns)
                $writer.WriteString((Get-SanitizedXmlText $result.errorStackTrace))
                $writer.WriteEndElement()
            }
            $writer.WriteEndElement()
        }
        $writer.WriteEndElement()
        $writer.WriteEndElement()
    }
    $writer.WriteEndElement()

    $writer.WriteStartElement('ResultSummary', $ns)
    $writer.WriteStartElement('Counters', $ns)
    $writer.WriteAttributeString('total', $total)
    $writer.WriteAttributeString('executed', $executed)
    $writer.WriteAttributeString('passed', $passed)
    $writer.WriteAttributeString('failed', $failed)
    $writer.WriteAttributeString('error', '0')
    $writer.WriteAttributeString('timeout', '0')
    $writer.WriteAttributeString('aborted', '0')
    $writer.WriteAttributeString('inconclusive', '0')
    $writer.WriteAttributeString('passedButRunAborted', '0')
    $writer.WriteAttributeString('notRunnable', '0')
    $writer.WriteAttributeString('notExecuted', $skipped)
    $writer.WriteAttributeString('disconnected', '0')
    $writer.WriteAttributeString('warning', '0')
    $writer.WriteAttributeString('completed', $executed)
    $writer.WriteAttributeString('inProgress', '0')
    $writer.WriteAttributeString('pending', '0')
    $writer.WriteEndElement()
    $writer.WriteEndElement()

    $writer.WriteEndElement()
    $writer.WriteEndDocument()
}
finally {
    $writer.Dispose()
}

Write-Host "Rebuilt TRX from tcp-test-events.jsonl: $TrxFile (total=$total, passed=$passed, failed=$failed, skipped=$skipped)"
