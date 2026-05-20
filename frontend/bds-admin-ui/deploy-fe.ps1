param(
    [string]$HostName = "103.116.52.213",
    [string]$User = "root",
    [string]$RemotePath = "/var/www/bds-admin-ui"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$remote = "$User@$HostName"
$distPath = Join-Path $PSScriptRoot "dist/bds-admin-ui/browser"

Push-Location $PSScriptRoot
try {
    Write-Host "Building Angular production bundle..."
    npx.cmd ng build --configuration production

    if (-not (Test-Path $distPath)) {
        throw "Build output not found: $distPath"
    }

    Write-Host "Clearing remote frontend directory: ${remote}:$RemotePath"
    ssh $remote "rm -rf $RemotePath/*"

    Write-Host "Uploading frontend files..."
    scp -r "$distPath/*" "${remote}:$RemotePath/"

    Write-Host "Restarting nginx..."
    ssh $remote "systemctl restart nginx"

    Write-Host "Frontend deployment completed."
}
finally {
    Pop-Location
}
