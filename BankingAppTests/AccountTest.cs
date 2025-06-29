using BankingApp;

namespace BankingAppTests;

[TestClass]
public sealed class AccountTest
{
    private Bank bank;
    private const string ValidAccountNumber = "12345678900";
    private const string ValidName = "John Doe";
    private const string ValidPin = "1234";
    private const decimal ValidBalance = 1000.00m;

    [TestInitialize]
    public void Setup()
    {
        bank = new Bank();
    }

    #region Account Constructor Tests

    [TestMethod]
    public void Account_Constructor_ValidParameters_ShouldCreateAccount()
    {
        // Arrange & Act
        var account = new Account(ValidName, ValidAccountNumber, ValidPin, ValidBalance);

        // Assert
        Assert.AreEqual(ValidName, account.Name);
        Assert.AreEqual(ValidAccountNumber, account.AccountNumber);
        Assert.AreEqual(ValidPin, account.Pin);
        Assert.AreEqual(ValidBalance, account.Balance);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Account_Constructor_NullName_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var account = new Account(null, ValidAccountNumber, ValidPin, ValidBalance);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Account_Constructor_EmptyName_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var account = new Account("", ValidAccountNumber, ValidPin, ValidBalance);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Account_Constructor_NullAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var account = new Account(ValidName, null, ValidPin, ValidBalance);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Account_Constructor_EmptyAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var account = new Account(ValidName, "", ValidPin, ValidBalance);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Account_Constructor_InvalidAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert (account number should be 11 digits)
        var account = new Account(ValidName, "123456", ValidPin, ValidBalance);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Account_Constructor_NullPin_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var account = new Account(ValidName, ValidAccountNumber, null, ValidBalance);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Account_Constructor_EmptyPin_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var account = new Account(ValidName, ValidAccountNumber, "", ValidBalance);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Account_Constructor_NegativeBalance_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var account = new Account(ValidName, ValidAccountNumber, ValidPin, -100.00m);
    }

    #endregion

    #region Bank CreateAccount Tests

