using System.Text.Json;

namespace BankingApp
{
    public class Bank
    {
        // Internal lists to hold accounts and transactions in memory
        private List<Account> accounts;
        private List<Transaction> transactions;

        // File paths for storing data (relative to executable)
        private const string AccountsFilePath = "data/accounts.json";
        private const string TransactionsFilePath = "data/transactions.json";

        /// <summary>
        /// Initializes a new instance of the Bank class.
        /// Sets up the internal data structures.
        /// </summary>
        public Bank()
        {
            accounts = new List<Account>();
            transactions = new List<Transaction>();
        }

        /// <summary>
        /// Loads account and transaction data from JSON files into memory.
        /// </summary>
        /// <remarks>
        /// Requirements:
        /// - The 'data' directory must exist.
        /// - JSON files should exist and be valid.
        /// </remarks>
        /// <exception cref="IOException">Thrown if there are issues reading the files (e.g., file not found, permission denied).</exception>
        /// <exception cref="System.Text.Json.JsonException">Thrown if the JSON data is malformed and cannot be deserialized. (This implies using System.Text.Json library).</exception>
        public void LoadData(string accountsFilePath = AccountsFilePath, string transactionsFilePath = TransactionsFilePath)
        {
            try
            {
                // Load accounts with retry logic
                if (File.Exists(accountsFilePath))
                {
                    var loadedAccounts = LoadWithRetry<List<Account>>(accountsFilePath);
                    if (loadedAccounts != null)
                    {
                        accounts = loadedAccounts;
                    }
                }

                // Load transactions with retry logic
                if (File.Exists(transactionsFilePath))
                {
                    var loadedTransactions = LoadWithRetry<List<Transaction>>(transactionsFilePath);
                    if (loadedTransactions != null)
                    {
                        transactions = loadedTransactions;
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                throw new IOException("The 'data' directory does not exist.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new IOException("Permission denied when accessing data files.");
            }
            catch (System.Text.Json.JsonException)
            {
                throw new JsonException("Malformed JSON file") ;
            }
            catch (Exception ex)
            {
                throw new IOException($"Error reading data files: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Helper method to load data with retry logic for file access conflicts.
        /// </summary>
        private T? LoadWithRetry<T>(string filePath, int maxRetries = 3)
        {
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(fileStream))
                    {
                        string json = reader.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
                        }
                    }
                    return default(T);
                }
                catch (IOException) when (attempt < maxRetries - 1)
                {
                    // Wait a bit before retrying with debounce logic
                    Thread.Sleep(50 * (attempt + 1));
                }
            }
            return default(T);
        }

        /// <summary>
        /// Saves current account and transaction data from memory to JSON files.
        /// </summary>
        /// <remarks>
        /// Requirements:
        /// - The 'data' directory must exist and be writable.
        /// </remarks>
        /// <exception cref="IOException">Thrown if there are issues writing to the files (e.g., permission denied, disk full).</exception>
        public void SaveData(string accountsFilePath = AccountsFilePath, string transactionsFilePath = TransactionsFilePath)
        {
            try
            {
                
                string? dataDirectory = Path.GetDirectoryName(accountsFilePath);
                if (!string.IsNullOrEmpty(dataDirectory) && !Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }

                // Save accounts with retry logic
                SaveWithRetry(accountsFilePath, accounts);

                // Save transactions with retry logic
                SaveWithRetry(transactionsFilePath, transactions);
            }
            catch (DirectoryNotFoundException)
            {
                throw new IOException("Unable to create or access the 'data' directory.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new IOException("Permission denied when writing to data files.");
            }
            catch (IOException)
            {
                throw; 
            }
            catch (Exception ex)
            {
                throw new IOException($"Error writing data files: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Helper method to save data with retry logic for file access conflicts.
        /// </summary>
        private void SaveWithRetry<T>(string filePath, T data, int maxRetries = 3)
        {
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                    string json = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                    
                    // Use FileShare.Read to allow other processes to read while we write
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    using (var writer = new StreamWriter(fileStream))
                    {
                        writer.Write(json);
                    }
                    return; 
                }
                catch (IOException) when (attempt < maxRetries - 1)
                {
                    // Wait a bit before retrying with debounce
                    Thread.Sleep(50 * (attempt + 1)); 
                }
            }
        }

        /// <summary>
        /// Creates a new bank account with the provided details.
        /// </summary>
        /// <param name="name">The name of the account holder.</param>
        /// <param name="pin">The PIN for the new account.</param>
        /// <returns>The generated account number if successful, null if PIN is too short.</returns>
        /// <remarks>
        /// Requirements:
        /// - Name and pin must not be null or empty.
        /// - Pin should have a minimum length (e.g., 4 characters).
        /// - A random, unique 11-digit account number will be generated.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if name or pin are invalid.</exception>
        public string? CreateAccount(string name, string pin)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be null or empty");
            
            if (string.IsNullOrEmpty(pin))
                throw new ArgumentException("PIN cannot be null or empty");
            
            if (pin.Length < 4)
                return null;
            
            string accountNumber = GenerateUniqueAccountNumber();
            var account = new Account(name, accountNumber, pin, 0);
            accounts.Add(account);
            return accountNumber;
        }


/// <summary>
/// Generates a unique 11-digit account number
/// </summary>
/// <returns></returns>
        private string GenerateUniqueAccountNumber()
        {
            Random random = new Random();
            const string chars = "0123456789";
            string accountNumber;

            do
            {
                accountNumber = new string([.. Enumerable.Repeat(chars, 11).Select(s => s[random.Next(s.Length)])]);
            } while (accounts.Any(a => a.AccountNumber == accountNumber));

            return accountNumber;
        }

        /// <summary>
        /// Edits the name of an existing account.
        /// </summary>
        /// <param name="accountNumber">The account number of the account to edit.</param>
        /// <param name="oldPin">The current PIN of the account for verification.</param>
        /// <param name="newName">The new name for the account holder.</param>
        /// <returns>True if the account name was updated successfully, false otherwise.</returns>
        /// <remarks>
        /// Requirements:
        /// - Account with matching accountNumber must exist.
        /// - Provided oldPin must match the account's current PIN.
        /// - newName must not be null or empty.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if accountNumber, oldPin, or newName are invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the account is not found, or the PIN does not match.</exception>
        public bool EditAccount(string accountNumber, string oldPin, string newName)
        {
            if (string.IsNullOrEmpty(accountNumber))
                throw new ArgumentException("Account number cannot be null or empty");
            if (accountNumber.Length != 11)
                throw new ArgumentException("Account number must be 11 digits long");
            
            if (string.IsNullOrEmpty(oldPin))
                throw new ArgumentException("PIN cannot be null or empty");
            
            if (string.IsNullOrEmpty(newName))
                throw new ArgumentException("New name cannot be null or empty");
            
            var account = accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
                throw new InvalidOperationException("Account not found");
            
            if (account.Pin != oldPin)
                throw new InvalidOperationException("PIN does not match");
            
            account.Name = newName;
            return true;
        }

        /// <summary>
        /// Deletes an existing bank account.
        /// </summary>
        /// <param name="accountNumber">The account number of the account to delete.</param>
        /// <param name="name">The name of the account holder for verification.</param>
        /// <param name="pin">The PIN of the account for verification.</param>
        /// <returns>True if the account was deleted successfully, false otherwise.</returns>
        /// <remarks>
        /// Requirements:
        /// - Account with matching accountNumber must exist.
        /// - Provided name and pin must match the account's details.
        /// - Account balance must be zero before deletion.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if accountNumber, name, or pin are invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the account is not found, details mismatch, or the balance is not zero.</exception>
        public bool DeleteAccount(string accountNumber, string name, string pin)
        {
            if (string.IsNullOrEmpty(accountNumber))
                throw new ArgumentException("Account number cannot be null or empty");
            if (accountNumber.Length != 11)
                throw new ArgumentException("Account number must be 11 digits long");
            
            if (string.IsNullOrEmpty(name))
                    throw new ArgumentException("Name cannot be null or empty");
            
            if (string.IsNullOrEmpty(pin))
                throw new ArgumentException("PIN cannot be null or empty");
            
            var account = accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
                throw new InvalidOperationException("Account not found");
            
            if (account.Name != name)
                throw new InvalidOperationException("Name does not match");
            
            if (account.Pin != pin)
                throw new InvalidOperationException("PIN does not match");
            
            if (account.Balance != 0)
                throw new InvalidOperationException("Account balance must be zero before deletion");
            
            accounts.Remove(account);
            return true;
        }

        /// <summary>
        /// Transfers funds from one account to another.
        /// </summary>
        /// <param name="senderAccountNumber">The account number of the sender.</param>
        /// <param name="senderPin">The PIN of the sender's account for verification.</param>
        /// <param name="receiverAccountNumber">The account number of the recipient.</param>
        /// <param name="amount">The amount of funds to transfer.</param>
        /// <returns>True if the transfer was successful, false otherwise.</returns>
        /// <remarks>
        /// Requirements:
        /// - Both sender and receiver accounts must exist.
        /// - senderAccountNumber and receiverAccountNumber must be different.
        /// - senderPin must match the sender's account PIN.
        /// - Amount must be positive.
        /// - Sender must have sufficient balance for the transfer.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if any input parameters are invalid (e.g., amount not positive, account numbers are the same).</exception>
        /// <exception cref="InvalidOperationException">Thrown if sender/receiver account not found, PIN mismatch, or insufficient funds.</exception>
        public bool TransferFunds(string senderAccountNumber, string senderPin, string receiverAccountNumber, decimal amount)
        {
            if (string.IsNullOrEmpty(senderAccountNumber))
                throw new ArgumentException("Sender account number cannot be null or empty");
            if (senderAccountNumber.Length != 11)
                throw new ArgumentException("Sender account number must be 11 digits long");
            
            if (string.IsNullOrEmpty(senderPin))
                    throw new ArgumentException("Sender PIN cannot be null or empty");
            
            if (string.IsNullOrEmpty(receiverAccountNumber))
                throw new ArgumentException("Receiver account number cannot be null or empty");
            if (receiverAccountNumber.Length != 11)
                throw new ArgumentException("Receiver account number must be 11 digits long");
            
            if (senderAccountNumber == receiverAccountNumber)
                    throw new ArgumentException("Sender and receiver account numbers must be different");
            
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive");
            
            var senderAccount = accounts.FirstOrDefault(a => a.AccountNumber == senderAccountNumber);
            if (senderAccount == null)
                throw new InvalidOperationException("Sender account not found");
            
            if (senderAccount.Pin != senderPin)
                throw new InvalidOperationException("Sender PIN does not match");
            
            var receiverAccount = accounts.FirstOrDefault(a => a.AccountNumber == receiverAccountNumber);
            if (receiverAccount == null)
                throw new InvalidOperationException("Receiver account not found");
            
            if (senderAccount.Balance < amount)
                throw new InvalidOperationException("Insufficient funds");
            
            senderAccount.Balance -= amount;
            receiverAccount.Balance += amount;
            
            Transaction transaction = new Transaction(
                Guid.NewGuid().ToString(),
                senderAccountNumber,
                TransactionType.Transfer,
                amount,
                DateTime.Now,
                receiverAccountNumber
            );
            transactions.Add(transaction);

            return true;
        }

        /// <summary>
        /// Deposits funds into a specified account.
        /// </summary>
        /// <param name="accountNumber">The account number to deposit into.</param>
        /// <param name="amount">The amount to deposit.</param>
        /// <returns>True if the deposit was successful, false otherwise.</returns>
        /// <remarks>
        /// Requirements:
        /// - Account with matching accountNumber must exist.
        /// - Amount must be positive.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if accountNumber is invalid or amount is not positive.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the account is not found.</exception>
        public bool DepositFunds(string accountNumber, decimal amount)
        {
            if (string.IsNullOrEmpty(accountNumber))
                throw new ArgumentException("Account number cannot be null or empty");
            if (accountNumber.Length != 11)
                throw new ArgumentException("Account number must be 11 digits long");
            
            if (amount <= 0)
                    throw new ArgumentException("Amount must be positive");
            
            var account = accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
                throw new InvalidOperationException("Account not found");
            
            account.Balance += amount;
            
            Transaction transaction = new Transaction(
                Guid.NewGuid().ToString(),
                accountNumber,
                TransactionType.Deposit,
                amount,
                DateTime.Now,
                null
            );

            transactions.Add(transaction); 

            return true;
        }

        /// <summary>
        /// Withdraws funds from a specified account.
        /// </summary>
        /// <param name="accountNumber">The account number to withdraw from.</param>
        /// <param name="pin">The PIN of the account for verification.</param>
        /// <param name="amount">The amount to withdraw.</param>
        /// <returns>True if the withdrawal was successful, false otherwise.</returns>
        /// <remarks>
        /// Requirements:
        /// - Account with matching accountNumber must exist.
        /// - Provided pin must match the account's PIN.
        /// - Amount must be positive.
        /// - Account must have sufficient balance for the withdrawal.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if accountNumber is invalid, pin is invalid, or amount is not positive.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the account is not found, PIN mismatch, or insufficient funds.</exception>
        public bool WithdrawFunds(string accountNumber, string pin, decimal amount)
        {
            if (string.IsNullOrEmpty(accountNumber))
                throw new ArgumentException("Account number cannot be null or empty");
            if (accountNumber.Length != 11)
                throw new ArgumentException("Account number must be 11 digits long");
            
            if (string.IsNullOrEmpty(pin))
                    throw new ArgumentException("PIN cannot be null or empty");
            
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive");
            
            var account = accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
                throw new InvalidOperationException("Account not found");
            
            if (account.Pin != pin)
                throw new InvalidOperationException("PIN does not match");
            
            if (account.Balance < amount)
                throw new InvalidOperationException("Insufficient funds");
            
            account.Balance -= amount;

            Transaction transaction = new Transaction(
                Guid.NewGuid().ToString(),
                accountNumber,
                TransactionType.Withdrawal,
                amount,
                DateTime.Now,
                null
            );

            transactions.Add(transaction);

            return true;
        }

        /// <summary>
        /// Retrieves an Account object based on account number and PIN for display or internal use.
        /// </summary>
        /// <param name="accountNumber">The account number to look up.</param>
        /// <param name="pin">The PIN of the account for verification.</param>
        /// <returns>The Account object if found and PIN matches; otherwise, returns null.</returns>
        /// <remarks>
        /// Requirements:
        /// - Account with matching accountNumber must exist.
        /// - Provided pin must match the account's PIN.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if accountNumber or pin are invalid.</exception>
        public Account? GetAccountDetails(string accountNumber, string pin)
        {
            if (string.IsNullOrEmpty(accountNumber))
                throw new ArgumentException("Account number cannot be null or empty");
            if (accountNumber.Length != 11)
                throw new ArgumentException("Account number must be 11 digits long");
            if (string.IsNullOrEmpty(pin))
                throw new ArgumentException("PIN cannot be null or empty");
            
            var account = accounts.FirstOrDefault(a => a.AccountNumber == accountNumber && a.Pin == pin);
            return account;
        }
    }
}