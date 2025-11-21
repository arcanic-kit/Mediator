#!/usr/bin/env pwsh

# Build and pack all Arcanic.Mediator NuGet packages

param(
    [string]$Configuration = "Release",
    [string]$OutputPath = "nupkg",
    [switch]$IncludeSymbols,
    [switch]$SkipBuild
)

Write-Host "Building and packing Arcanic.Mediator packages..." -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Output Path: $OutputPath" -ForegroundColor Yellow

# Create output directory if it doesn't exist
if (!(Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    Write-Host "Created output directory: $OutputPath" -ForegroundColor Gray
}

# Clean output directory
if (Test-Path $OutputPath) {
    Get-ChildItem -Path $OutputPath -Filter "*.nupkg" -ErrorAction SilentlyContinue | Remove-Item -Force
    Get-ChildItem -Path $OutputPath -Filter "*.snupkg" -ErrorAction SilentlyContinue | Remove-Item -Force
    Write-Host "Cleaned output directory" -ForegroundColor Gray
}

# Define the projects to pack (in dependency order)
$projects = @(
    "src\Arcanic.Mediator.Abstractions\Arcanic.Mediator.Abstractions.csproj",
    "src\Arcanic.Mediator.Messaging.Abstractions\Arcanic.Mediator.Messaging.Abstractions.csproj",
    "src\Arcanic.Mediator.Command.Abstractions\Arcanic.Mediator.Command.Abstractions.csproj",
    "src\Arcanic.Mediator.Query.Abstractions\Arcanic.Mediator.Query.Abstractions.csproj",
    "src\Arcanic.Mediator.Event.Abstractions\Arcanic.Mediator.Event.Abstractions.csproj",
    "src\Arcanic.Mediator\Arcanic.Mediator.csproj",
    "src\Arcanic.Mediator.Messaging\Arcanic.Mediator.Messaging.csproj",
    "src\Arcanic.Mediator.Command\Arcanic.Mediator.Command.csproj",
    "src\Arcanic.Mediator.Query\Arcanic.Mediator.Query.csproj",
    "src\Arcanic.Mediator.Event\Arcanic.Mediator.Event.csproj"
)

# Build solution if not skipping build
if (!$SkipBuild) {
    Write-Host "Building core projects..." -ForegroundColor Yellow
    $buildSuccess = $true
    
    foreach ($project in $projects) {
        if (Test-Path $project) {
            Write-Host "Building $project..." -ForegroundColor Gray
            dotnet build $project --configuration $Configuration --verbosity minimal
            if ($LASTEXITCODE -ne 0) {
                Write-Error "Build failed for $project"
                $buildSuccess = $false
                break
            }
        }
    }
    
    if (!$buildSuccess) {
        Write-Error "Build failed"
        exit 1
    }
    Write-Host "Build completed successfully" -ForegroundColor Green
}

# Pack each project
$successCount = 0
$failCount = 0

foreach ($project in $projects) {
    if (Test-Path $project) {
        Write-Host "Packing $project..." -ForegroundColor Yellow
        
        $packArgs = @(
            "pack", $project,
            "--configuration", $Configuration,
            "--output", $OutputPath,
            "--verbosity", "minimal",
            "--no-build"
        )
        
        if ($IncludeSymbols) {
            $packArgs += "--include-symbols"
            $packArgs += "-p:SymbolPackageFormat=snupkg"
        }
        
        dotnet @packArgs
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "? Successfully packed $project" -ForegroundColor Green
            $successCount++
        } else {
            Write-Error "? Failed to pack $project"
            $failCount++
        }
    } else {
        Write-Warning "Project not found: $project"
        $failCount++
    }
}

# Summary
Write-Host "`nPackaging Summary:" -ForegroundColor Cyan
Write-Host "Successfully packed: $successCount projects" -ForegroundColor Green
Write-Host "Failed: $failCount projects" -ForegroundColor Red

if ($failCount -eq 0) {
    Write-Host "`nAll packages created successfully in: $OutputPath" -ForegroundColor Green
    
    # List created packages
    Write-Host "`nCreated packages:" -ForegroundColor Cyan
    if (Test-Path $OutputPath) {
        Get-ChildItem -Path $OutputPath -Filter "*.nupkg" | Sort-Object Name | ForEach-Object {
            $size = [math]::Round($_.Length / 1KB, 1)
            Write-Host "  $($_.Name) ($size KB)" -ForegroundColor Gray
        }
        
        if ($IncludeSymbols) {
            Write-Host "`nSymbol packages:" -ForegroundColor Cyan
            Get-ChildItem -Path $OutputPath -Filter "*.snupkg" | Sort-Object Name | ForEach-Object {
                $size = [math]::Round($_.Length / 1KB, 1)
                Write-Host "  $($_.Name) ($size KB)" -ForegroundColor Gray
            }
        }
        
        $totalPackages = (Get-ChildItem -Path $OutputPath -Filter "*.nupkg").Count
        Write-Host "`nTotal packages created: $totalPackages" -ForegroundColor Green
    }
    
    Write-Host "`nTo publish to NuGet.org:" -ForegroundColor Yellow
    Write-Host "  dotnet nuget push $OutputPath\*.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY" -ForegroundColor Gray
    
} else {
    Write-Error "Some packages failed to build"
    exit 1
}