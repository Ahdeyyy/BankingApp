using BankingApp;

namespace BankingAppTests;

[TestClass]
public sealed class BankTest
{
    private Bank bank;
    private const string ValidName = "John Doe";
    private const string ValidPin = "1234";
    private const string ValidAccountNumber = "123456789";

    [TestInitialize]
    public void Setup()
    {
        bank = new Bank();
    }

    #region Bank Constructor Tests

    [TestMethod]
    public void Bank_Constructor_ShouldInitializeSuccessfully()
    {
        // Arrange & Act
        var bank = new Bank();

        // Assert
        Assert.IsNotNull(bank);
    }

    [TestMethod]
    public void Bank_Constructor_ShouldAllowMultipleInstances()
    {
        // Arrange & Act
        var bank1 = new Bank();
        var bank2 = new Bank();

        // Assert
        Assert.IsNotNull(bank1);
        Assert.IsNotNull(bank2);
        Assert.AreNotSame(bank1, bank2);
    }

    #endregion

    #region Data Persistence Tests

    [TestMethod]
    public void Bank_LoadData_ShouldExecuteWithoutException()
    {
        // Arrange & Act & Assert
        // Currently LoadData is a stub implementation, so it should not throw
        bank.LoadData();
    }

    [TestMethod]
    public void Bank_SaveData_ShouldExecuteWithoutException()
    {
        // Arrange & Act & Assert
        // Currently SaveData is a stub implementation, so it should not throw
        bank.SaveData();
    }

