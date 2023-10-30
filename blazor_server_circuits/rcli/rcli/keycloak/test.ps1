# Define the API endpoint
$apiUrl = "https://jsonplaceholder.typicode.com/todos/1"

# Make the GET request
$response = Invoke-RestMethod -Uri $apiUrl -Method Get

# Display the response
$response

# Optionally, you can check if a specific field exists or has a certain value
if ($response.userId -eq 1)
{
    Write-Host "Test passed: Expected userId is 1" -ForegroundColor Green
}
else
{
    Write-Host "Test failed: Unexpected userId value" -ForegroundColor Red
}
