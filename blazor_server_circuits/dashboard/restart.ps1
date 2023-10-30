<#
.SYNOPSIS
A script to manage a Docker service.

.DESCRIPTION
This script allows you to stop a Docker service, pull its latest image, and start it in detached mode.

.PARAMETER Service
The name of the Docker service you want to manage. This parameter specifies which service to stop, pull, and start.

.EXAMPLE
.\restart.ps1 -Service "warehouse"
#>

param (
    [Parameter(Mandatory=$true, HelpMessage="Specify the name of the Docker service to manage.")]
    [string]$Service
)

# Store the original directory
$originalDir = Get-Location

# Change to the directory of the Docker files
Set-Location "C:\customer\lungmuss\refractory\docker"

$dockerCmd = "docker"
$composeArg = "compose"
$envFileArg = "--env-file"
$envFilePath = ".\.env\vars.env"
$fileArg = "--file"
$composeFilePath = "docker-compose.yml"
$composeDebugFilePath = "$originalDir\docker-compose-debug.yml"

# Stop the service
& $dockerCmd $composeArg $envFileArg $envFilePath $fileArg $composeFilePath $fileArg $composeDebugFilePath "stop" $Service

# Pull the latest image for the service
& $dockerCmd $composeArg $envFileArg $envFilePath $fileArg $composeFilePath $fileArg $composeDebugFilePath "pull" $Service

# Start the service in detached mode
& $dockerCmd $composeArg $envFileArg $envFilePath $fileArg $composeFilePath $fileArg $composeDebugFilePath "up" "-d" $Service

# Return to the original directory
Set-Location $originalDir
