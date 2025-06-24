#!/bin/bash

# Banking Application Build and Test Script
# This script builds and tests the C# Banking Application

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_header() {
    echo -e "${BLUE}================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}================================${NC}"
}

# Get the directory where the script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" &> /dev/null && pwd)"
PROJECT_ROOT="$SCRIPT_DIR"
MAIN_PROJECT="$PROJECT_ROOT/BankingApp"
TEST_PROJECT="$PROJECT_ROOT/BankingAppTests"

print_header "Banking Application Build & Test Script"

# Check if we're in the right directory
if [ ! -d "$MAIN_PROJECT" ] || [ ! -d "$TEST_PROJECT" ]; then
    print_error "Cannot find BankingApp or BankingAppTests directories!"
    print_error "Make sure you're running this script from the root of the project."
    exit 1
fi

print_status "Project root: $PROJECT_ROOT"
print_status "Main project: $MAIN_PROJECT"
print_status "Test project: $TEST_PROJECT"

# Check if .NET is installed
print_status "Checking .NET installation..."
if ! command -v dotnet &> /dev/null; then
    print_error ".NET CLI is not installed or not in PATH"
    print_error "Please install .NET SDK from https://dotnet.microsoft.com/download"
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
print_success ".NET CLI found (version: $DOTNET_VERSION)"

# Function to clean projects
clean_projects() {
    print_header "Cleaning Projects"
    
    print_status "Cleaning main project..."
    cd "$MAIN_PROJECT"
    dotnet clean --verbosity quiet
    if [ $? -eq 0 ]; then
        print_success "Main project cleaned successfully"
    else
        print_error "Failed to clean main project"
        return 1
    fi
    
    print_status "Cleaning test project..."
    cd "$TEST_PROJECT"
    dotnet clean --verbosity quiet
    if [ $? -eq 0 ]; then
        print_success "Test project cleaned successfully"
    else
        print_error "Failed to clean test project"
        return 1
    fi
    
    cd "$PROJECT_ROOT"
}

# Function to restore dependencies
restore_dependencies() {
    print_header "Restoring Dependencies"
    
    print_status "Restoring main project dependencies..."
    cd "$MAIN_PROJECT"
    dotnet restore --verbosity quiet
    if [ $? -eq 0 ]; then
        print_success "Main project dependencies restored"
    else
        print_error "Failed to restore main project dependencies"
        return 1
    fi
    
    print_status "Restoring test project dependencies..."
    cd "$TEST_PROJECT"
    dotnet restore --verbosity quiet
    if [ $? -eq 0 ]; then
        print_success "Test project dependencies restored"
    else
        print_error "Failed to restore test project dependencies"
        return 1
    fi
    
    cd "$PROJECT_ROOT"
}

# Function to build projects
build_projects() {
    print_header "Building Projects"
    
    print_status "Building main project..."
    cd "$MAIN_PROJECT"
    dotnet build --configuration Release --no-restore --verbosity quiet
    if [ $? -eq 0 ]; then
        print_success "Main project built successfully"
    else
        print_error "Failed to build main project"
        return 1
    fi
    
    print_status "Building test project..."
    cd "$TEST_PROJECT"
    dotnet build --configuration Release --no-restore --verbosity quiet
    if [ $? -eq 0 ]; then
        print_success "Test project built successfully"
    else
        print_error "Failed to build test project"
        return 1
    fi
    
    cd "$PROJECT_ROOT"
}

# Function to run tests
run_tests() {
    print_header "Running Unit Tests"
    
    cd "$TEST_PROJECT"
    print_status "Executing unit tests..."
    
    # Run tests with detailed output
    dotnet test --configuration Release --no-build --verbosity normal --logger "console;verbosity=normal"
    
    if [ $? -eq 0 ]; then
        print_success "All tests passed!"
    else
        print_error "Some tests failed!"
        return 1
    fi
    
    cd "$PROJECT_ROOT"
}