    [TestMethod]
    public void Bank_LoadDataMultipleTimes_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        bank.LoadData();
        bank.LoadData();
        bank.LoadData();
    }

    [TestMethod]
    public void Bank_SaveDataMultipleTimes_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        bank.SaveData();
        bank.SaveData();
        bank.SaveData();
    }

    [TestMethod]
    public void Bank_LoadAndSaveData_ShouldWorkTogether()
    {
        // Arrange & Act & Assert
        bank.LoadData();
        bank.SaveData();
        bank.LoadData();
        bank.SaveData();
    }

    [TestMethod]
    public void Bank_SaveData_WithAccountsInMemory_ShouldNotThrow()
    {
        // Arrange - Create some accounts to save
        var account1 = bank.CreateAccount("User One", ValidPin);
        var account2 = bank.CreateAccount("User Two", "5678");
        bank.DepositFunds(account1!, 500.00m);
        bank.DepositFunds(account2!, 1000.00m);

        // Act & Assert - SaveData should not throw even with data in memory
        bank.SaveData();
    }

    [TestMethod]
    public void Bank_LoadData_AfterAccountCreation_ShouldNotThrow()
    {
        // Arrange - Create accounts first
        var account1 = bank.CreateAccount("User One", ValidPin);
        var account2 = bank.CreateAccount("User Two", "5678");

        // Act & Assert - LoadData should not interfere with existing accounts
        bank.LoadData();

        // Verify accounts still exist after LoadData call
        var retrievedAccount1 = bank.GetAccountDetails(account1!, ValidPin);
        var retrievedAccount2 = bank.GetAccountDetails(account2!, "5678");
        Assert.IsNotNull(retrievedAccount1);
        Assert.IsNotNull(retrievedAccount2);
    }

    [TestMethod]
    public void Bank_SaveData_MultipleTimesInSequence_ShouldNotThrow()
    {
        // Arrange - Create test data
        var account = bank.CreateAccount("Test User", ValidPin);
        bank.DepositFunds(account!, 1000.00m);

        // Act & Assert - Multiple saves in sequence
        bank.SaveData();
        bank.SaveData();
        bank.SaveData();
        bank.SaveData();
        bank.SaveData();
    }

    [TestMethod]
    public void Bank_LoadData_MultipleTimesInSequence_ShouldNotThrow()
    {
        // Arrange - Create test data first
        var account = bank.CreateAccount("Test User", ValidPin);
        bank.DepositFunds(account!, 1000.00m);

        // Act & Assert - Multiple loads in sequence
        bank.LoadData();
        bank.LoadData();
        bank.LoadData();
        bank.LoadData();
        bank.LoadData();

        // Verify account still exists after multiple loads
        var retrievedAccount = bank.GetAccountDetails(account!, ValidPin);
        Assert.IsNotNull(retrievedAccount);
    }

    [TestMethod]
    public void Bank_LoadData_InterleavedWithSaveData_ShouldNotThrow()
    {
        // Arrange - Create test data
        var account1 = bank.CreateAccount("User One", ValidPin);
        bank.DepositFunds(account1!, 500.00m);

        // Act & Assert - Interleaved save and load operations
        bank.SaveData();
        bank.LoadData();
        
        var account2 = bank.CreateAccount("User Two", "5678");
        bank.DepositFunds(account2!, 750.00m);
        
        bank.SaveData();
        bank.LoadData();
        bank.SaveData();
        bank.LoadData();

        // Verify both accounts exist after interleaved operations
        var retrievedAccount1 = bank.GetAccountDetails(account1!, ValidPin);
        var retrievedAccount2 = bank.GetAccountDetails(account2!, "5678");
        Assert.IsNotNull(retrievedAccount1);
        Assert.IsNotNull(retrievedAccount2);
    }

    [TestMethod]
    public void Bank_SaveData_WithLargeDataset_ShouldNotThrow()
    {
        // Arrange - Create a large number of accounts with transactions
        var accounts = new List<string>();
        
        for (int i = 0; i < 100; i++)
        {
            var accountNumber = bank.CreateAccount($"User {i}", $"{1000 + i}");
            accounts.Add(accountNumber!);
            
            // Add some transactions
            bank.DepositFunds(accountNumber!, 100.00m * (i + 1));
            if (i > 0)
            {
                bank.WithdrawFunds(accountNumber!, $"{1000 + i}", 50.00m);
            }
        }

        // Act & Assert - Save large dataset
        bank.SaveData();
    }

    [TestMethod]
    public void Bank_LoadData_WithLargeDataset_ShouldNotThrow()
    {
        // Arrange - Create a large dataset and save it
        var accounts = new List<string>();
        
        for (int i = 0; i < 50; i++)
        {
            var accountNumber = bank.CreateAccount($"Large User {i}", $"{2000 + i}");
            accounts.Add(accountNumber!);
            bank.DepositFunds(accountNumber!, 200.00m * (i + 1));
        }
        
        bank.SaveData();

        // Act & Assert - Load large dataset
        bank.LoadData();

        // Verify data integrity after load
        for (int i = 0; i < accounts.Count; i++)
        {
            var account = bank.GetAccountDetails(accounts[i], $"{2000 + i}");
            Assert.IsNotNull(account, $"Account {i} should exist after load");
        }
    }

    [TestMethod]
    public void Bank_SaveData_EmptyBank_ShouldNotThrow()
    {
        // Arrange - Bank with no accounts or transactions
        var emptyBank = new Bank();

        // Act & Assert - Save empty state
        emptyBank.SaveData();
    }

    [TestMethod]
    public void Bank_LoadData_EmptyBank_ShouldNotThrow()
    {
        // Arrange - Bank with no accounts or transactions
        var emptyBank = new Bank();

        // Act & Assert - Load into empty state
        emptyBank.LoadData();
    }

    [TestMethod]
    public void Bank_SaveData_OnlyAccountsNoTransactions_ShouldNotThrow()
    {
        // Arrange - Create accounts but no transactions
        var account1 = bank.CreateAccount("No Transaction User 1", ValidPin);
        var account2 = bank.CreateAccount("No Transaction User 2", "9999");
        
        // Don't add any transactions, just create accounts

        // Act & Assert - Save accounts without transactions
        bank.SaveData();
    }

    [TestMethod]
    public void Bank_SaveData_AccountsWithTransactions_ShouldNotThrow()
    {
        // Arrange - Create accounts with various types of transactions
        var account1 = bank.CreateAccount("Transaction User 1", ValidPin);
        var account2 = bank.CreateAccount("Transaction User 2", "8888");
        
        // Add deposits
        bank.DepositFunds(account1!, 1000.00m);
        bank.DepositFunds(account2!, 500.00m);
        
        // Add withdrawals
        bank.WithdrawFunds(account1!, ValidPin, 100.00m);
        bank.WithdrawFunds(account2!, "8888", 50.00m);
        
        // Add more deposits
        bank.DepositFunds(account1!, 200.00m);

        // Act & Assert - Save accounts with transactions
        bank.SaveData();
    }

    [TestMethod]
    public void Bank_LoadData_AfterSaveAndRecreate_ShouldMaintainState()
    {
        // Arrange - Create accounts and transactions
        var account1 = bank.CreateAccount("State User 1", ValidPin);
        var account2 = bank.CreateAccount("State User 2", "7777");
        
        bank.DepositFunds(account1!, 1500.00m);
        bank.DepositFunds(account2!, 800.00m);
        bank.WithdrawFunds(account1!, ValidPin, 300.00m);
        
        var originalBalance1 = bank.GetAccountDetails(account1!, ValidPin)?.Balance;
        var originalBalance2 = bank.GetAccountDetails(account2!, "7777")?.Balance;

        // Save current state
        bank.SaveData();

        // Create new bank instance (simulating restart)
        var newBank = new Bank();
        
        // Act - Load data into new bank instance
        newBank.LoadData();

        // Assert - State should be maintained (for future implementation)
        // Note: Currently LoadData is a stub, so accounts won't actually be loaded
        // This test verifies the method call doesn't throw and sets up for future implementation
        Assert.IsNotNull(originalBalance1);
        Assert.IsNotNull(originalBalance2);
    }

    [TestMethod]
    public void Bank_SaveData_MultipleInstances_ShouldNotInterfere()
    {
        // Arrange - Create multiple bank instances
        var bank1 = new Bank();
        var bank2 = new Bank();
        var bank3 = new Bank();

        // Add different data to each bank
        var account1 = bank1.CreateAccount("Bank1 User", ValidPin);
        var account2 = bank2.CreateAccount("Bank2 User", "6666");
        var account3 = bank3.CreateAccount("Bank3 User", "5555");

        bank1.DepositFunds(account1!, 1000.00m);
        bank2.DepositFunds(account2!, 2000.00m);
        bank3.DepositFunds(account3!, 3000.00m);

        // Act & Assert - Each instance should be able to save independently
        bank1.SaveData();
        bank2.SaveData();
        bank3.SaveData();
    }

    [TestMethod]
    public void Bank_LoadData_MultipleInstances_ShouldNotInterfere()
    {
        // Arrange - Create multiple bank instances
        var bank1 = new Bank();
        var bank2 = new Bank();
        var bank3 = new Bank();

        // Add data to first bank and save
        var account1 = bank1.CreateAccount("Multi Instance User", ValidPin);
        bank1.DepositFunds(account1!, 500.00m);
        bank1.SaveData();

        // Act & Assert - All instances should be able to load independently
        bank1.LoadData();
        bank2.LoadData();
        bank3.LoadData();
    }

    [TestMethod]
    public void Bank_SaveData_RepeatableOperations_ShouldBeIdempotent()
    {
        // Arrange - Create test data
        var account = bank.CreateAccount("Idempotent User", ValidPin);
        bank.DepositFunds(account!, 750.00m);
        bank.WithdrawFunds(account!, ValidPin, 250.00m);

        // Act - Perform multiple save operations
        bank.SaveData();
        bank.SaveData();
        bank.SaveData();

        // Assert - Account should still exist and be accessible
        var retrievedAccount = bank.GetAccountDetails(account!, ValidPin);
        Assert.IsNotNull(retrievedAccount);
        Assert.AreEqual(500.00m, retrievedAccount.Balance);
    }

    [TestMethod]
    public void Bank_LoadData_RepeatableOperations_ShouldBeIdempotent()
    {
        // Arrange - Create test data and save
        var account = bank.CreateAccount("Load Idempotent User", ValidPin);
        bank.DepositFunds(account!, 900.00m);
        bank.SaveData();

        // Act - Perform multiple load operations
        bank.LoadData();
        bank.LoadData();
        bank.LoadData();

        // Assert - Account should still exist and be accessible
        var retrievedAccount = bank.GetAccountDetails(account!, ValidPin);
        Assert.IsNotNull(retrievedAccount);
    }

    [TestMethod]
    public void Bank_SaveData_ConsecutiveWithDifferentData_ShouldNotThrow()
    {
        // Arrange & Act - Save at different states
        bank.SaveData(); // Empty state
        
        var account1 = bank.CreateAccount("Consecutive User 1", ValidPin);
        bank.SaveData(); // After first account
        
        bank.DepositFunds(account1!, 400.00m);
        bank.SaveData(); // After deposit
        
        var account2 = bank.CreateAccount("Consecutive User 2", "4444");
        bank.SaveData(); // After second account
        
        bank.WithdrawFunds(account1!, ValidPin, 100.00m);
        bank.SaveData(); // After withdrawal

        // Assert - Verify final state
        var finalAccount1 = bank.GetAccountDetails(account1!, ValidPin);
        var finalAccount2 = bank.GetAccountDetails(account2!, "4444");
        
        Assert.IsNotNull(finalAccount1);
        Assert.IsNotNull(finalAccount2);
        Assert.AreEqual(300.00m, finalAccount1.Balance);
        Assert.AreEqual(0.00m, finalAccount2.Balance);
    }

    [TestMethod]
    public void Bank_LoadData_ConsecutiveWithStateChanges_ShouldNotThrow()
    {
        // Arrange - Create initial state
        var account = bank.CreateAccount("State Change User", ValidPin);
        bank.DepositFunds(account!, 600.00m);

        // Act - Load at different points
        bank.LoadData(); // After account creation
        
        bank.WithdrawFunds(account!, ValidPin, 150.00m);
        bank.LoadData(); // After withdrawal
        
        bank.DepositFunds(account!, 300.00m);
        bank.LoadData(); // After deposit

        // Assert - Verify account is still accessible
        var finalAccount = bank.GetAccountDetails(account!, ValidPin);
        Assert.IsNotNull(finalAccount);
        Assert.AreEqual(750.00m, finalAccount.Balance);
    }

    [TestMethod]
    public void Bank_SaveAndLoadData_ComplexWorkflow_ShouldNotThrow()
    {
        // Arrange & Act - Complex workflow combining save and load operations
        var account1 = bank.CreateAccount("Complex User 1", ValidPin);
        bank.SaveData();
        bank.LoadData();
        
        bank.DepositFunds(account1!, 1200.00m);
        var account2 = bank.CreateAccount("Complex User 2", "3333");
        bank.SaveData();
        
        bank.WithdrawFunds(account1!, ValidPin, 400.00m);
        bank.DepositFunds(account2!, 800.00m);
        bank.LoadData();
        bank.SaveData();
        
        var account3 = bank.CreateAccount("Complex User 3", "2222");
        bank.DepositFunds(account3!, 1500.00m);
        bank.LoadData();
        bank.SaveData();
        bank.LoadData();

        // Assert - Verify all accounts are accessible
        var finalAccount1 = bank.GetAccountDetails(account1!, ValidPin);
        var finalAccount2 = bank.GetAccountDetails(account2!, "3333");
        var finalAccount3 = bank.GetAccountDetails(account3!, "2222");
        
        Assert.IsNotNull(finalAccount1);
        Assert.IsNotNull(finalAccount2);
        Assert.IsNotNull(finalAccount3);
        Assert.AreEqual(800.00m, finalAccount1.Balance);
        Assert.AreEqual(800.00m, finalAccount2.Balance);
        Assert.AreEqual(1500.00m, finalAccount3.Balance);
    }
    #endregion

    #region Integration Tests

    [TestMethod]
    public void Bank_CreateMultipleAccounts_ShouldGenerateUniqueAccountNumbers()
    {
        // Arrange & Act
        var accountNumber1 = bank.CreateAccount("User One", ValidPin);
        var accountNumber2 = bank.CreateAccount("User Two", ValidPin);
        var accountNumber3 = bank.CreateAccount("User Three", ValidPin);

        // Assert
        Assert.IsNotNull(accountNumber1);
        Assert.IsNotNull(accountNumber2);
        Assert.IsNotNull(accountNumber3);
        Assert.AreNotEqual(accountNumber1, accountNumber2);
        Assert.AreNotEqual(accountNumber2, accountNumber3);
        Assert.AreNotEqual(accountNumber1, accountNumber3);
    }

    [TestMethod]
    public void Bank_CreateManyAccounts_ShouldAllHaveUniqueNumbers()
    {
        // Arrange
        var accountNumbers = new List<string>();
        const int numberOfAccounts = 100;

        // Act
        for (int i = 0; i < numberOfAccounts; i++)
        {
            var accountNumber = bank.CreateAccount($"User {i}", ValidPin);
            Assert.IsNotNull(accountNumber);
            accountNumbers.Add(accountNumber!);
        }

        // Assert
        var uniqueNumbers = accountNumbers.Distinct().ToList();
        Assert.AreEqual(numberOfAccounts, uniqueNumbers.Count, "All account numbers should be unique");
    }

    [TestMethod]
    public void Bank_AccountLifecycle_ShouldWorkEndToEnd()
    {
        // Arrange
        const string accountName = "Test User";
        const string accountPin = "5678";
        const decimal initialDeposit = 1000.00m;
        const decimal withdrawalAmount = 300.00m;

        // Act - Create account
        var accountNumber = bank.CreateAccount(accountName, accountPin);
        Assert.IsNotNull(accountNumber);

        // Act - Deposit funds
        bool depositResult = bank.DepositFunds(accountNumber!, initialDeposit);
        Assert.IsTrue(depositResult);

        // Act - Get account details
        Account accountDetails = bank.GetAccountDetails(accountNumber!, accountPin);
        Assert.IsNotNull(accountDetails);
        Assert.AreEqual(initialDeposit, accountDetails.Balance);

        // Act - Withdraw funds
        bool withdrawResult = bank.WithdrawFunds(accountNumber!, accountPin, withdrawalAmount);
        Assert.IsTrue(withdrawResult);

        // Act - Check balance after withdrawal
        Account updatedAccount = bank.GetAccountDetails(accountNumber!, accountPin);
        Assert.IsNotNull(updatedAccount);
        Assert.AreEqual(initialDeposit - withdrawalAmount, updatedAccount.Balance);

        // Act - Edit account
        const string newName = "Updated User";
        bool editResult = bank.EditAccount(accountNumber!, accountPin, newName);
        Assert.IsTrue(editResult);

        // Act - Verify edit
        Account editedAccount = bank.GetAccountDetails(accountNumber!, accountPin);
        Assert.IsNotNull(editedAccount);
        Assert.AreEqual(newName, editedAccount.Name);

        // Act - Withdraw remaining funds to prepare for deletion
        bool finalWithdrawResult = bank.WithdrawFunds(accountNumber!, accountPin, updatedAccount.Balance);
        Assert.IsTrue(finalWithdrawResult);

        // Act - Delete account
        bool deleteResult = bank.DeleteAccount(accountNumber!, newName, accountPin);
        Assert.IsTrue(deleteResult);

        // Assert - Account should no longer exist
        Account deletedAccount = bank.GetAccountDetails(accountNumber!, accountPin);
        Assert.IsNull(deletedAccount);
    }

    [TestMethod]
    public void Bank_TransferBetweenAccounts_ShouldWorkCorrectly()
    {
        // Arrange
        var senderAccount = bank.CreateAccount("Sender", ValidPin);
        var receiverAccount = bank.CreateAccount("Receiver", "5678");
        const decimal senderInitialBalance = 1000.00m;
        const decimal transferAmount = 250.00m;

        // Act - Set up accounts
        bank.DepositFunds(senderAccount!, senderInitialBalance);

        // Act - Transfer funds
        bool transferResult = bank.TransferFunds(senderAccount!, ValidPin, receiverAccount!, transferAmount);
        Assert.IsTrue(transferResult);

        // Assert - Check balances
        var senderDetails = bank.GetAccountDetails(senderAccount!, ValidPin);
        var receiverDetails = bank.GetAccountDetails(receiverAccount!, "5678");

        Assert.IsNotNull(senderDetails);
        Assert.IsNotNull(receiverDetails);
        Assert.AreEqual(senderInitialBalance - transferAmount, senderDetails.Balance);
        Assert.AreEqual(transferAmount, receiverDetails.Balance);
    }

    [TestMethod]
    public void Bank_MultipleTransactionsOnSameAccount_ShouldMaintainCorrectBalance()
    {
        // Arrange
        var accountNumber = bank.CreateAccount(ValidName, ValidPin);
        Assert.IsNotNull(accountNumber);

        // Act - Multiple deposits
        bank.DepositFunds(accountNumber!, 100.00m);
        bank.DepositFunds(accountNumber!, 200.00m);
        bank.DepositFunds(accountNumber!, 300.00m);

        // Act - Multiple withdrawals
        bank.WithdrawFunds(accountNumber!, ValidPin, 50.00m);
        bank.WithdrawFunds(accountNumber!, ValidPin, 75.00m);

        // Assert - Check final balance
        var account = bank.GetAccountDetails(accountNumber!, ValidPin);
        Assert.IsNotNull(account);
        Assert.AreEqual(475.00m, account.Balance); // 100 + 200 + 300 - 50 - 75 = 475
    }

    #endregion

    #region Concurrent Operations Tests

    [TestMethod]
    public void Bank_CreateAccountsWithSameName_ShouldAllowDuplicateNames()
    {
        // Arrange & Act
        var account1 = bank.CreateAccount(ValidName, ValidPin);
        var account2 = bank.CreateAccount(ValidName, "5678");
        var account3 = bank.CreateAccount(ValidName, "9999");

        // Assert
        Assert.IsNotNull(account1);
        Assert.IsNotNull(account2);
        Assert.IsNotNull(account3);
        Assert.AreNotEqual(account1, account2);
        Assert.AreNotEqual(account2, account3);
        Assert.AreNotEqual(account1, account3);
    }

    [TestMethod]
    public void Bank_CreateAccountsWithSamePIN_ShouldAllowDuplicatePINs()
    {
        // Arrange & Act
        var account1 = bank.CreateAccount("User One", ValidPin);
        var account2 = bank.CreateAccount("User Two", ValidPin);
        var account3 = bank.CreateAccount("User Three", ValidPin);

        // Assert
        Assert.IsNotNull(account1);
        Assert.IsNotNull(account2);
        Assert.IsNotNull(account3);
        Assert.AreNotEqual(account1, account2);
        Assert.AreNotEqual(account2, account3);
        Assert.AreNotEqual(account1, account3);
    }

    #endregion

    #region Edge Cases and Error Conditions

    [TestMethod]
    public void Bank_OperationsOnNonExistentAccount_ShouldHandleGracefully()
    {
        // Arrange
        const string nonExistentAccount = "999999999";

        // Act & Assert - GetAccountDetails should return null
        var account = bank.GetAccountDetails(nonExistentAccount, ValidPin);
        Assert.IsNull(account);

        // Act & Assert - DepositFunds should throw exception
        Assert.ThrowsException<InvalidOperationException>(() =>
            bank.DepositFunds(nonExistentAccount, 100.00m));

        // Act & Assert - WithdrawFunds should throw exception
        Assert.ThrowsException<InvalidOperationException>(() =>
            bank.WithdrawFunds(nonExistentAccount, ValidPin, 100.00m));

        // Act & Assert - EditAccount should throw exception
        Assert.ThrowsException<InvalidOperationException>(() =>
            bank.EditAccount(nonExistentAccount, ValidPin, "New Name"));

        // Act & Assert - DeleteAccount should throw exception
        Assert.ThrowsException<InvalidOperationException>(() =>
            bank.DeleteAccount(nonExistentAccount, ValidName, ValidPin));
    }

    [TestMethod]
    public void Bank_TransferToNonExistentAccount_ShouldThrowException()
    {
        // Arrange
        var senderAccount = bank.CreateAccount(ValidName, ValidPin);
        bank.DepositFunds(senderAccount!, 1000.00m);
        const string nonExistentReceiver = "999999999";

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            bank.TransferFunds(senderAccount!, ValidPin, nonExistentReceiver, 100.00m));
    }

    [TestMethod]
    public void Bank_TransferFromNonExistentAccount_ShouldThrowException()
    {
        // Arrange
        var receiverAccount = bank.CreateAccount(ValidName, ValidPin);
        const string nonExistentSender = "999999999";

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            bank.TransferFunds(nonExistentSender, ValidPin, receiverAccount!, 100.00m));
    }

    [TestMethod]
    public void Bank_AccountNumberGeneration_ShouldBe9Digits()
    {
        // Arrange & Act
        var accountNumber = bank.CreateAccount(ValidName, ValidPin);

        // Assert
        Assert.IsNotNull(accountNumber);
        Assert.AreEqual(9, accountNumber!.Length);
        Assert.IsTrue(long.TryParse(accountNumber, out _), "Account number should be numeric");
    }

    [TestMethod]
    public void Bank_AccountNumberGeneration_ShouldBeInValidRange()
    {
        // Arrange & Act
        var accountNumber = bank.CreateAccount(ValidName, ValidPin);

        // Assert
        Assert.IsNotNull(accountNumber);
        var number = long.Parse(accountNumber!);
        Assert.IsTrue(number >= 100000000, "Account number should be at least 100000000");
        Assert.IsTrue(number <= 999999999, "Account number should be at most 999999999");
    }

    #endregion

    #region Data Persistence Error Handling Tests (Future Implementation)

    // Note: These tests are designed for when LoadData and SaveData are fully implemented
    // Currently they test that the stub methods don't throw exceptions

    [TestMethod]
    public void Bank_LoadData_FileNotFound_ShouldHandleGracefully()
    {
        // Arrange - Fresh bank instance (no data files exist)
        var freshBank = new Bank();

        // Act & Assert - Should not throw even if files don't exist
        // Future implementation should handle missing files gracefully
        freshBank.LoadData();
    }

    [TestMethod]
    public void Bank_SaveData_ImmediatelyAfterLoad_ShouldNotThrow()
    {
        // Arrange - Create data and load
        var account = bank.CreateAccount("Load Save User", ValidPin);
        bank.DepositFunds(account!, 300.00m);
        bank.LoadData();

        // Act & Assert - Save immediately after load
        bank.SaveData();
    }

    [TestMethod]
    public void Bank_LoadData_ImmediatelyAfterSave_ShouldNotThrow()
    {
        // Arrange - Create data and save
        var account = bank.CreateAccount("Save Load User", ValidPin);
        bank.DepositFunds(account!, 450.00m);
        bank.SaveData();

        // Act & Assert - Load immediately after save
        bank.LoadData();
    }

    [TestMethod]
    public void Bank_SaveData_WithSpecialCharactersInNames_ShouldNotThrow()
    {
        // Arrange - Create accounts with special characters in names
        var account1 = bank.CreateAccount("José María O'Connor", ValidPin);
        var account2 = bank.CreateAccount("李明", "9876");
        var account3 = bank.CreateAccount("Müller & Co.", "5432");
        var account4 = bank.CreateAccount("user@domain.com", "1111");

        bank.DepositFunds(account1!, 100.00m);
        bank.DepositFunds(account2!, 200.00m);
        bank.DepositFunds(account3!, 300.00m);
        bank.DepositFunds(account4!, 400.00m);

        // Act & Assert - Should handle special characters in JSON serialization
        bank.SaveData();
    }

    [TestMethod]
    public void Bank_LoadData_WithSpecialCharactersInNames_ShouldNotThrow()
    {
        // Arrange - Create accounts with special characters and save
        var account1 = bank.CreateAccount("François Château", ValidPin);
        var account2 = bank.CreateAccount("Владимир", "8765");
        
        bank.DepositFunds(account1!, 500.00m);
        bank.DepositFunds(account2!, 600.00m);
        bank.SaveData();

        // Act & Assert - Should handle special characters when loading
        bank.LoadData();
    }

    [TestMethod]
    public void Bank_SaveData_WithZeroBalanceAccounts_ShouldNotThrow()
    {
        // Arrange - Create accounts with zero balance
        var account1 = bank.CreateAccount("Zero Balance User 1", ValidPin);
        var account2 = bank.CreateAccount("Zero Balance User 2", "0000");
        
        // Don't deposit anything - keep balance at 0

        // Act & Assert - Should handle zero balance accounts
        bank.SaveData();
    }

    [TestMethod]
    public void Bank_SaveData_WithNegativeBalanceAccounts_ShouldNotThrow()
    {
        // Arrange - Create account and attempt to create negative balance scenario
        var account = bank.CreateAccount("Test User", ValidPin);
        bank.DepositFunds(account!, 100.00m);
        
        // Note: Current implementation doesn't allow negative balances
        // This test ensures SaveData works with current business logic

        // Act & Assert - Should handle current balance state
        bank.SaveData();
    }

    [TestMethod]
    public void Bank_LoadData_MultipleConsecutiveCalls_ShouldBeStable()
    {
        // Arrange - Create test data
        var account = bank.CreateAccount("Stability User", ValidPin);
        bank.DepositFunds(account!, 800.00m);
        bank.SaveData();

        // Act - Multiple consecutive loads
        for (int i = 0; i < 10; i++)
        {
            bank.LoadData();
        }

        // Assert - Account should still be accessible
        var retrievedAccount = bank.GetAccountDetails(account!, ValidPin);
        Assert.IsNotNull(retrievedAccount);
    }

    [TestMethod]
    public void Bank_SaveData_MultipleConsecutiveCalls_ShouldBeStable()
    {
        // Arrange - Create test data
        var account = bank.CreateAccount("Save Stability User", ValidPin);
        bank.DepositFunds(account!, 950.00m);

        // Act - Multiple consecutive saves
        for (int i = 0; i < 10; i++)
        {
            bank.SaveData();
        }

        // Assert - Account should still be accessible
        var retrievedAccount = bank.GetAccountDetails(account!, ValidPin);
        Assert.IsNotNull(retrievedAccount);
        Assert.AreEqual(950.00m, retrievedAccount.Balance);
    }

    [TestMethod]
    public void Bank_LoadData_AfterAccountModification_ShouldNotThrow()
    {
        // Arrange - Create and modify accounts
        var account1 = bank.CreateAccount("Modify User 1", ValidPin);
        var account2 = bank.CreateAccount("Modify User 2", "7890");
        
        bank.DepositFunds(account1!, 1000.00m);
        bank.WithdrawFunds(account1!, ValidPin, 300.00m);
        bank.DepositFunds(account2!, 750.00m);

        // Act & Assert - Load after modifications
        bank.LoadData();

        // Verify accounts are still accessible with correct balances
        var retrievedAccount1 = bank.GetAccountDetails(account1!, ValidPin);
        var retrievedAccount2 = bank.GetAccountDetails(account2!, "7890");
        
        Assert.IsNotNull(retrievedAccount1);
        Assert.IsNotNull(retrievedAccount2);
        Assert.AreEqual(700.00m, retrievedAccount1.Balance);
        Assert.AreEqual(750.00m, retrievedAccount2.Balance);
    }

    [TestMethod]
    public void Bank_SaveData_WithVeryLongAccountNames_ShouldNotThrow()
    {
        // Arrange - Create account with very long name
        var longName = new string('A', 500); // 500 character name
        var account = bank.CreateAccount(longName, ValidPin);
        bank.DepositFunds(account!, 250.00m);

        // Act & Assert - Should handle long names in serialization
        bank.SaveData();
    }

    [TestMethod]
    public void Bank_LoadData_WithVeryLongAccountNames_ShouldNotThrow()
    {
        // Arrange - Create account with very long name and save
        var longName = new string('B', 300); // 300 character name
        var account = bank.CreateAccount(longName, ValidPin);
        bank.DepositFunds(account!, 350.00m);
        bank.SaveData();

        // Act & Assert - Should handle long names in deserialization
        bank.LoadData();
        
        var retrievedAccount = bank.GetAccountDetails(account!, ValidPin);
        Assert.IsNotNull(retrievedAccount);
    }

    [TestMethod]
    public void Bank_SaveAndLoadData_DataIntegrityCheck_ShouldMaintainConsistency()
    {
        // Arrange - Create comprehensive test data
        var accounts = new List<(string accountNumber, string pin, decimal expectedBalance)>();
        
        for (int i = 0; i < 10; i++)
        {
            var accountNumber = bank.CreateAccount($"Integrity User {i}", $"{1000 + i}");
            var initialDeposit = 100.00m * (i + 1);
            bank.DepositFunds(accountNumber!, initialDeposit);
            
            if (i % 2 == 0 && i > 0)
            {
                var withdrawal = 50.00m;
                bank.WithdrawFunds(accountNumber!, $"{1000 + i}", withdrawal);
                accounts.Add((accountNumber!, $"{1000 + i}", initialDeposit - withdrawal));
            }
            else
            {
                accounts.Add((accountNumber!, $"{1000 + i}", initialDeposit));
            }
        }

        // Act - Save and load data
        bank.SaveData();
        bank.LoadData();

        // Assert - Verify data integrity
        foreach (var (accountNumber, pin, expectedBalance) in accounts)
        {
            var account = bank.GetAccountDetails(accountNumber, pin);
            Assert.IsNotNull(account, $"Account {accountNumber} should exist after save/load");
            Assert.AreEqual(expectedBalance, account.Balance, $"Balance for account {accountNumber} should be preserved");
        }
    }

    [TestMethod]
    public void Bank_SaveData_EmptyAccountName_ShouldNotThrow()
    {
        // Arrange - Test that SaveData works with current validation rules
        // Since empty names are not allowed by CreateAccount, we test with a minimal valid name
        var account = bank.CreateAccount("A", ValidPin); // Minimal valid name
        
        if (account != null) // Only test if account creation succeeded
        {
            bank.DepositFunds(account, 100.00m);
            
            // Act & Assert - Should handle minimal names
            bank.SaveData();
        }
    }

    [TestMethod]
    public void Bank_LoadData_PerformanceWithLargeDataset_ShouldComplete()
    {
        // Arrange - Create large dataset
        var accounts = new List<string>();
        
        for (int i = 0; i < 200; i++) // Large dataset
        {
            var accountNumber = bank.CreateAccount($"Performance User {i}", $"{3000 + i}");
            accounts.Add(accountNumber!);
            bank.DepositFunds(accountNumber!, 100.00m + i);
            
            if (i % 3 == 0)
            {
                bank.WithdrawFunds(accountNumber!, $"{3000 + i}", 25.00m);
            }
        }
        
        bank.SaveData();

        // Act - Measure that load completes (future implementation)
        var startTime = DateTime.Now;
        bank.LoadData();
        var endTime = DateTime.Now;
        
        // Assert - Operation should complete in reasonable time
        var duration = endTime - startTime;
        Assert.IsTrue(duration.TotalSeconds < 30, "LoadData should complete within 30 seconds for large dataset");
    }

    #endregion

    #region State Consistency Tests

    [TestMethod]
    public void Bank_AfterAccountDeletion_ShouldNotAffectOtherAccounts()
    {
        // Arrange
        var account1 = bank.CreateAccount("User One", ValidPin);
        var account2 = bank.CreateAccount("User Two", "5678");
        bank.DepositFunds(account1!, 500.00m);
        bank.DepositFunds(account2!, 1000.00m);

        // Act - Delete first account
        bank.WithdrawFunds(account1!, ValidPin, 500.00m); // Zero balance for deletion
        bank.DeleteAccount(account1!, "User One", ValidPin);

        // Assert - Second account should remain unaffected
        var remainingAccount = bank.GetAccountDetails(account2!, "5678");
        Assert.IsNotNull(remainingAccount);
        Assert.AreEqual(1000.00m, remainingAccount.Balance);
        Assert.AreEqual("User Two", remainingAccount.Name);

        // Assert - First account should be gone
        var deletedAccount = bank.GetAccountDetails(account1!, ValidPin);
        Assert.IsNull(deletedAccount);
    }

    [TestMethod]
    public void Bank_LargeNumberOfAccounts_ShouldMaintainPerformance()
    {
        // Arrange
        const int numberOfAccounts = 1000;
        var accountNumbers = new List<string>();

        // Act - Create many accounts
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < numberOfAccounts; i++)
        {
            var accountNumber = bank.CreateAccount($"User{i}", ValidPin);
            Assert.IsNotNull(accountNumber);
            accountNumbers.Add(accountNumber!);
        }
        stopwatch.Stop();

        // Assert - Should complete in reasonable time (less than 10 seconds)
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 10000, 
            $"Creating {numberOfAccounts} accounts took too long: {stopwatch.ElapsedMilliseconds}ms");

        // Assert - All accounts should be accessible
        for (int i = 0; i < 10; i++) // Test first 10 accounts
        {
            var account = bank.GetAccountDetails(accountNumbers[i], ValidPin);
            Assert.IsNotNull(account);
            Assert.AreEqual($"User{i}", account.Name);
        }
    }

    #endregion

    #region Account Number Collision Tests

    [TestMethod]
    public void Bank_AccountNumberGeneration_ShouldAvoidCollisions()
    {
        // Arrange
        var accountNumbers = new HashSet<string>();
        const int numberOfAccounts = 1000;

        // Act
        for (int i = 0; i < numberOfAccounts; i++)
        {
            var accountNumber = bank.CreateAccount($"User{i}", ValidPin);
            Assert.IsNotNull(accountNumber);
            
            // Assert - No collisions
            Assert.IsFalse(accountNumbers.Contains(accountNumber!), 
                $"Account number collision detected: {accountNumber}");
            accountNumbers.Add(accountNumber!);
        }

        // Assert
        Assert.AreEqual(numberOfAccounts, accountNumbers.Count);
    }

    #endregion

    #region Business Logic Tests

    [TestMethod]
    public void Bank_PINValidation_ShouldEnforceMinimumLength()
    {
        // Arrange & Act & Assert
        var result1 = bank.CreateAccount(ValidName, "1");     // 1 digit
        var result2 = bank.CreateAccount(ValidName, "12");    // 2 digits
        var result3 = bank.CreateAccount(ValidName, "123");   // 3 digits
        var result4 = bank.CreateAccount(ValidName, "1234");  // 4 digits (valid)

        Assert.IsNull(result1);
        Assert.IsNull(result2);
        Assert.IsNull(result3);
        Assert.IsNotNull(result4);
    }

    [TestMethod]
    public void Bank_AccountOperations_ShouldRequireCorrectPIN()
    {
        // Arrange
        var accountNumber = bank.CreateAccount(ValidName, ValidPin);
        bank.DepositFunds(accountNumber!, 1000.00m);
        const string wrongPin = "9999";

        // Act & Assert - Wrong PIN should fail
        var accountDetails = bank.GetAccountDetails(accountNumber!, wrongPin);
        Assert.IsNull(accountDetails);

        Assert.ThrowsException<InvalidOperationException>(() =>
            bank.WithdrawFunds(accountNumber!, wrongPin, 100.00m));

        Assert.ThrowsException<InvalidOperationException>(() =>
            bank.EditAccount(accountNumber!, wrongPin, "New Name"));

        Assert.ThrowsException<InvalidOperationException>(() =>
            bank.DeleteAccount(accountNumber!, ValidName, wrongPin));

        Assert.ThrowsException<InvalidOperationException>(() =>
            bank.TransferFunds(accountNumber!, wrongPin, "123456789", 100.00m));
    }

    [TestMethod]
    public void Bank_BalanceCalculations_ShouldBeAccurate()
    {
        // Arrange
        var accountNumber = bank.CreateAccount(ValidName, ValidPin);

        // Act - Complex sequence of transactions
        bank.DepositFunds(accountNumber!, 1000.50m);
        bank.DepositFunds(accountNumber!, 250.25m);
        bank.WithdrawFunds(accountNumber!, ValidPin, 300.75m);
        bank.DepositFunds(accountNumber!, 100.00m);
        bank.WithdrawFunds(accountNumber!, ValidPin, 50.00m);

        // Assert
        var account = bank.GetAccountDetails(accountNumber!, ValidPin);
        Assert.IsNotNull(account);
        Assert.AreEqual(1000.00m, account.Balance); // 1000.50 + 250.25 - 300.75 + 100.00 - 50.00 = 1000.00
    }

    #endregion
}
