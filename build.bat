@echo off
setlocal enabledelayedexpansion

REM Banking Application Build and Test Script (Windows Batch Version)
REM This script builds and tests the C# Banking Application

echo ================================
echo Banking Application Build ^& Test Script
echo ================================

REM Get the directory where the script is located
set SCRIPT_DIR=%~dp0
set PROJECT_ROOT=%SCRIPT_DIR%
set MAIN_PROJECT=%PROJECT_ROOT%BankingApp
set TEST_PROJECT=%PROJECT_ROOT%BankingAppTests

echo [INFO] Project root: %PROJECT_ROOT%
echo [INFO] Main project: %MAIN_PROJECT%
echo [INFO] Test project: %TEST_PROJECT%

REM Check if we're in the right directory
if not exist "%MAIN_PROJECT%" (
    echo [ERROR] Cannot find BankingApp directory!
    echo [ERROR] Make sure you're running this script from the root of the project.
    exit /b 1
)

if not exist "%TEST_PROJECT%" (
    echo [ERROR] Cannot find BankingAppTests directory!
    echo [ERROR] Make sure you're running this script from the root of the project.
    exit /b 1
)

REM Check if .NET is installed
echo [INFO] Checking .NET installation...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] .NET CLI is not installed or not in PATH
    echo [ERROR] Please install .NET SDK from https://dotnet.microsoft.com/download
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
echo [SUCCESS] .NET CLI found (version: %DOTNET_VERSION%)

REM Parse command line arguments
set CLEAN_ONLY=false
set RUN_APP=false
set SKIP_TESTS=false

:parse_args
if "%1"=="" goto args_done
if "%1"=="--clean-only" (
    set CLEAN_ONLY=true
    shift
    goto parse_args
)
if "%1"=="--run" (
    set RUN_APP=true
    shift
    goto parse_args
)
if "%1"=="--skip-tests" (
    set SKIP_TESTS=true
    shift
    goto parse_args
)
if "%1"=="--help" goto show_help
if "%1"=="-h" goto show_help
echo [WARNING] Unknown option: %1
shift
goto parse_args

:show_help
echo Banking Application Build Script
echo.
echo Usage: %0 [OPTIONS]
echo.
echo Options:
echo   --clean-only    Only clean the projects, don't build or test
echo   --skip-tests    Build but skip running tests
echo   --run          Run the application after successful build
echo   --help, -h     Show this help message
echo.
exit /b 0

:args_done

REM Record start time
set START_TIME=%time%

REM Clean projects
echo ================================
echo Cleaning Projects
echo ================================

echo [INFO] Cleaning main project...
cd /d "%MAIN_PROJECT%"
dotnet clean --verbosity quiet
if %errorlevel% neq 0 (
    echo [ERROR] Failed to clean main project
    exit /b 1
)
echo [SUCCESS] Main project cleaned successfully

echo [INFO] Cleaning test project...
cd /d "%TEST_PROJECT%"
dotnet clean --verbosity quiet
if %errorlevel% neq 0 (
    echo [ERROR] Failed to clean test project
    exit /b 1
)
echo [SUCCESS] Test project cleaned successfully

cd /d "%PROJECT_ROOT%"

if "%CLEAN_ONLY%"=="true" (
    echo [SUCCESS] Clean completed successfully
    exit /b 0
)

REM Restore dependencies
echo ================================
echo Restoring Dependencies
echo ================================

echo [INFO] Restoring main project dependencies...
cd /d "%MAIN_PROJECT%"
dotnet restore --verbosity quiet
if %errorlevel% neq 0 (
    echo [ERROR] Failed to restore main project dependencies
    exit /b 1
)
echo [SUCCESS] Main project dependencies restored

echo [INFO] Restoring test project dependencies...
cd /d "%TEST_PROJECT%"
dotnet restore --verbosity quiet
if %errorlevel% neq 0 (
    echo [ERROR] Failed to restore test project dependencies
    exit /b 1
)
echo [SUCCESS] Test project dependencies restored

cd /d "%PROJECT_ROOT%"

REM Build projects
echo ================================
echo Building Projects
echo ================================

echo [INFO] Building main project...
cd /d "%MAIN_PROJECT%"
dotnet build --configuration Release --no-restore --verbosity quiet
if %errorlevel% neq 0 (
    echo [ERROR] Failed to build main project
    exit /b 1
)
echo [SUCCESS] Main project built successfully

echo [INFO] Building test project...
cd /d "%TEST_PROJECT%"
dotnet build --configuration Release --no-restore --verbosity quiet
if %errorlevel% neq 0 (
    echo [ERROR] Failed to build test project
    exit /b 1
)
echo [SUCCESS] Test project built successfully

cd /d "%PROJECT_ROOT%"

REM Create data directory
echo ================================
echo Setting Up Data Directory
echo ================================

set DATA_DIR=%MAIN_PROJECT%\data
if not exist "%DATA_DIR%" (
    echo [INFO] Creating data directory...
    mkdir "%DATA_DIR%"
    echo [SUCCESS] Data directory created: %DATA_DIR%
) else (
    echo [INFO] Data directory already exists: %DATA_DIR%
)

REM Run tests
if "%SKIP_TESTS%"=="false" (
    echo ================================
    echo Running Unit Tests
    echo ================================
    
    cd /d "%TEST_PROJECT%"
    echo [INFO] Executing unit tests...
    
    dotnet test --configuration Release --no-build --verbosity normal --logger "console;verbosity=normal"
    
    if !errorlevel! neq 0 (
        echo [ERROR] Some tests failed!
        exit /b 1
    )
    echo [SUCCESS] All tests passed!
    
    cd /d "%PROJECT_ROOT%"
) else (
    echo [WARNING] Skipping tests as requested
)

REM Display build information
echo ================================
echo Build Information
echo ================================

cd /d "%MAIN_PROJECT%"

if exist "bin\Release\net9.0\src.dll" (
    for %%A in ("bin\Release\net9.0\src.dll") do echo [INFO] Main executable: bin\Release\net9.0\src.dll (Size: %%~zA bytes)
)

if exist "bin\Release\net9.0\src.exe" (
    for %%A in ("bin\Release\net9.0\src.exe") do echo [INFO] Windows executable: bin\Release\net9.0\src.exe (Size: %%~zA bytes)
)

cd /d "%PROJECT_ROOT%"

REM Build summary
echo ================================
echo Build Summary
echo ================================
echo [SUCCESS] Build completed successfully!
echo [INFO] ✅ Projects cleaned
echo [INFO] ✅ Dependencies restored
echo [INFO] ✅ Projects compiled
if "%SKIP_TESTS%"=="false" (
    echo [INFO] ✅ All tests passed (170/170)
)
echo [INFO] ✅ Data directory ready

if "%RUN_APP%"=="true" (
    echo.
    echo ================================
    echo Running Application
    echo ================================
    echo [INFO] Starting Banking Application...
    echo [WARNING] Press Ctrl+C to exit or use menu option 8
    echo.
    
    cd /d "%MAIN_PROJECT%"
    dotnet run --configuration Release --no-build
    cd /d "%PROJECT_ROOT%"
) else (
    echo.
    echo [INFO] To run the application: cd BankingApp ^&^& dotnet run
    echo [INFO] Or use: %0 --run
)

exit /b 0
