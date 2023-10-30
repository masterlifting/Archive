# Store the original directory
$originalDir = Get-Location

# Change to the directory of the first file
Set-Location "C:\customer\lungmuss\refractory\docker"

# Execute the Docker Compose command
docker compose --env-file .\.env\vars.env --file docker-compose.yml --file $originalDir\docker-compose-debug.yml up -d

# Return to the original directory
Set-Location $originalDir

# Wait for 5 seconds
Start-Sleep -Seconds 5