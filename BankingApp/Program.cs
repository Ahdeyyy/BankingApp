using System;
using System.IO; // For Directory.CreateDirectory


namespace BankingApp
{
public class Program
{
    public static void Main(string[] args)
    {
        // Ensure the data directory exists before trying to load/save
        string dataDirectory = "data";
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }

        Bank bank = new Bank();


        try
        {
            bank.LoadData();
            Console.WriteLine("Bank data loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading bank data: {ex.Message}");
            Console.WriteLine("Starting with an empty bank data.");
        }

        
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n--- CLI Banking Application ---");
            Console.WriteLine("1. Create New Account");
            Console.WriteLine("2. Edit Account Name");
            Console.WriteLine("3. Delete Account");
            Console.WriteLine("4. Deposit Funds");
            Console.WriteLine("5. Withdraw Funds");
            Console.WriteLine("6. Transfer Funds");
            Console.WriteLine("7. View Account Details");
            Console.WriteLine("8. Exit");
            Console.Write("Enter your choice: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateNewAccount(bank);
                    break;
                
                case "2":
                    EditAccountName(bank);
                    break;
                
                case "3":
                    DeleteAccount(bank);
                    break;
                
                case "4":
                    DepositFunds(bank);
                    break;
                
                case "5":
                    WithdrawFunds(bank);
                    break;
                
                case "6":
                    TransferFunds(bank);
                    break;
                
                case "7":
                    ViewAccountDetails(bank);
                    break;
                
                case "8":
                    running = false;
                    break;
                
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

        
            try
            {
                bank.SaveData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }
        
        Console.WriteLine("Application exiting. Goodbye!");
    }

    private static void CreateNewAccount(Bank bank)
    {
        Console.WriteLine("\n--- Create New Account ---");
        Console.Write("Enter account holder name: ");
        string? name = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Error: Name cannot be empty.");
            return;
        }

        Console.Write("Enter a 4-digit PIN: ");
        string? pin = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(pin))
        {
            Console.WriteLine("Error: PIN cannot be empty.");
            return;
        }

        try
        {
            string? accountNumber = bank.CreateAccount(name, pin);
            if (accountNumber != null)
            {
                Console.WriteLine($"Account created successfully! Account Number: {accountNumber}");
            }
            else
            {
                Console.WriteLine("Failed to create account. Please ensure PIN is at least 4 digits.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating account: {ex.Message}");
        }
    }

    private static void EditAccountName(Bank bank)
    {
        Console.WriteLine("\n--- Edit Account Name ---");
        Console.Write("Enter account number: ");
        string? accountNumber = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            Console.WriteLine("Error: Account number cannot be empty.");
            return;
        }

