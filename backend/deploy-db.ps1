param(
    [string]$ConnectionString = $env:BDSADMIN_SUPABASE_DB_URL,
    [string]$Output = "migration.sql",
    [switch]$NoBuild
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Join-Path $scriptRoot "BdsAdmin.API/BdsAdmin.API.csproj"
$outputPath = if ([System.IO.Path]::IsPathRooted($Output)) {
    $Output
} else {
    Join-Path $scriptRoot $Output
}

if ([string]::IsNullOrWhiteSpace($ConnectionString)) {
    throw "Missing database connection string. Set BDSADMIN_SUPABASE_DB_URL or pass -ConnectionString."
}

$efArgs = @(
    "ef", "migrations", "script",
    "--idempotent",
    "--project", $projectPath,
    "--startup-project", $projectPath,
    "--output", $outputPath
)

if ($NoBuild) {
    $efArgs += "--no-build"
}

Write-Host "Generating EF migration SQL: $outputPath"
dotnet @efArgs

Write-Host "Applying migration SQL with psql..."
psql $ConnectionString -v ON_ERROR_STOP=1 -f $outputPath

Write-Host "Database deployment completed."
