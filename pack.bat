@echo off
setlocal enabledelayedexpansion

echo Building and packing Arcanic.Mediator packages...
echo.

REM Create output directory
if not exist "nupkg" mkdir nupkg

REM Clean output directory
del /q "nupkg\*.nupkg" 2>nul
del /q "nupkg\*.snupkg" 2>nul

REM Build solution
echo Building solution...
dotnet build --configuration Release --verbosity minimal
if !errorlevel! neq 0 (
    echo Build failed
    exit /b 1
)

REM Pack projects in dependency order
set projects=src\Arcanic.Mediator.Abstractions\Arcanic.Mediator.Abstractions.csproj src\Arcanic.Mediator.Messaging.Abstractions\Arcanic.Mediator.Messaging.Abstractions.csproj src\Arcanic.Mediator.Command.Abstractions\Arcanic.Mediator.Command.Abstractions.csproj src\Arcanic.Mediator.Query.Abstractions\Arcanic.Mediator.Query.Abstractions.csproj src\Arcanic.Mediator.Event.Abstractions\Arcanic.Mediator.Event.Abstractions.csproj src\Arcanic.Mediator\Arcanic.Mediator.csproj src\Arcanic.Mediator.Messaging\Arcanic.Mediator.Messaging.csproj src\Arcanic.Mediator.Command\Arcanic.Mediator.Command.csproj src\Arcanic.Mediator.Query\Arcanic.Mediator.Query.csproj src\Arcanic.Mediator.Event\Arcanic.Mediator.Event.csproj

for %%p in (%projects%) do (
    echo Packing %%p...
    dotnet pack "%%p" --configuration Release --output nupkg --verbosity minimal --no-build
    if !errorlevel! neq 0 (
        echo Failed to pack %%p
        exit /b 1
    )
)

echo.
echo All packages created successfully in: nupkg\
echo.
echo Created packages:
dir /b nupkg\*.nupkg

pause