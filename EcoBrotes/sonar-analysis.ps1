# =====================================================
#   SonarQube Analysis Pipeline
# =====================================================

param(
    [string]$SonarToken = $env:SONAR_TOKEN,
    [string]$SonarHostUrl = $env:SONAR_HOST_URL
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  SonarQube Analysis - EcoBrotes.NET" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Get current directory
$projectDir = "d:\Proyectos\ADN-Ceiba\NetCore8.1.0.0\EcoBrotes"
$solutionFile = Join-Path $projectDir "EcoBrotes.sln"

# Verify solution exists
if (-not (Test-Path $solutionFile)) {
    Write-Error "Solution file not found: $solutionFile"
    exit 1
}

Write-Host "`n[1/5] Restoring NuGet packages..." -ForegroundColor Yellow
& dotnet restore $solutionFile

if ($LASTEXITCODE -ne 0) {
    Write-Error "NuGet restore failed"
    exit 1
}

Write-Host "`n[2/5] Building solution..." -ForegroundColor Yellow
& dotnet build $solutionFile --configuration Release --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

Write-Host "`n[3/5] Running tests with coverage..." -ForegroundColor Yellow
$coverageOutput = Join-Path $projectDir "TestResults"
New-Item -ItemType Directory -Force -Path $coverageOutput | Out-Null

& dotnet test $solutionFile `
    --configuration Release `
    --no-build `
    --logger "trx;LogFileName=test-results.trx" `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=opencover `
    /p:CoverletOutput="$coverageOutput\coverage.opencover.xml" `
    --verbosity minimal

if ($LASTEXITCODE -ne 0) {
    Write-Warning "Some tests failed, but continuing analysis..."
}

Write-Host "`n[4/5] Running SonarScanner for analysis..." -ForegroundColor Yellow

# SonarScanner.MSBuild begin
Write-Host "Starting SonarScanner..." -ForegroundColor Cyan
& dotnet tool restore

& dotnet sonarscanner begin `
    /k:"EcoBrotes" `
    /o:"yeiser" `
    /d:sonar.login="$SonarToken" `
    /d:sonar.host.url="$SonarHostUrl" `
    /d:sonar.projectName="EcoBrotes.NET" `
    /d:sonar.projectVersion="1.0.0" `
    /d:sonar.sources="EcoBrotes" `
    /d:sonar.tests="EcoBrotes" `
    /d:sonar.test.inclusions="**/*Tests/**/*.cs,**/*Test/**/*.cs" `
    /d:sonar.cs.opencover.reportsPaths="$coverageOutput\coverage.opencover.xml" `
    /d:sonar.coverage.exclusions="**/*Tests/**/*.cs,**/*Test/**/*.cs"

if ($LASTEXITCODE -ne 0) {
    Write-Error "SonarScanner begin failed"
    exit 1
}

# Rebuild for scanner
& dotnet build $solutionFile --configuration Release

# SonarScanner end
& dotnet sonarscanner end /d:sonar.login="$SonarToken"

if ($LASTEXITCODE -ne 0) {
    Write-Error "SonarScanner end failed"
    exit 1
}

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "  Analysis Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "`nView results at:" -ForegroundColor Cyan
if ($SonarHostUrl) {
    Write-Host "$SonarHostUrl/dashboard?id=EcoBrotes" -ForegroundColor Cyan
} else {
    Write-Host "https://sonarcloud.io/dashboard?id=EcoBrotes" -ForegroundColor Cyan
}
