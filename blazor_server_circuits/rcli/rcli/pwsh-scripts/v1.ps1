# Sleep for 25 seconds
Start-Sleep -Seconds 25

# Execute ijhttp with the specified arguments
& "/ijhttp/ijhttp" "/keycloak/keycloak.http" `
    "--private-env-file" "/keycloak/http-client.env.json" `
    "--env" "dev" `
    "--docker-mode"

# Execute rcli for the database with the specified arguments
& "/rcli/rcli" "database" `
    "--database-name" "container" `
    "--sql-script" "/sql-sripts/init-dapr-config-store.sql"
