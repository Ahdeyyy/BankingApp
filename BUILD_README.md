# Banking Application Build Scripts

This directory contains automated build and test scripts for the C# Banking Application.

## Available Scripts

### 1. Linux/macOS/Git Bash: `build.sh`

A comprehensive shell script for Unix-like systems and Git Bash on Windows.

### 2. Windows: `build.bat`

A Windows batch script for native Windows command prompt.

## Usage

### Shell Script (build.sh)

Make the script executable (if needed):

```bash
chmod +x build.sh
```

#### Basic Usage:

```bash
# Full build and test (recommended)
./build.sh

# Show help
./build.sh --help

# Clean only (remove build artifacts)
./build.sh --clean-only

# Build without running tests
./build.sh --skip-tests

# Build, test, and run the application
./build.sh --run
```

### Batch Script (build.bat)

#### Basic Usage:

```cmd
REM Full build and test (recommended)
build.bat

REM Show help
build.bat --help

REM Clean only (remove build artifacts)
build.bat --clean-only

REM Build without running tests
build.bat --skip-tests

REM Build, test, and run the application
build.bat --run
```

## What the Scripts Do

### 1. **Environment Check**

- Verifies .NET SDK is installed and accessible
- Checks for required project directories
- Displays .NET version information

### 2. **Clean Phase**

- Removes all previous build artifacts
- Cleans both main application and test projects
- Ensures a fresh build environment

### 3. **Restore Phase**

- Downloads and installs all NuGet package dependencies
- Restores packages for both projects
- Prepares the build environment

### 4. **Build Phase**

- Compiles the main Banking Application (Release configuration)
- Compiles the test project (Release configuration)
- Generates optimized executables

### 5. **Data Directory Setup**

- Creates the `data/` directory if it doesn't exist
- Ensures the application has a place to store persistence files

### 6. **Test Phase** (unless skipped)

- Runs all 170 unit tests
- Validates all banking operations
- Ensures code quality and functionality
- Reports test results with detailed output

### 7. **Build Information**

- Displays build artifact information
- Shows file sizes and locations
- Provides execution instructions

### 8. **Optional Application Run**

- Can automatically launch the application after successful build
- Provides interactive banking interface

## Script Features

### **Colored Output** (Shell Script)

- 🔵 **Blue**: Informational messages
- 🟢 **Green**: Success messages
- 🟡 **Yellow**: Warning messages
- 🔴 **Red**: Error messages

### **Error Handling**

- Stops execution on any build failure
- Provides clear error messages
- Returns appropriate exit codes

### **Performance Tracking**

- Measures total build time
- Reports completion time

### **Comprehensive Logging**

- Detailed progress information
- Build artifact details
- Test execution results

## Project Structure

```
BankingApp/
├── build.sh              # Unix/Linux/macOS/Git Bash script
├── build.bat             # Windows batch script
├── demo_script.txt       # Application usage examples
├── BankingApp/           # Main application project
│   ├── src.csproj        # Project file
│   ├── Program.cs        # Main application
│   ├── Bank.cs           # Banking logic
│   ├── Account.cs        # Account model
│   ├── Transaction.cs    # Transaction model
│   └── data/             # Data storage directory
└── BankingAppTests/      # Unit test project
    ├── tests.csproj      # Test project file
    ├── BankTest.cs       # Bank class tests (85 tests)
    ├── AccountTest.cs    # Account class tests (47 tests)
    └── TransactionTest.cs # Transaction class tests (38 tests)
```

## Build Outputs

After a successful build, you'll find:

### Main Application:

- `BankingApp/bin/Release/net9.0/src.dll` - .NET assembly
- `BankingApp/bin/Release/net9.0/src.exe` - Windows executable (Windows only)

### Test Project:

- `BankingAppTests/bin/Release/net9.0/tests.dll` - Test assembly

## Requirements

- **.NET 9.0 SDK** or later
- **Windows 10/11** (for Windows batch script)
- **Linux/macOS/Git Bash** (for shell script)
- Minimum **2GB RAM** for compilation
- **100MB** free disk space

## Test Coverage

The build scripts run a comprehensive test suite:

- **170 Total Tests**
- **Account Class**: 47 tests covering validation, edge cases, and error handling
- **Transaction Class**: 38 tests covering constructors, properties, and enum validation
- **Bank Class**: 85 tests covering all banking operations, data persistence, and integration scenarios

## Troubleshooting

### Common Issues:

1. **".NET CLI not found"**

   - Install .NET SDK from https://dotnet.microsoft.com/download
   - Ensure `dotnet` command is in your PATH

2. **"Permission denied" (Linux/macOS)**

   ```bash
   chmod +x build.sh
   ```

3. **"Project directories not found"**

   - Ensure you're running the script from the project root
   - Verify BankingApp and BankingAppTests directories exist

4. **Build failures**
   - Check for sufficient disk space
   - Verify internet connection for package restoration
   - Review error messages for specific issues

### Getting Help:

Run the help command for detailed usage information:

```bash
./build.sh --help        # Unix/Linux/macOS/Git Bash
build.bat --help         # Windows Command Prompt
```

## Examples

### Quick Build and Test:

```bash
./build.sh
```

### Development Workflow:

```bash
# Clean everything
./build.sh --clean-only

# Build and test
./build.sh

# Run the application
./build.sh --run
```

### CI/CD Pipeline:

```bash
# Automated build (no interaction)
./build.sh --skip-tests  # For faster builds
# or
./build.sh               # For full validation
```

---

**Note**: Both scripts provide identical functionality. Choose the one appropriate for your operating system and shell environment.