    [TestMethod]
    public void Bank_CreateAccount_ValidParameters_ShouldReturnTrue()
    {
        // Arrange & Act
        string? result = bank.CreateAccount(ValidName, ValidPin);

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_CreateAccount_NullName_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.CreateAccount(null, ValidPin);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_CreateAccount_EmptyName_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.CreateAccount("", ValidPin);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_CreateAccount_NullPin_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.CreateAccount(ValidName, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_CreateAccount_EmptyPin_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.CreateAccount(ValidName, "");
    }

    [TestMethod]
    public void Bank_CreateAccount_ShortPin_ShouldReturnFalse()
    {
        // Arrange & Act (PIN should be at least 4 characters)
        string? result = bank.CreateAccount(ValidName, "123");

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region Bank EditAccount Tests

    [TestMethod]
    public void Bank_EditAccount_ValidParameters_ShouldReturnTrue()
    {
        // Arrange
        string? accountNumber = bank.CreateAccount(ValidName, ValidPin);
        string newName = "Jane Doe";

        // Act
        bool result = bank.EditAccount(accountNumber!, ValidPin, newName);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_EditAccount_NullAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.EditAccount(null, ValidPin, "New Name");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_EditAccount_EmptyAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.EditAccount("", ValidPin, "New Name");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_EditAccount_NullPin_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.EditAccount(ValidAccountNumber, null, "New Name");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_EditAccount_EmptyPin_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.EditAccount(ValidAccountNumber, "", "New Name");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_EditAccount_NullNewName_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.EditAccount(ValidAccountNumber, ValidPin, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_EditAccount_EmptyNewName_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.EditAccount(ValidAccountNumber, ValidPin, "");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bank_EditAccount_NonExistentAccount_ShouldThrowInvalidOperationException()
    {
        // Arrange & Act & Assert
        bank.EditAccount("99999999999", ValidPin, "New Name");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bank_EditAccount_WrongPin_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string? accountNumber = bank.CreateAccount(ValidName, ValidPin);

        // Act & Assert
        bank.EditAccount(accountNumber!, "9999", "New Name");
    }

    #endregion

    #region Bank DeleteAccount Tests

    [TestMethod]
    public void Bank_DeleteAccount_ValidParameters_ShouldReturnTrue()
    {
        // Arrange
        string? accountNumber = bank.CreateAccount(ValidName, ValidPin);

        // Act
        bool result = bank.DeleteAccount(accountNumber!, ValidName, ValidPin);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_DeleteAccount_NullAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.DeleteAccount(null, ValidName, ValidPin);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_DeleteAccount_EmptyAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.DeleteAccount("", ValidName, ValidPin);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_DeleteAccount_NullName_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.DeleteAccount(ValidAccountNumber, null, ValidPin);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_DeleteAccount_EmptyName_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.DeleteAccount(ValidAccountNumber, "", ValidPin);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_DeleteAccount_NullPin_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.DeleteAccount(ValidAccountNumber, ValidName, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_DeleteAccount_EmptyPin_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.DeleteAccount(ValidAccountNumber, ValidName, "");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bank_DeleteAccount_NonExistentAccount_ShouldThrowInvalidOperationException()
    {
        // Arrange & Act & Assert
        bank.DeleteAccount("99999999999", ValidName, ValidPin);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bank_DeleteAccount_WrongName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string? accountNumber = bank.CreateAccount(ValidName, ValidPin);

        // Act & Assert
        bank.DeleteAccount(accountNumber!, "Wrong Name", ValidPin);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bank_DeleteAccount_WrongPin_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string? accountNumber = bank.CreateAccount(ValidName, ValidPin);

        // Act & Assert
        bank.DeleteAccount(accountNumber!, ValidName, "9999");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bank_DeleteAccount_NonZeroBalance_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string? accountNumber = bank.CreateAccount(ValidName, ValidPin);
        bank.DepositFunds(accountNumber!, 100.00m);

        // Act & Assert
        bank.DeleteAccount(accountNumber!, ValidName, ValidPin);
    }

    #endregion

    #region Bank GetAccountDetails Tests

    [TestMethod]
    public void Bank_GetAccountDetails_ValidParameters_ShouldReturnAccount()
    {
        // Arrange
        string? accountNumber = bank.CreateAccount(ValidName, ValidPin);

        // Act
        Account result = bank.GetAccountDetails(accountNumber!, ValidPin);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(ValidName, result.Name);
        Assert.AreEqual(accountNumber, result.AccountNumber);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_GetAccountDetails_NullAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.GetAccountDetails(null, ValidPin);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_GetAccountDetails_EmptyAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.GetAccountDetails("", ValidPin);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_GetAccountDetails_NullPin_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.GetAccountDetails(ValidAccountNumber, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_GetAccountDetails_EmptyPin_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.GetAccountDetails(ValidAccountNumber, "");
    }

    [TestMethod]
    public void Bank_GetAccountDetails_NonExistentAccount_ShouldReturnNull()
    {
        // Arrange & Act
        Account result = bank.GetAccountDetails("99999999999", ValidPin);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Bank_GetAccountDetails_WrongPin_ShouldReturnNull()
    {
        // Arrange
        string? accountNumber = bank.CreateAccount(ValidName, ValidPin);

        // Act
        Account result = bank.GetAccountDetails(accountNumber!, "9999");

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region Bank DepositFunds Tests

    [TestMethod]
    public void Bank_DepositFunds_ValidParameters_ShouldReturnTrue()
    {
        // Arrange
        string? accountNumber = bank.CreateAccount(ValidName, ValidPin);

        // Act
        bool result = bank.DepositFunds(accountNumber!, 500.00m);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_DepositFunds_NullAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.DepositFunds(null, 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_DepositFunds_EmptyAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.DepositFunds("", 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_DepositFunds_NegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.DepositFunds(ValidAccountNumber, -100.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_DepositFunds_ZeroAmount_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.DepositFunds(ValidAccountNumber, 0.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bank_DepositFunds_NonExistentAccount_ShouldThrowInvalidOperationException()
    {
        // Arrange & Act & Assert
        bank.DepositFunds("99999999999", 500.00m);
    }

    #endregion

    #region Bank WithdrawFunds Tests

    [TestMethod]
    public void Bank_WithdrawFunds_ValidParameters_ShouldReturnTrue()
    {
        // Arrange
        string? accountNumber = bank.CreateAccount(ValidName, ValidPin);
        bank.DepositFunds(accountNumber!, 1000.00m);

        // Act
        bool result = bank.WithdrawFunds(accountNumber!, ValidPin, 500.00m);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_WithdrawFunds_NullAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.WithdrawFunds(null, ValidPin, 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_WithdrawFunds_EmptyAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.WithdrawFunds("", ValidPin, 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_WithdrawFunds_NullPin_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.WithdrawFunds(ValidAccountNumber, null, 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_WithdrawFunds_EmptyPin_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.WithdrawFunds(ValidAccountNumber, "", 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_WithdrawFunds_NegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.WithdrawFunds(ValidAccountNumber, ValidPin, -100.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_WithdrawFunds_ZeroAmount_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.WithdrawFunds(ValidAccountNumber, ValidPin, 0.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bank_WithdrawFunds_NonExistentAccount_ShouldThrowInvalidOperationException()
    {
        // Arrange & Act & Assert
        bank.WithdrawFunds("99999999999", ValidPin, 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bank_WithdrawFunds_WrongPin_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string? accountNumber = bank.CreateAccount(ValidName, ValidPin);
        bank.DepositFunds(accountNumber!, 1000.00m);

        // Act & Assert
        bank.WithdrawFunds(accountNumber!, "9999", 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bank_WithdrawFunds_InsufficientBalance_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string? accountNumber = bank.CreateAccount(ValidName, ValidPin);
        bank.DepositFunds(accountNumber!, 100.00m);

        // Act & Assert
        bank.WithdrawFunds(accountNumber!, ValidPin, 500.00m);
    }

    #endregion

    #region Bank TransferFunds Tests

    [TestMethod]
    public void Bank_TransferFunds_ValidParameters_ShouldReturnTrue()
    {
        // Arrange
        string? senderAccount = bank.CreateAccount("Sender", ValidPin);
        string? receiverAccount = bank.CreateAccount("Receiver", "5678");
        bank.DepositFunds(senderAccount!, 1000.00m);

        // Act
        bool result = bank.TransferFunds(senderAccount!, ValidPin, receiverAccount!, 500.00m);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_TransferFunds_NullSenderAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.TransferFunds(null, ValidPin, "98765432100", 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_TransferFunds_EmptySenderAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.TransferFunds("", ValidPin, "98765432100", 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_TransferFunds_NullSenderPin_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.TransferFunds(ValidAccountNumber, null, "98765432100", 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_TransferFunds_EmptySenderPin_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.TransferFunds(ValidAccountNumber, "", "98765432100", 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_TransferFunds_NullReceiverAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.TransferFunds(ValidAccountNumber, ValidPin, null, 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_TransferFunds_EmptyReceiverAccountNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.TransferFunds(ValidAccountNumber, ValidPin, "", 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_TransferFunds_SameAccountNumbers_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.TransferFunds(ValidAccountNumber, ValidPin, ValidAccountNumber, 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_TransferFunds_NegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.TransferFunds(ValidAccountNumber, ValidPin, "98765432100", -100.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Bank_TransferFunds_ZeroAmount_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        bank.TransferFunds(ValidAccountNumber, ValidPin, "98765432100", 0.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bank_TransferFunds_NonExistentSenderAccount_ShouldThrowInvalidOperationException()
    {
        // Arrange
        bank.CreateAccount("Receiver", "5678");

        // Act & Assert
        bank.TransferFunds("99999999999", ValidPin, "98765432100", 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bank_TransferFunds_NonExistentReceiverAccount_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string? senderAccount = bank.CreateAccount(ValidName, ValidPin);
        bank.DepositFunds(senderAccount!, 1000.00m);

        // Act & Assert
        bank.TransferFunds(senderAccount!, ValidPin, "99999999999", 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bank_TransferFunds_WrongSenderPin_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string? senderAccount = bank.CreateAccount("Sender", ValidPin);
        string? receiverAccount = bank.CreateAccount("Receiver", "5678");
        bank.DepositFunds(senderAccount!, 1000.00m);

        // Act & Assert
        bank.TransferFunds(senderAccount!, "9999", receiverAccount!, 500.00m);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bank_TransferFunds_InsufficientBalance_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string? senderAccount = bank.CreateAccount("Sender", ValidPin);
        string? receiverAccount = bank.CreateAccount("Receiver", "5678");
        bank.DepositFunds(senderAccount!, 100.00m);

        // Act & Assert
        bank.TransferFunds(senderAccount!, ValidPin, receiverAccount!, 500.00m);
    }

    #endregion
}