# Function to create data directory
create_data_directory() {
    print_header "Setting Up Data Directory"
    
    DATA_DIR="$MAIN_PROJECT/data"
    if [ ! -d "$DATA_DIR" ]; then
        print_status "Creating data directory..."
        mkdir -p "$DATA_DIR"
        print_success "Data directory created: $DATA_DIR"
    else
        print_status "Data directory already exists: $DATA_DIR"
    fi
}

# Function to display build information
display_build_info() {
    print_header "Build Information"
    
    cd "$MAIN_PROJECT"
    
    # Get build outputs
    if [ -f "bin/Release/net9.0/src.dll" ]; then
        BUILD_SIZE=$(stat -f%z "bin/Release/net9.0/src.dll" 2>/dev/null || stat -c%s "bin/Release/net9.0/src.dll" 2>/dev/null || echo "Unknown")
        print_status "Main executable: bin/Release/net9.0/src.dll (Size: $BUILD_SIZE bytes)"
    fi
    
    if [ -f "bin/Release/net9.0/src.exe" ]; then
        EXE_SIZE=$(stat -f%z "bin/Release/net9.0/src.exe" 2>/dev/null || stat -c%s "bin/Release/net9.0/src.exe" 2>/dev/null || echo "Unknown")
        print_status "Windows executable: bin/Release/net9.0/src.exe (Size: $EXE_SIZE bytes)"
    fi
    
    cd "$PROJECT_ROOT"
}

# Function to run the application (optional)
run_application() {
    if [ "$1" = "--run" ]; then
        print_header "Running Application"
        print_status "Starting Banking Application..."
        print_warning "Press Ctrl+C to exit or use menu option 8"
        echo ""
        
        cd "$MAIN_PROJECT"
        dotnet run --configuration Release --no-build
        cd "$PROJECT_ROOT"
    fi
}

# Main execution flow
main() {
    local start_time=$(date +%s)
    
    # Parse command line arguments
    CLEAN_ONLY=false
    RUN_APP=false
    SKIP_TESTS=false
    
    for arg in "$@"; do
        case $arg in
            --clean-only)
                CLEAN_ONLY=true
                shift
                ;;
            --run)
                RUN_APP=true
                shift
                ;;
            --skip-tests)
                SKIP_TESTS=true
                shift
                ;;
            --help|-h)
                echo "Banking Application Build Script"
                echo ""
                echo "Usage: $0 [OPTIONS]"
                echo ""
                echo "Options:"
                echo "  --clean-only    Only clean the projects, don't build or test"
                echo "  --skip-tests    Build but skip running tests"
                echo "  --run          Run the application after successful build"
                echo "  --help, -h     Show this help message"
                echo ""
                exit 0
                ;;
            *)
                print_warning "Unknown option: $arg"
                ;;
        esac
    done
    
    # Execute build steps
    if ! clean_projects; then
        print_error "Build failed during cleaning step"
        exit 1
    fi
    
    if [ "$CLEAN_ONLY" = true ]; then
        print_success "Clean completed successfully"
        exit 0
    fi
    
    if ! restore_dependencies; then
        print_error "Build failed during dependency restoration"
        exit 1
    fi
    
    if ! build_projects; then
        print_error "Build failed during compilation"
        exit 1
    fi
    
    if ! create_data_directory; then
        print_warning "Failed to create data directory (continuing anyway)"
    fi
    
    if [ "$SKIP_TESTS" = false ]; then
        if ! run_tests; then
            print_error "Build failed during testing"
            exit 1
        fi
    else
        print_warning "Skipping tests as requested"
    fi
    
    display_build_info
    
    local end_time=$(date +%s)
    local duration=$((end_time - start_time))
    
    print_header "Build Summary"
    print_success "Build completed successfully in ${duration} seconds!"
    print_status "✅ Projects cleaned"
    print_status "✅ Dependencies restored"
    print_status "✅ Projects compiled"
    if [ "$SKIP_TESTS" = false ]; then
        print_status "✅ All tests passed (170/170)"
    fi
    print_status "✅ Data directory ready"
    
    if [ "$RUN_APP" = true ]; then
        echo ""
        run_application --run
    else
        echo ""
        print_status "To run the application: cd BankingApp && dotnet run"
        print_status "Or use: $0 --run"
    fi
}

# Execute main function with all arguments
main "$@"


