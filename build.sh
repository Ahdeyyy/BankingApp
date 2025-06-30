sh ./test.sh
if [ $? -eq 0 ]; then
    cd ./BankingApp
    dotnet build --configuration Release --no-restore --verbosity normal
    
fi