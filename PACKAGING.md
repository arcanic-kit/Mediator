# Arcanic.Mediator NuGet Packaging

This document provides information about building and packaging the Arcanic.Mediator NuGet packages.

## Package Structure

The Arcanic.Mediator library is split into multiple NuGet packages for modular consumption:

### Core Packages
- **Arcanic.Mediator.Abstractions** - Core abstractions and interfaces (~13.7 KB)
- **Arcanic.Mediator** - Main dependency injection extensions (~15.9 KB)

### Messaging Infrastructure
- **Arcanic.Mediator.Messaging.Abstractions** - Messaging interfaces (~26.6 KB)
- **Arcanic.Mediator.Messaging** - Messaging implementation (~42.0 KB)

### Command Module
- **Arcanic.Mediator.Command.Abstractions** - Command interfaces (~14.9 KB)
- **Arcanic.Mediator.Command** - Command implementation (~27.9 KB)

### Query Module
- **Arcanic.Mediator.Query.Abstractions** - Query interfaces (~14.4 KB)
- **Arcanic.Mediator.Query** - Query implementation (~25.9 KB)

### Event Module
- **Arcanic.Mediator.Event.Abstractions** - Event interfaces (~13.3 KB)
- **Arcanic.Mediator.Event** - Event implementation (~26.4 KB)

**Total: 10 packages (combined ~247 KB)**

## Building Packages

### Using PowerShell Script (Recommended)

The PowerShell script provides the most comprehensive build and pack experience:

```powershell
# Basic build and pack
.\pack.ps1

# Include symbol packages
.\pack.ps1 -IncludeSymbols

# Specify configuration and output path
.\pack.ps1 -Configuration Release -OutputPath ".\dist" -IncludeSymbols

# Skip build step (if already built)
.\pack.ps1 -SkipBuild

# Cross-platform execution
pwsh pack.ps1 -IncludeSymbols
```

### Using Batch Script (Windows)

For simple Windows environments:

```cmd
pack.bat
```

### Manual Build

For complete control or CI/CD environments:

```bash
# Build all projects in dependency order
dotnet build src\Arcanic.Mediator.Abstractions\Arcanic.Mediator.Abstractions.csproj --configuration Release
dotnet build src\Arcanic.Mediator.Messaging.Abstractions\Arcanic.Mediator.Messaging.Abstractions.csproj --configuration Release
dotnet build src\Arcanic.Mediator.Command.Abstractions\Arcanic.Mediator.Command.Abstractions.csproj --configuration Release
dotnet build src\Arcanic.Mediator.Query.Abstractions\Arcanic.Mediator.Query.Abstractions.csproj --configuration Release
dotnet build src\Arcanic.Mediator.Event.Abstractions\Arcanic.Mediator.Event.Abstractions.csproj --configuration Release
dotnet build src\Arcanic.Mediator\Arcanic.Mediator.csproj --configuration Release
dotnet build src\Arcanic.Mediator.Messaging\Arcanic.Mediator.Messaging.csproj --configuration Release
dotnet build src\Arcanic.Mediator.Command\Arcanic.Mediator.Command.csproj --configuration Release
dotnet build src\Arcanic.Mediator.Query\Arcanic.Mediator.Query.csproj --configuration Release
dotnet build src\Arcanic.Mediator.Event\Arcanic.Mediator.Event.csproj --configuration Release

# Pack all projects
dotnet pack src\Arcanic.Mediator.Abstractions\Arcanic.Mediator.Abstractions.csproj --configuration Release --output nupkg --no-build
dotnet pack src\Arcanic.Mediator.Messaging.Abstractions\Arcanic.Mediator.Messaging.Abstractions.csproj --configuration Release --output nupkg --no-build
dotnet pack src\Arcanic.Mediator.Command.Abstractions\Arcanic.Mediator.Command.Abstractions.csproj --configuration Release --output nupkg --no-build
dotnet pack src\Arcanic.Mediator.Query.Abstractions\Arcanic.Mediator.Query.Abstractions.csproj --configuration Release --output nupkg --no-build
dotnet pack src\Arcanic.Mediator.Event.Abstractions\Arcanic.Mediator.Event.Abstractions.csproj --configuration Release --output nupkg --no-build
dotnet pack src\Arcanic.Mediator\Arcanic.Mediator.csproj --configuration Release --output nupkg --no-build
dotnet pack src\Arcanic.Mediator.Messaging\Arcanic.Mediator.Messaging.csproj --configuration Release --output nupkg --no-build
dotnet pack src\Arcanic.Mediator.Command\Arcanic.Mediator.Command.csproj --configuration Release --output nupkg --no-build
dotnet pack src\Arcanic.Mediator.Query\Arcanic.Mediator.Query.csproj --configuration Release --output nupkg --no-build
dotnet pack src\Arcanic.Mediator.Event\Arcanic.Mediator.Event.csproj --configuration Release --output nupkg --no-build
```

## Package Configuration

### Directory.Build.props

All packages share common properties defined in `Directory.Build.props`:

- **Version**: 0.1.0
- **Authors**: Intello Solutions
- **License**: MIT
- **Target Frameworks**: .NET 8.0, .NET 9.0, .NET 10.0
- **Repository**: https://github.com/arcanic-dotnet/mediator
- **Documentation**: XML documentation files included
- **Symbols**: Symbol packages (.snupkg) generated automatically

