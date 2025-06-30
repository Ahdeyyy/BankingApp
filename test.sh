cd ./BankingAppTests
dotnet test --configuration Release --no-build --verbosity normal --logger "console;verbosity=normal"
 if [ $? -eq 0 ]; then
        echo "All tests passed!"
    else
        echo "Some tests failed!"
        
    fi