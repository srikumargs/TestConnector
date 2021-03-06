
function Run-CESql($dllPath, $connectionString, $sqlString)
{
    [Reflection.Assembly]::LoadFile($cePath) | out-null
    $cn = new-object "System.Data.SqlServerCe.SqlCeConnection" $connString

    # create the command 
    $cmd = new-object "System.Data.SqlServerCe.SqlCeCommand" 
    $cmd.CommandType = [System.Data.CommandType]"Text" 
    $cmd.CommandText = $sqlCmd
    $cmd.Connection = $cn

    #get the data 
    $dt = new-object "System.Data.DataTable"

    $cn.Open() 
    $rdr = $cmd.ExecuteReader()
    $dt.Load($rdr) 
    $cn.Close()

    if ($dt.Rows.Count -eq 0)
    {
        "No log entries to display."
    }
    else 
    {
        $dt
    }
}

function Get-ConnectorLog($countLimit)
{
    $cePath = "\Microsoft SQL Server Compact Edition\v4.0\Desktop\System.Data.SqlServerCe.dll”
    if ($ENV:PROCESSOR_ARCHITECTURE -eq "x86")
    {
        $cePath = $ENV:ProgramFiles + $cePath
    }
    else
    {
        $cePath = ${ENV:ProgramFiles(x86)} + $cePath
    }

    #$connString = "Data Source=C:\dev\Sage Cloud\CloudConnector\Main\Runtime Files\Documents and Settings\All Users\Application Data\Sage\CM\HostingFramework\Sage.CloudConnector.SCA.Service.1.0\LogStore.sdf"
    #$connString = "Data Source=C:\ProgramData\Sage\CM\HostingFramework\Sage.CloudConnector.SCA.Service.1.0\LogStore.sdf"
    $connString = "Data Source=LogStore.sdf;Max Database Size=1024"

    $now = [System.DateTime]::Now
    $nowUtc = $now.ToUniversalTime()
    $timeZoneOffsetInMinutes = ($now - $nowUtc).TotalMinutes
    
    # Attach our optional limit
    $select = "SELECT "
    if ($countLimit)
    {
        $select = $select + "TOP (" + $countLimit + ") "
    }

    $sqlCmd = [string]::Format($select + 
        "t1.[Type],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, t1.[DateTimeUtc]), 109) AS [Date and Time],
        t1.[SourceTypeName] as [Source Type Name],
        t1.[SourceMemberName] as [Source Member Name],
        t1.[ProcessId] as [Process Id],
        t1.[AppDomainId] as [App Domain Id],
        t1.[ThreadId] as [Thread Id],
        t1.[ObjectId] as [Object Id],
        t1.[ActivityEntry_Id] as [Activity Id],
        t1.[CloudTenantId] AS [Tenant Id],
        t1.[CloudRequestId] AS [Request Id],
        t1.[User],
        t1.[Machine],
        t1.[Id],
        SUBSTRING(t1.[Content], 0, 3000) AS [Content]
        FROM LogEntries t1 
        ORDER BY DateTimeUtc DESC", $timeZoneOffsetInMinutes)

    Run-CESql $cePath $connString $sqlCmd 
}

function KeepConsoleOpenIfNeeded
{
    if((get-host).name -eq "ConsoleHost"){ 
        powershell.exe -noexit -nologo
    }
}

# Prompt for number of log entries
$countLimit = read-host "Enter the number of log entries to view (or blank for all entries)"

Get-ConnectorLog($countLimit) | Out-GridView

KeepConsoleOpenifNeeded