        Console.Write("Enter PIN: ");
        string? pin = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(pin))
        {
            Console.WriteLine("Error: PIN cannot be empty.");
            return;
        }

        Console.Write("Enter new name: ");
        string? newName = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(newName))
        {
            Console.WriteLine("Error: New name cannot be empty.");
            return;
        }

        try
        {
            bool success = bank.EditAccount(accountNumber, pin, newName);
            if (success)
            {
                Console.WriteLine("Account name updated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to update account name.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating account: {ex.Message}");
        }
    }

    private static void DeleteAccount(Bank bank)
    {
        Console.WriteLine("\n--- Delete Account ---");
        Console.Write("Enter account number: ");
        string? accountNumber = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            Console.WriteLine("Error: Account number cannot be empty.");
            return;
        }

        Console.Write("Enter account holder name: ");
        string? name = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Error: Name cannot be empty.");
            return;
        }

        Console.Write("Enter PIN: ");
        string? pin = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(pin))
        {
            Console.WriteLine("Error: PIN cannot be empty.");
            return;
        }

        try
        {
            // First check the account details and balance
            var account = bank.GetAccountDetails(accountNumber, pin);
            if (account == null)
            {
                Console.WriteLine("Account not found or incorrect PIN.");
                return;
            }

            if (account.Balance != 0)
            {
                Console.WriteLine($"Cannot delete account with non-zero balance. Current balance: ${account.Balance:F2}");
                Console.WriteLine("Please withdraw all funds before deleting the account.");
                return;
            }

            Console.Write($"Are you sure you want to delete the account for {account.Name}? (y/N): ");
            string? confirmation = Console.ReadLine();
            
            if (confirmation?.ToLower() == "y" || confirmation?.ToLower() == "yes")
            {
                bool success = bank.DeleteAccount(accountNumber, name, pin);
                if (success)
                {
                    Console.WriteLine("Account deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to delete account.");
                }
            }
            else
            {
                Console.WriteLine("Account deletion cancelled.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting account: {ex.Message}");
        }
    }

    private static void DepositFunds(Bank bank)
    {
        Console.WriteLine("\n--- Deposit Funds ---");
        Console.Write("Enter account number: ");
        string? accountNumber = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            Console.WriteLine("Error: Account number cannot be empty.");
            return;
        }

        Console.Write("Enter amount to deposit: $");
        string? amountInput = Console.ReadLine();
        
        if (!decimal.TryParse(amountInput, out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Error: Please enter a valid positive amount.");
            return;
        }

        try
        {
            bool success = bank.DepositFunds(accountNumber, amount);
            if (success)
            {
                Console.WriteLine($"Successfully deposited ${amount:F2}!");
            }
            else
            {
                Console.WriteLine("Failed to deposit funds.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error depositing funds: {ex.Message}");
        }
    }

    private static void WithdrawFunds(Bank bank)
    {
        Console.WriteLine("\n--- Withdraw Funds ---");
        Console.Write("Enter account number: ");
        string? accountNumber = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            Console.WriteLine("Error: Account number cannot be empty.");
            return;
        }

        Console.Write("Enter PIN: ");
        string? pin = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(pin))
        {
            Console.WriteLine("Error: PIN cannot be empty.");
            return;
        }

        Console.Write("Enter amount to withdraw: $");
        string? amountInput = Console.ReadLine();
        
        if (!decimal.TryParse(amountInput, out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Error: Please enter a valid positive amount.");
            return;
        }

        try
        {
            bool success = bank.WithdrawFunds(accountNumber, pin, amount);
            if (success)
            {
                Console.WriteLine($"Successfully withdrew ${amount:F2}!");
            }
            else
            {
                Console.WriteLine("Failed to withdraw funds.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error withdrawing funds: {ex.Message}");
        }
    }

    private static void TransferFunds(Bank bank)
    {
        Console.WriteLine("\n--- Transfer Funds ---");
        Console.Write("Enter sender account number: ");
        string? senderAccount = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(senderAccount))
        {
            Console.WriteLine("Error: Sender account number cannot be empty.");
            return;
        }

        Console.Write("Enter sender PIN: ");
        string? pin = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(pin))
        {
            Console.WriteLine("Error: PIN cannot be empty.");
            return;
        }

        Console.Write("Enter receiver account number: ");
        string? receiverAccount = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(receiverAccount))
        {
            Console.WriteLine("Error: Receiver account number cannot be empty.");
            return;
        }

        if (senderAccount == receiverAccount)
        {
            Console.WriteLine("Error: Cannot transfer to the same account.");
            return;
        }

        Console.Write("Enter amount to transfer: $");
        string? amountInput = Console.ReadLine();
        
        if (!decimal.TryParse(amountInput, out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Error: Please enter a valid positive amount.");
            return;
        }

        try
        {
            bool success = bank.TransferFunds(senderAccount, pin, receiverAccount, amount);
            if (success)
            {
                Console.WriteLine($"Successfully transferred ${amount:F2} from {senderAccount} to {receiverAccount}!");
            }
            else
            {
                Console.WriteLine("Failed to transfer funds.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error transferring funds: {ex.Message}");
        }
    }

    private static void ViewAccountDetails(Bank bank)
    {
        Console.WriteLine("\n--- View Account Details ---");
        Console.Write("Enter account number: ");
        string? accountNumber = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            Console.WriteLine("Error: Account number cannot be empty.");
            return;
        }

        Console.Write("Enter PIN: ");
        string? pin = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(pin))
        {
            Console.WriteLine("Error: PIN cannot be empty.");
            return;
        }

        try
        {
            Account? account = bank.GetAccountDetails(accountNumber, pin);
            if (account != null)
            {
                Console.WriteLine("\n--- Account Details ---");
                Console.WriteLine($"Account Number: {account.AccountNumber}");
                Console.WriteLine($"Account Holder: {account.Name}");
                Console.WriteLine($"Current Balance: ${account.Balance:F2}");
            }
            else
            {
                Console.WriteLine("Account not found or incorrect PIN.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving account details: {ex.Message}");
        }
    }
}
}