Each package has specific properties:
- **Package ID**: Unique package identifier
- **Description**: Package-specific description
- **Tags**: Relevant tags for discoverability
- **Release Notes**: Version-specific release information

### Sample Project Exclusion

Sample projects in the `samples/` folder are excluded from packaging via `samples\Directory.Build.props`:

```xml
<PropertyGroup>
  <IsPackable>false</IsPackable>
  <GeneratePackageOnBuild>false</GeneratePackageOnBuild>
</PropertyGroup>
```

## Publishing to NuGet

### Setting up API Key

First, set up your NuGet API key (do this once):

```bash
dotnet nuget setapikey YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

### Publishing Packages

```bash
# Push all packages to NuGet.org
dotnet nuget push nupkg\*.nupkg --source https://api.nuget.org/v3/index.json --skip-duplicate

# Push to a specific feed
dotnet nuget push nupkg\*.nupkg --source YOUR_FEED_URL --api-key YOUR_API_KEY

# Push including symbols
dotnet nuget push nupkg\*.nupkg --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push nupkg\*.snupkg --source https://nuget.smbsrc.net/ --skip-duplicate
```

### Validation Before Publishing

Before publishing, validate packages:

1. **Build validation**: Ensure all projects build without errors
   ```bash
   .\pack.ps1
   ```

2. **Pack validation**: Verify all packages are created successfully
   ```bash
   dir nupkg\*.nupkg
   ```

3. **Dependency validation**: Check that package dependencies are correct using NuGet Package Explorer

4. **Metadata validation**: Verify package metadata is accurate

## Version Management

To update the version for all packages, modify the properties in `Directory.Build.props`:

```xml
<!-- Version Information -->
<AssemblyVersion>0.2.0.0</AssemblyVersion>
<FileVersion>0.2.0.0</FileVersion>
<Version>0.2.0</Version>
```

### Pre-release Versions

For pre-release versions, use semantic versioning:

```xml
<Version>0.2.0-alpha.1</Version>  <!-- Alpha release -->
<Version>0.2.0-beta.1</Version>   <!-- Beta release -->
<Version>0.2.0-rc.1</Version>     <!-- Release candidate -->
```

## Package Dependencies

The packages have the following dependency relationships:

```
Arcanic.Mediator.Abstractions (foundation)
??? Arcanic.Mediator.Messaging.Abstractions
    ??? Arcanic.Mediator.Command.Abstractions
    ??? Arcanic.Mediator.Query.Abstractions
    ??? Arcanic.Mediator.Event.Abstractions
    ??? Arcanic.Mediator (core DI extensions)
        ??? Arcanic.Mediator.Messaging
            ??? Arcanic.Mediator.Command
            ??? Arcanic.Mediator.Query
            ??? Arcanic.Mediator.Event
```

## CI/CD Integration

### GitHub Actions

```yaml
name: Build and Pack

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build-and-pack:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: |
          8.0.x
          9.0.x
          
    - name: Pack NuGet packages
      run: pwsh pack.ps1 -IncludeSymbols
      
    - name: Upload packages
      uses: actions/upload-artifact@v4
      with:
        name: nuget-packages
        path: nupkg/*.nupkg
        
    - name: Publish to NuGet (Release only)
      if: github.ref == 'refs/heads/main'
      run: dotnet nuget push nupkg/*.nupkg --source https://api.nuget.org/v3/index.json --api-key ${{ secrets.NUGET_API_KEY }}
```

### Azure DevOps

```yaml
trigger:
- main

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'

steps:
- task: UseDotNet@2
  inputs:
    packageType: 'sdk'
    version: '8.x'

- task: PowerShell@2
  displayName: 'Build and Pack'
  inputs:
    filePath: 'pack.ps1'
    arguments: '-IncludeSymbols'
    pwsh: true

- task: NuGetCommand@2
  displayName: 'Push to NuGet'
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  inputs:
    command: 'push'
    packagesToPush: 'nupkg/*.nupkg'
    nuGetFeedType: 'external'
    publishFeedCredentials: 'NuGet.org'
```

## Troubleshooting

### Common Build Issues

1. **Missing dependencies**: Ensure projects are built in dependency order
2. **XML documentation errors**: Check for invalid cref attributes in XML comments
3. **Assembly version conflicts**: Verify all projects use consistent versioning

### Package Size Optimization

The packages are already optimized, but for further size reduction:

1. **Minimize dependencies**: Only include necessary project references
2. **Exclude development files**: Ensure test files and samples are excluded
3. **Use PackageReference**: Prefer PackageReference over project references where appropriate

### Symbol Package Issues

If symbol packages aren't generated:

1. Check that `IncludeSymbols` is set to `true`
2. Verify `SymbolPackageFormat` is set to `snupkg`
3. Ensure PDB files are generated during build

## Package Consumption

### Installing Individual Packages

```bash
# For command handling only
dotnet add package Arcanic.Mediator.Command

# For query handling only  
dotnet add package Arcanic.Mediator.Query

# For event handling only
dotnet add package Arcanic.Mediator.Event

# For full CQRS + Events
dotnet add package Arcanic.Mediator.Command
dotnet add package Arcanic.Mediator.Query
dotnet add package Arcanic.Mediator.Event
```

### Usage Examples

All packages include comprehensive XML documentation and work together seamlessly as shown in the main README.md file.