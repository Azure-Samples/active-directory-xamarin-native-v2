 
[CmdletBinding()]
param(
    [Parameter(Mandatory=$False, HelpMessage='Tenant ID (This is a GUID which represents the "Directory ID" of the AzureAD tenant into which you want to create the apps')]
    [string] $tenantId,
    [Parameter(Mandatory=$False, HelpMessage='Azure environment to use while running the script. Default = Global')]
    [string] $azureEnvironmentName
)

<#
 This script creates the Azure AD applications needed for this sample and updates the configuration files
 for the visual Studio projects from the data in the Azure AD applications.

 In case you don't have Microsoft.Graph.Applications already installed, the script will automatically install it for the current user
 
 There are four ways to run this script. For more information, read the AppCreationScripts.md file in the same folder as this script.
#>

# Adds the requiredAccesses (expressed as a pipe separated string) to the requiredAccess structure
# The exposed permissions are in the $exposedPermissions collection, and the type of permission (Scope | Role) is 
# described in $permissionType
Function AddResourcePermission($requiredAccess, `
                               $exposedPermissions, [string]$requiredAccesses, [string]$permissionType)
{
    foreach($permission in $requiredAccesses.Trim().Split("|"))
    {
        foreach($exposedPermission in $exposedPermissions)
        {
            if ($exposedPermission.Value -eq $permission)
                {
                $resourceAccess = New-Object Microsoft.Graph.PowerShell.Models.MicrosoftGraphResourceAccess
                $resourceAccess.Type = $permissionType # Scope = Delegated permissions | Role = Application permissions
                $resourceAccess.Id = $exposedPermission.Id # Read directory data
                $requiredAccess.ResourceAccess += $resourceAccess
                }
        }
    }
}

#
# Example: GetRequiredPermissions "Microsoft Graph"  "Graph.Read|User.Read"
# See also: http://stackoverflow.com/questions/42164581/how-to-configure-a-new-azure-ad-application-through-powershell
Function GetRequiredPermissions([string] $applicationDisplayName, [string] $requiredDelegatedPermissions, [string]$requiredApplicationPermissions, $servicePrincipal)
{
    # If we are passed the service principal we use it directly, otherwise we find it from the display name (which might not be unique)
    if ($servicePrincipal)
    {
        $sp = $servicePrincipal
    }
    else
    {
        $sp = Get-MgServicePrincipal -Filter "DisplayName eq '$applicationDisplayName'"
    }
    $appid = $sp.AppId
    $requiredAccess = New-Object Microsoft.Graph.PowerShell.Models.MicrosoftGraphRequiredResourceAccess
    $requiredAccess.ResourceAppId = $appid 
    $requiredAccess.ResourceAccess = New-Object System.Collections.Generic.List[Microsoft.Graph.PowerShell.Models.MicrosoftGraphResourceAccess]

    # $sp.Oauth2Permissions | Select Id,AdminConsentDisplayName,Value: To see the list of all the Delegated permissions for the application:
    if ($requiredDelegatedPermissions)
    {
        AddResourcePermission $requiredAccess -exposedPermissions $sp.Oauth2PermissionScopes -requiredAccesses $requiredDelegatedPermissions -permissionType "Scope"
    }
    
    # $sp.AppRoles | Select Id,AdminConsentDisplayName,Value: To see the list of all the Application permissions for the application
    if ($requiredApplicationPermissions)
    {
        AddResourcePermission $requiredAccess -exposedPermissions $sp.AppRoles -requiredAccesses $requiredApplicationPermissions -permissionType "Role"
    }
    return $requiredAccess
}


Function ReplaceInLine([string] $line, [string] $key, [string] $value)
{
    $index = $line.IndexOf($key)
    if ($index -ige 0)
    {
        $index2 = $index+$key.Length
        $line = $line.Substring(0, $index) + $value + $line.Substring($index2)
    }
    return $line
}

Function ReplaceInTextFile([string] $configFilePath, [System.Collections.HashTable] $dictionary)
{
    $lines = Get-Content $configFilePath
    $index = 0
    while($index -lt $lines.Length)
    {
        $line = $lines[$index]
        foreach($key in $dictionary.Keys)
        {
            if ($line.Contains($key))
            {
                $lines[$index] = ReplaceInLine $line $key $dictionary[$key]
            }
        }
        $index++
    }

    Set-Content -Path $configFilePath -Value $lines -Force
}

Function ConfigureApplications
{
    $isOpenSSl = 'N' #temporary disable open certificate creation 

    <#.Description
       This function creates the Azure AD applications for the sample in the provided Azure AD tenant and updates the
       configuration files in the client and service project  of the visual studio solution (App.Config and Web.Config)
       so that they are consistent with the Applications parameters
    #> 
    
    if (!$azureEnvironmentName)
    {
        $azureEnvironmentName = "Global"
    }

    # Connect to the Microsoft Graph API, non-interactive is not supported for the moment (Oct 2021)
    Write-Host "Connecting to Microsoft Graph"
    if ($tenantId -eq "") {
        Connect-MgGraph -Scopes "Application.ReadWrite.All" -Environment $azureEnvironmentName
        $tenantId = (Get-MgContext).TenantId
    }
    else {
        Connect-MgGraph -TenantId $tenantId -Scopes "Application.ReadWrite.All" -Environment $azureEnvironmentName
    }
    

   # Create the client AAD application
   Write-Host "Creating the AAD application (active-directory-maui-v2)"
   
   # create the application 
   $clientAadApplication = New-MgApplication -DisplayName "active-directory-maui-v2" `
                                             -PublicClient @{ `
                                                   RedirectUris="https://login.microsoftonline.com/common/oauth2/nativeclient" `
                                             } `
                                             -SignInAudience AzureADMultipleOrgs `
                                                      #end of command
    # create the service principal of the newly created application 
    $currentAppId = $clientAadApplication.AppId
    $clientServicePrincipal = New-MgServicePrincipal -AppId $currentAppId -Tags {WindowsAzureActiveDirectoryIntegratedApp}

    # add the user running the script as an app owner if needed
    $owner = Get-MgApplicationOwner -ApplicationId $clientAadApplication.Id
    if ($owner -eq $null)
    { 
        New-MgApplicationOwnerByRef -ApplicationId $clientAadApplication.Id  -BodyParameter = @{"@odata.id" = "htps://graph.microsoft.com/v1.0/directoryObjects/$user.ObjectId"}
        Write-Host "'$($user.UserPrincipalName)' added as an application owner to app '$($clientServicePrincipal.DisplayName)'"
    }
    Write-Host "Done creating the client application (active-directory-maui-v2)"

    # URL of the AAD application in the Azure portal
    # Future? $clientPortalUrl = "https://portal.azure.com/#@"+$tenantName+"/blade/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/Overview/appId/"+$clientAadApplication.AppId+"/objectId/"+$clientAadApplication.Id+"/isMSAApp/"
    $clientPortalUrl = "https://portal.azure.com/#blade/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/CallAnAPI/appId/"+$clientAadApplication.AppId+"/objectId/"+$clientAadApplication.Id+"/isMSAApp/"
    Add-Content -Value "<tr><td>client</td><td>$currentAppId</td><td><a href='$clientPortalUrl'>active-directory-maui-v2</a></td></tr>" -Path createdApps.html
    $requiredResourcesAccess = New-Object System.Collections.Generic.List[Microsoft.Graph.PowerShell.Models.MicrosoftGraphRequiredResourceAccess]
    
    # Add Required Resources Access (from 'client' to 'Microsoft Graph')
    Write-Host "Getting access from 'client' to 'Microsoft Graph'"
    $requiredPermissions = GetRequiredPermissions -applicationDisplayName "Microsoft Graph" `
        -requiredDelegatedPermissions "User.Read" `
    $requiredResourcesAccess.Add($requiredPermissions)
    Update-MgApplication -ApplicationId $clientAadApplication.Id -RequiredResourceAccess $requiredResourcesAccess
    Write-Host "Granted permissions."

    Write-Host "Successfully registered and configured that app registration for 'active-directory-maui-v2' at" -ForegroundColor Green
    $clientPortalUrl
Function UpdateLine([string] $line, [string] $value)
{
    $index = $line.IndexOf(':')
    $lineEnd = ''

    if($line[$line.Length - 1] -eq ','){   $lineEnd = ',' }
    
    if ($index -ige 0)
    {
        $line = $line.Substring(0, $index+1) + " " + '"' + $value+ '"' + $lineEnd
    }
    return $line
}

Function UpdateTextFile([string] $configFilePath, [System.Collections.HashTable] $dictionary)
{
    $lines = Get-Content $configFilePath
    $index = 0
    while($index -lt $lines.Length)
    {
        $line = $lines[$index]
        foreach($key in $dictionary.Keys)
        {
            if ($line.Contains($key))
            {
                $lines[$index] = UpdateLine $line $dictionary[$key]
            }
        }
        $index++
    }

    Set-Content -Path $configFilePath -Value $lines -Force
}
    
    # Update config file for 'client'
    # $configFile = $pwd.Path + "\..\MSALClient\AppConstants.cs"
    $configFile = $(Resolve-Path ($pwd.Path + "\..\MSALClient\AppConstants.cs"))
    
    $dictionary = @{ "858b4a09-dc31-45d3-83a7-2b5f024f99cd" = $clientAadApplication.AppId };

    Write-Host "Updating the sample config '$configFile' with the following config values"

    ReplaceInTextFile -configFilePath $configFile -dictionary $dictionary

    $configFile = $(Resolve-Path ($pwd.Path + "\..\MSALClient\AppConstants.cs"))
    
    $dictionary = @{ "7f58f645-c190-4ce5-9de4-e2b7acd2a6ab" = $tenantId };

    Write-Host "Updating the sample config '$configFile' with the following config values"

    ReplaceInTextFile -configFilePath $configFile -dictionary $dictionary
       
    if($isOpenSSL -eq 'Y')
    {
        Write-Host -ForegroundColor Green "------------------------------------------------------------------------------------------------" 
        Write-Host "You have generated certificate using OpenSSL so follow below steps: "
        Write-Host "Install the certificate on your system from current folder."
        Write-Host -ForegroundColor Green "------------------------------------------------------------------------------------------------" 
    }
    Add-Content -Value "</tbody></table></body></html>" -Path createdApps.html  
} # end of ConfigureApplications function

# Pre-requisites
if ($null -eq (Get-Module -ListAvailable -Name "Microsoft.Graph.Applications")) {
    Install-Module "Microsoft.Graph.Applications" -Scope CurrentUser 
}

Import-Module Microsoft.Graph.Applications

Set-Content -Value "<html><body><table>" -Path createdApps.html
Add-Content -Value "<thead><tr><th>Application</th><th>AppId</th><th>Url in the Azure portal</th></tr></thead><tbody>" -Path createdApps.html

$ErrorActionPreference = "Stop"

# Run interactively (will ask you for the tenant ID)

try
{
    ConfigureApplications -tenantId $tenantId -environment $azureEnvironmentName
}
catch
{
    $message = $_
    Write-Warning $Error[0]
    Write-Host "Unable to register apps. Error is $message." -ForegroundColor White -BackgroundColor Red
}
Write-Host "Disconnecting from tenant"
Disconnect-MgGraph