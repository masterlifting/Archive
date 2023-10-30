# rcli
Refractory Commandline Interface

```bash
docker run ghcr.io/lungmuss/rcli /app/rcli
```

To build a Docker image from the provided Dockerfile using Docker Desktop and PowerShell for Windows, you'll use
the `docker build` command.

Given the Dockerfile uses several ARGs (arguments), you'll need to provide values for them using the `--build-arg`
option.

Here's an example command to build the Docker image with the tag `ghcr.io/lungmuss/rcli`:

```powershell
$githubUser = "YourGitHubUsername"
$githubToken = "YourGitHubToken"
$buildConfiguration = "Release" # or "Debug" or any other configuration you might have
$projectName = "rcli"

docker build `
    --build-arg GITHUB_USER=$githubUser `
    --build-arg GITHUB_TOKEN=$githubToken `
    --build-arg BUILD_CONFIGURATION=$buildConfiguration `
    --build-arg PROJECT_NAME=$projectName `
    --build-arg INF_VER="1.0.0" `
    --build-arg SEMVER="1.0.0" `
    --build-arg RELEASE_TAG="v1.0" `
    -t ghcr.io/lungmuss/rcli `
    .
```

In the command above:

- Replace `YourGitHubUsername`, `YourGitHubToken`, and `NameOfYourProject` with appropriate values.
- Adjust the `INF_VER`, `SEMVER`, and `RELEASE_TAG` as needed for your versioning.
- The backticks (`) in PowerShell are used for multi-line commands.
- The `-t` flag specifies the tag of the image.
- The `.` at the end specifies the build context, which in this case is the current directory (assuming the Dockerfile
  is located there).

After building the image, you can push it to the GitHub Container Registry (`ghcr.io`) if you've set up authentication
and access for it. Ensure you're logged in with `docker login ghcr.io` using your GitHub credentials before pushing.

```powershell
docker run ghcr.io/lungmuss/rcli rcli/rcli -h André
```

```log
[15:15:17 INF] Assembly full name: rcli, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
[15:15:17 INF] Location: /rcli/rcli.dll
[15:15:17 INF] Informational version: develop
Hello André!
```

Example execute PowerShell script

```powershell
docker run ghcr.io/lungmuss/rcli pwsh -c "Write-Output 'Hello from PowerShell!'"
```

```powershell
docker run --rm ghcr.io/lungmuss/rcli pwsh -File /keycloak/test.ps1
```

```powershell
docker run --rm ghcr.io/lungmuss/rcli pwsh -Command "& /ijhttp/ijhttp --version"
```

Run keycloak direct

```powershell
docker run --rm ghcr.io/lungmuss/rcli pwsh -Command "& /ijhttp/ijhttp /keycloak/keycloak.http"
```

```powershell
docker run --rm ghcr.io/lungmuss/rcli pwsh -File /keycloak/httpcalls.ps1
```

```bash
docker run --rm ghcr.io/lungmuss/rcli /ijhttp/ijhttp /keycloak/keycloak.http
```

Run ijhttp direct

```bash
docker run ghcr.io/lungmuss/rcli ./ijhttp/ijhttp ./keycloak/keycloak.http --private-env-file ./keycloak/http-client.env.json --env dev
```
