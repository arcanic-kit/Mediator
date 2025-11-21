@echo off
setlocal

echo Building and packing all Arcanic.Mediator packages...
echo.

REM Create output directory
if not exist "nupkg" mkdir nupkg

REM Define projects in dependency order
set projects[0]=src\Arcanic.Mediator.Abstractions\Arcanic.Mediator.Abstractions.csproj
set projects[1]=src\Arcanic.Mediator.Messaging.Abstractions\Arcanic.Mediator.Messaging.Abstractions.csproj
set projects[2]=src\Arcanic.Mediator.Command.Abstractions\Arcanic.Mediator.Command.Abstractions.csproj
set projects[3]=src\Arcanic.Mediator.Query.Abstractions\Arcanic.Mediator.Query.Abstractions.csproj
set projects[4]=src\Arcanic.Mediator.Event.Abstractions\Arcanic.Mediator.Event.Abstractions.csproj
set projects[5]=src\Arcanic.Mediator\Arcanic.Mediator.csproj
set projects[6]=src\Arcanic.Mediator.Messaging\Arcanic.Mediator.Messaging.csproj
set projects[7]=src\Arcanic.Mediator.Command\Arcanic.Mediator.Command.csproj
set projects[8]=src\Arcanic.Mediator.Query\Arcanic.Mediator.Query.csproj
set projects[9]=src\Arcanic.Mediator.Event\Arcanic.Mediator.Event.csproj

echo Building projects...
for /L %%i in (0,1,9) do (
    call set proj=%%projects[%%i]%%
    if defined proj (
        echo Building !proj!...
        dotnet build "!proj!" --configuration Release --verbosity minimal
        if !errorlevel! neq 0 (
            echo Build failed for !proj!
            exit /b 1
        )
    )
)

echo.
echo Packing projects...
for /L %%i in (0,1,9) do (
    call set proj=%%projects[%%i]%%
    if defined proj (
        echo Packing !proj!...
        dotnet pack "!proj!" --configuration Release --output nupkg --verbosity minimal --no-build
        if !errorlevel! neq 0 (
            echo Pack failed for !proj!
            exit /b 1
        )
    )
)

echo.
echo All packages created successfully in nupkg\
echo.
echo Created packages:
dir /b nupkg\*.nupkg

pause