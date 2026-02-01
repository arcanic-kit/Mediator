# GitHub Actions CI/CD Setup

This directory contains GitHub Actions workflows for the Arcanic.Mediator project.

## Workflows

### 1. Build and Test Workflow (`build-and-test.yml`)

**Triggers:**
- Pull requests to `main` or `develop` branches

**What it does:**
- Builds the solution across multiple .NET versions (8, 9, 10)
- Runs all tests with code coverage collection
- Validates package creation
- Uses the `Arcanic.Mediator.slnx` solution file

### 2. Release Workflow (`release.yml`)

**Triggers:**
- Push of version tags (e.g., `v1.0.0`, `v2.1.3`)
- Manual workflow dispatch with version input

**What it does:**
- Builds the solution in Release configuration using multiple .NET versions (8, 9, 10)
- Runs tests to ensure quality
- Creates NuGet packages for all source projects (excluding samples, tests, and benchmarks)
- Publishes packages to both NuGet.org and GitHub Packages
- Creates symbol packages (.snupkg) for debugging
- Creates a GitHub release with package information
- Uploads packages as GitHub artifacts with 90-day retention

## Setup Instructions

### 1. Configure NuGet API Key

1. Go to [NuGet.org](https://www.nuget.org/) and sign in
2. Go to your account settings ? API Keys
3. Create a new API key with "Push" permissions for your packages
4. In your GitHub repository, go to Settings ? Secrets and Variables ? Actions
5. Add a new repository secret named `NUGET_API_KEY` with your API key value

Note: The workflow also publishes to GitHub Packages using the `GITHUB_TOKEN` (automatically available).

### 2. Release Process

#### Option A: Tag-based Release (Recommended)
```bash
# Create and push a version tag
git tag v1.0.0
git push origin v1.0.0
```

#### Option B: Manual Release
1. Go to your repository on GitHub
2. Navigate to Actions ? Release
3. Click "Run workflow"
4. Enter the version number (e.g., `1.0.0`)
5. Click "Run workflow"

### 3. Version Management

The workflow automatically handles version information:
- For tag-based releases: Extracts version from the tag name (removes 'v' prefix)
- For manual releases: Uses the version input provided
- Updates `AssemblyVersion`, `FileVersion`, and package `Version` properties
- Creates both regular packages (.nupkg) and symbol packages (.snupkg) for debugging

### 4. Package Output

The workflow creates packages for the source projects in the solution:
- `Arcanic.Mediator`
- `Arcanic.Mediator.Abstractions` 
- `Arcanic.Mediator.Command`
- `Arcanic.Mediator.Command.Abstractions`
- `Arcanic.Mediator.Event`
- `Arcanic.Mediator.Event.Abstractions`
- `Arcanic.Mediator.Query`
- `Arcanic.Mediator.Query.Abstractions`
- `Arcanic.Mediator.Request`
- `Arcanic.Mediator.Request.Abstractions`

Note: Sample projects, test projects, and benchmarks are excluded from packaging.

### 5. Troubleshooting

- **Build failures:** Check the CI workflow first to ensure your changes build correctly
- **Package conflicts:** The workflow uses `--skip-duplicate` to avoid conflicts with existing packages
- **Version conflicts:** Ensure you're using a new version number that hasn't been published before
- **API key issues:** Verify your NuGet API key has the correct permissions and hasn't expired

## Best Practices

1. **Always test locally first:** Run `dotnet build`, `dotnet test`, and `dotnet pack` locally before pushing
2. **Use semantic versioning:** Follow [SemVer](https://semver.org/) for version numbers
3. **Update changelogs:** Consider maintaining a CHANGELOG.md file for release notes
4. **Review packages:** Check the generated packages in the GitHub artifacts before they're published
5. **CI/CD separation:** The build-and-test workflow runs on PRs, while release workflow only runs on tags/manual dispatch

## Security Notes

- Never commit API keys or secrets to the repository
- Use GitHub repository secrets for sensitive information
- Regularly rotate your NuGet API keys
- Review workflow permissions and scope as needed
- GitHub Packages publishing uses the automatically provided `GITHUB_TOKEN`