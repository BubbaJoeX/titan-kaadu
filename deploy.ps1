<#
#.SYNOPSIS
    Build and deploy Kaadu launcher to SFTP server.

#.DESCRIPTION
    This script builds the Kaadu launcher in Release mode, publishes it,
    and optionally deploys it to the SFTP server.

#.PARAMETER Version
    The version number for this release (e.g., 1.0.2)

#.PARAMETER Deploy
    If specified, deploy to SFTP server after building

#.PARAMETER SftpHost
    SFTP server hostname (default: test.swgtitan.org)

#.PARAMETER User
    SFTP username (will prompt if not provided)

#.EXAMPLE
    .\deploy.ps1 -Version 1.0.2
    Build only, no deployment

#.EXAMPLE
    .\deploy.ps1 -Version 1.0.2 -Deploy
    Build and deploy, prompting for credentials

#.EXAMPLE
    .\deploy.ps1 -Version 1.0.2 -Deploy -User admin
    Build and deploy with specified username
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [switch]$Deploy,
    
    [string]$SftpHost = "test.swgtitan.org",
    
    [string]$User
)

$ErrorActionPreference = "Stop"

Write-Host "=== Kaadu Launcher Build & Deploy ===" -ForegroundColor Cyan
Write-Host ""

# Validate version format
if (-not ($Version -match '^\d+\.\d+\.\d+$')) {
    Write-Host "Error: Invalid version format. Expected X.Y.Z (e.g., 1.0.2)" -ForegroundColor Red
    exit 1
}

# Update version in AppConfiguration.cs
$configPath = "Kaadu\Models\AppConfiguration.cs"
if (Test-Path $configPath) {
    Write-Host "Updating launcher version to $Version in AppConfiguration.cs..." -ForegroundColor Yellow
    $content = Get-Content $configPath -Raw
    $content = $content -replace 'LauncherVersion = "[^"]*"', "LauncherVersion = `"$Version`""
    Set-Content $configPath $content -NoNewline
    Write-Host "Version updated!" -ForegroundColor Green
}

# Build Release
Write-Host ""
Write-Host "Building Kaadu in Release mode..." -ForegroundColor Yellow
dotnet build Kaadu\Kaadu.csproj -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Publish
Write-Host ""
Write-Host "Publishing Kaadu..." -ForegroundColor Yellow
dotnet publish Kaadu\Kaadu.csproj -c Release -o "Kaadu\bin\Release\publish" --self-contained false

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish failed!" -ForegroundColor Red
    exit 1
}

$publishedExe = "Kaadu\bin\Release\net9.0-windows\publish\win-x64\Kaadu.exe"
if (-not (Test-Path $publishedExe)) {
    Write-Host "Error: Published executable not found at $publishedExe" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Build successful!" -ForegroundColor Green
Write-Host "Published to: $publishedExe"

if (-not $Deploy) {
    Write-Host ""
    Write-Host "To deploy, run:" -ForegroundColor Cyan
    Write-Host "  .\deploy.ps1 -Version $Version -Deploy"
    Write-Host ""
    Write-Host "Or manually:" -ForegroundColor Cyan
    Write-Host "  dotnet run --project Kaadu.Deploy -- $Version `"$publishedExe`""
    exit 0
}

# Build deploy tool
Write-Host ""
Write-Host "Building deployment tool..." -ForegroundColor Yellow
dotnet build Kaadu.Deploy\Kaadu.Deploy.csproj -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "Deploy tool build failed!" -ForegroundColor Red
    exit 1
}

# Run deployment
Write-Host ""
Write-Host "Starting deployment..." -ForegroundColor Yellow

$deployArgs = @($Version, $publishedExe, "--host", $SftpHost)

if ($User) {
    $deployArgs += @("--user", $User)
}

dotnet run --project Kaadu.Deploy\Kaadu.Deploy.csproj -c Release -- @deployArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host "Deployment failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=== Deployment Complete ===" -ForegroundColor Green
