<#
.SYNOPSIS
This script performs various tasks including executing Docker Compose commands, launching a dashboard, and optionally enabling database persistence.

.DESCRIPTION
The script first stores the original directory, switches to a specific directory, and runs a Docker Compose command with optional configurations based on the provided parameters.
After executing the Docker Compose command, it returns to the original directory. Finally, if the ShowDashboard parameter is set to true, it launches a browser to display a dashboard.

.PARAMETER ShowDashboard
Indicates whether to launch the dashboard in a browser. If set to true, a browser window is opened at the end of the script to display the dashboard. Default is true.

.PARAMETER persistDatabase
Specifies if the database should persist on the host. When set to true, a volume mapping is added to the postgres service in the docker-compose configuration to persist data between container restarts. Default is false.

.EXAMPLE
PS> .\up.ps1 -ShowDashboard $false

This example runs the script without opening the dashboard in a browser at the end.

.EXAMPLE
PS> .\up.ps1 -persistDatabase $true

This example runs the script with database persistence enabled, mapping a local directory to the container's database storage location.

#>
param(
    [Parameter(Mandatory=$false, HelpMessage="Indicate if the dashboard should be shown in the browser.")]
    [bool]$ShowDashboard = $true,
    [Parameter(Mandatory=$false, HelpMessage="Persist the database on host.")]
    [bool]$persistDatabase = $false
)

# Store the original directory
$originalDir = Get-Location

# Change to the directory of the first file
Set-Location "C:\customer\lungmuss\refractory\docker"

# Execute the Docker Compose command
if ($persistDatabase) {
    docker compose --env-file .\.env\vars.env --file docker-compose.yml --file docker-compose-presist-db.yml up -d
} else {
    docker compose --env-file .\.env\vars.env --file docker-compose.yml up -d
}

# Return to the original directory
Set-Location $originalDir

# Wait for 5 seconds
Start-Sleep -Seconds 5

# Open a browser at the specified address if ShowDashboard is true
if ($ShowDashboard) {
    Start-Process "http://localhost:9880"
}
