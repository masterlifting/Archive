# Call dotnet-gitversion and parse its output
# Install the dotnet tool with: dotnet tool install --global GitVersion.Tool
$gitVersionOutput = dotnet-gitversion | ConvertFrom-Json
$infoVersion = $gitVersionOutput.InformationalVersion

# Set an environment variable for this session
$env:INF_VERSION = $infoVersion

# Store the original directory
$originalDir = Get-Location

# Change to the directory of the first file
Set-Location "C:\customer\lungmuss\refractory\docker"

# Set the $originalDir as an environment variable
$env:ORIGINAL_DIR = $originalDir

# Execute the Docker Compose command
docker compose --env-file .\.env\vars.env --file docker-compose.yml --file $originalDir\docker-compose-run.yml up --detach --build

# Return to the original directory
Set-Location $originalDir

# Wait for 5 seconds
Start-Sleep -Seconds 5

# Open a browser at the specified address
Start-Process "http://localhost:9880"