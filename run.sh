sh ./test.sh

if [ $? -eq 0 ]; then
    cd ./BankingApp
    dotnet run --configuration Release --no-build --verbosity normal
fi