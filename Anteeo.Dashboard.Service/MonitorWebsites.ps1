Function GetAllAppPoolProcesses() {    
    $result = (C:\Windows\System32\inetsrv\appcmd.exe list wps)
    if (![string]::IsNullOrEmpty($result)) {
        $regex = ("WP\s\""(?'processid'\d+)\""\s\(applicationPool:(?'apppool'\w+)\)")
        foreach ($line in $result.Split("`n")) {
            $line -match $regex | Out-Null
            New-Object -TypeName psobject -Property @{
                ProcessId = [convert]::ToInt32($matches["processid"])
                ApplicationPool = $matches["apppool"]
            }
        }
    }     
}

Function GetCpuUsageForProcess([int]$processId) {    
    $processName = (Get-Process -Id $processId).Name
    $cpuCores = (Get-WMIObject Win32_ComputerSystem).NumberOfLogicalProcessors
    $result = (Get-Counter “\Process($processname*)\% Processor Time”).CounterSamples | `
        Select InstanceName, @{Name=”CPUPercent”;Expression={[Decimal]::Round(($_.CookedValue / $cpuCores), 2)}} | `
        Select -First 1
    return $result.CPUPercent
}

Function MonitorWebsites($config) {
    $processes = GetAllAppPoolProcesses
    foreach ($environment in $config.environments) {
        foreach ($source in $environment.sources) {
            $process = $processes | ? { $_.ApplicationPool -eq $source.ApplicationPool } | select -First 1
            if ($process -ne $null) {
                $obj = New-Object -TypeName psobject -Property @{
                    Name = $source.Name
                    CpuPercent = GetCpuUsageForProcess($process.ProcessId)
                }
                Write-Host @obj
            }
        }
    }
}

$config = Get-Content -Raw -Path "config.json" | ConvertFrom-Json

<#while ($true) {
    MonitorWebsites($config)
    Start-Sleep -Milliseconds $config.performanceInterval
}#>