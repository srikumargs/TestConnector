# example of using management APIs to add the first tenant in the tenantList. 
# Assumes that the connection is to the mock plugin.    
    
    $dllsToLoad = @("\Sage.Connector.Management.dll", 
                    "\Sage.Connector.Data.dll",
                    "\Sage.Connector.Common.dll",
                    "\Sage.Connector.Utilities.dll"
                    "\Sage.Connector.ConfigurationService.Proxy.dll",
                    "\Sage.Connector.ConfigurationService.Interfaces.dll",
                    "\Sage.Connector.Logging.dll",
                    "\Sage.Connector.StateService.Proxy.dll",
                    "\Sage.Connector.StateService.Interfaces.dll",
                    "\Newtonsoft.Json.dll")
    
    $loc = Get-Location
    foreach($dll in $dllsToLoad)
    {
        $fullDllName = $loc.Path + $dll
        [Reflection.Assembly]::LoadFile($fullDllName) | out-null
    }
    

    #[Sage.Connector.Management.ConfigurationHelpers]::InstallService()

    #$o = [Sage.Connector.Management.DeveloperFlags]::ShowEndPointAddress()
    
    
    $pcr = [Sage.Connector.Management.ConfigurationHelpers]::CreateNewTenantConfiguration()
    $pcr.SiteAddress =  [Sage.Connector.Management.ConfigurationHelpers]::GetDefaultCloudSiteUri()

    $validation = [Sage.Connector.Management.ConfigurationHelpers]::ValidateSageId("user", "password" )
    $list = [Sage.Connector.Management.ConfigurationHelpers]::GetTenantList($validation.Token, $siteAddress)
    $pcr.CloudTenantId = $list.Item(0).TenantGuid.ToString()
    #$pcr.CloudCompanyName = $list.Item(0).TenantName

    $state = new-object "Sage.Connector.Management.StateServiceWrapper"
    #plugins contains the list of plugins in BackOfficePlugins. Can more more then one.
    $plugins = $state.GetBackOfficePlugins()
    $plugin = $plugins.BackOfficePlugins
    
    #Pull valid mock credentials from thin air... Credentials gathing realy needs a UX. So just use a valid set for this workflow test
    $credentials = '{"UserId":"User","Password":"password","CompanyId":"Company","CompanyPath":"C:\\"}'
    
    #validate them though since there is data we need from the validation call.
    $validation = $state.ValidateBackOfficeConnectionCredentialsAsString($plugin.PluginId, $credentials)
    
    $state = $null

    $pcr.ConnectorPluginId = $plugin.PluginId
    $pcr.BackOfficeProductName = $plugin.BackOfficeProductName
    $pcr.BackOfficePluginProductId = $plugin.BackOfficePluginProductId

    $pcr.BackOfficeCompanyUniqueId = $validation.CompanyUniqueIndentifier
    $pcr.BackOfficeCompanyName = $validation.CompanyNameForDisplay
    
    #Fix this, should be a string but its a dictionary not sure I can use that here.
    $pcr.BackOfficeConnectionCredentials = [Sage.Connector.Management.StateServiceWrapper]::StringStringDictonaryToJsonString($validation.ConnectionCredentials)

        
    $regResult = [Sage.Connector.Management.ConfigurationHelpers]::RegisterConnection()
    $pcr.CloudPremiseKey = $regResult.TenantClaim
    $pcr.CloudCompanyUrl = $regResult.TenantUrl
    $pcr.CloudCompanyName = $regResult.TenantName

   [Sage.Connector.Management.ConfigurationHelpers]::SaveNewTenantConfiguration($pcr)




    
