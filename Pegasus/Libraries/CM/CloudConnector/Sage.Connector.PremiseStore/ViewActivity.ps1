
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
        "No activity to display."
    }
    else 
    {
        $dt
    }
}

function Get-ConnectorActivity($tenantId)
{
    # Derive the path to the SqlServerCe installation
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

    # Attach our optional filter to our order by
    $whereOrderBy = "ORDER BY [DateTimeUtc] DESC"
    if ($tenantId)
    {
        $whereOrderBy = "WHERE [CloudTenantId] = '" + $tenantId + "' " + $whereOrderBy
    }

    $sqlCmd = [string]::Format("SELECT 
        CASE [Status]
            WHEN 0 THEN 'Not Available'
            WHEN 1 THEN 'In Progress'
            WHEN 2 THEN 'In Progress - Bindable work processing'
            WHEN 3 THEN 'In Progress - Back Office Processing'
            WHEN 4 THEN 'Completed Successfully'
            WHEN 5 THEN 'Completed with Errors'
            ELSE 'Not Available'
        END AS [Activity Status],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [DateTimeUtc]), 109) AS [Activity Started],
        [Id] AS [Activity Id],
        [CloudTenantId] AS [Tenant Id], 
        [CloudRequestId] AS [Request Id], 
        [CloudRequestRetryCount] AS [Retry Count],
        [CloudRequestType] AS [Request Type],
        [CloudRequestInnerType] AS [Inner Request Type],
        -- [CloudRequestSummary] AS [Request Summary],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [CloudRequestCreatedTimestampUtc]), 109) AS [Cloud Request Created],
        -- [CloudProjectName] AS [Project Name],
        -- [CloudRequestRequestingUser] AS [Requesting User],
        [IsSystemRequest] as [IsSystemRequest],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State1DateTimeUtc]), 109) AS [(1) Request Received From Cloud],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State2DateTimeUtc]), 109) AS [(2) Enqueue Tenant Inbox Request],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State3DateTimeUtc]), 109) AS [(3) DequeueTenantInboxRequest],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State4DateTimeUtc]), 109) AS [(4) EnqueueTenantBinderRequest],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State5DateTimeUtc]), 109) AS [(5) DequeueTenantBinderRequest],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State6DateTimeUtc]), 109) AS [(6) InvokingBindableWork],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State7DateTimeUtc]), 109) AS [(7) InvokingProcessExecution],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State8DateTimeUtc]), 109) AS [(8) InvokingDomainMediation],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State9DateTimeUtc]), 109) AS [(9) InvokingMediationBoundeWork],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State10DateTimeUtc]), 109) AS [(10) MediationBoundWorkComplete],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State11DateTimeUtc]), 109) AS [(11) DomainMediationComplete],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State12DateTimeUtc]), 109) AS [(12) ProcessExecutionComplete],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State13DateTimeUtc]), 109) AS [(13) BindableWorkComplete],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State14DateTimeUtc]), 109) AS [(14) EnqueueTenantOutboxResponse],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State15DateTimeUtc]), 109) AS [(15) DequeueTenantOutboxResponse],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State16DateTimeUtc]), 109) AS [(16) UploadsCompletedAndResponseFinalized],
        CONVERT(nvarchar(30), DATEADD(minute, {0}, [State17DateTimeUtc]), 109) AS [(17) ResponseSentToCloud],
        DATEDIFF(second, [State9DateTimeUtc], [State10DateTimeUtc]) AS [Total Back Office Time Processing (Seconds)],
        --DATEDIFF(second, [State8DateTimeUtc], [State11DateTimeUtc]) AS [Total Domain Mediation Processing Time (Seconds)],
        --DATEDIFF(second, [State7DateTimeUtc], [State12DateTimeUtc]) AS [Total Process Execution Processing Time (Seconds)],
        DATEDIFF(second, [State6DateTimeUtc], [State13DateTimeUtc]) AS [Total Bindable Work Processing Time (Seconds)],
        DATEDIFF(second, [State1DateTimeUtc], [State17DateTimeUtc]) AS [Total Processing Time (Seconds)]
        FROM ActivityEntries " + $whereOrderBy, $timeZoneOffsetInMinutes)

    Run-CESql $cePath $connString $sqlCmd 
}

function KeepConsoleOpenIfNeeded
{
    if((get-host).name -eq "ConsoleHost"){ 
        powershell.exe -noexit -nologo
    }
}

# Prompt for a tenant identifier filter
$tenantId = read-host "Enter a tenant identifier (or blank for all tenants)"

Get-ConnectorActivity($tenantId) | Out-GridView

KeepConsoleOpenifNeeded