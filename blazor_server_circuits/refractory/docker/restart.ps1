param ([string]$Service = "dashboard")

$dockerCmd = "docker"
$composeArg = "compose"
$envFileArg = "--env-file"
$envFilePath = ".\.env\vars.env"
$fileArg = "--file"
$composeFilePath = "C:\customer\lungmuss\refractory\docker\docker-compose.yml"

# Stop the service
& $dockerCmd $composeArg $envFileArg $envFilePath $fileArg $composeFilePath "stop" $Service

# Pull the latest image for the service
& $dockerCmd $composeArg $envFileArg $envFilePath $fileArg $composeFilePath "pull" $Service

# Start the service in detached mode
& $dockerCmd $composeArg $envFileArg $envFilePath $fileArg $composeFilePath "up" "-d" $Service
