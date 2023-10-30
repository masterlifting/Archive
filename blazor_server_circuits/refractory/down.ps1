# Store the original directory
$originalDir = Get-Location

# Change to the directory of the first file
Set-Location "C:\customer\lungmuss\refractory\docker"

# Execute the Docker Compose command
docker compose --env-file .\.env\vars.env --file docker-compose.yml down

# Return to the original directory
Set-Location $originalDir
