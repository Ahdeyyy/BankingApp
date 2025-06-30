using BankingApp;

namespace BankingAppTests;

[TestClass]
public sealed class TransactionTest
{
    private const string ValidTransactionId = "TXN123456789";
    private const string ValidAccountId = "12345678901";
    private const string ValidRecipientAccountId = "98765432100";
    private const decimal ValidAmount = 100.00m;
    private static readonly DateTime ValidTimestamp = DateTime.Now;

    #region Transaction Constructor Tests

    [TestMethod]
    public void Transaction_Constructor_ValidParameters_Deposit_ShouldCreateTransaction()
    {
        // Arrange & Act
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Deposit", ValidAmount, ValidTimestamp, null);

        // Assert
        Assert.AreEqual(ValidTransactionId, transaction.TransactionId);
        Assert.AreEqual(ValidAccountId, transaction.AccountId);
        Assert.AreEqual(TransactionType.Deposit, transaction.Type);
        Assert.AreEqual(ValidAmount, transaction.Amount);
        Assert.AreEqual(ValidTimestamp, transaction.Timestamp);
        Assert.IsNull(transaction.RecipientAccountId);
    }

    [TestMethod]
    public void Transaction_Constructor_ValidParameters_Withdrawal_ShouldCreateTransaction()
    {
        // Arrange & Act
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Withdrawal", ValidAmount, ValidTimestamp, null);

        // Assert
        Assert.AreEqual(ValidTransactionId, transaction.TransactionId);
        Assert.AreEqual(ValidAccountId, transaction.AccountId);
        Assert.AreEqual(TransactionType.Withdrawal, transaction.Type);
        Assert.AreEqual(ValidAmount, transaction.Amount);
        Assert.AreEqual(ValidTimestamp, transaction.Timestamp);
        Assert.IsNull(transaction.RecipientAccountId);
    }

    [TestMethod]
    public void Transaction_Constructor_ValidParameters_Transfer_ShouldCreateTransaction()
    {
        // Arrange & Act
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Transfer", ValidAmount, ValidTimestamp, ValidRecipientAccountId);

        // Assert
        Assert.AreEqual(ValidTransactionId, transaction.TransactionId);
        Assert.AreEqual(ValidAccountId, transaction.AccountId);
        Assert.AreEqual(TransactionType.Transfer, transaction.Type);
        Assert.AreEqual(ValidAmount, transaction.Amount);
        Assert.AreEqual(ValidTimestamp, transaction.Timestamp);
        Assert.AreEqual(ValidRecipientAccountId, transaction.RecipientAccountId);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Transaction_Constructor_NullTransactionId_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var transaction = new Transaction(null, ValidAccountId, "Deposit", ValidAmount, ValidTimestamp, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Transaction_Constructor_EmptyTransactionId_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var transaction = new Transaction("", ValidAccountId, "Deposit", ValidAmount, ValidTimestamp, null);
    }

    [TestMethod]
    public void Transaction_Constructor_WhitespaceTransactionId_ShouldCreateTransaction()
    {
        // Arrange & Act (whitespace is allowed since only null/empty are checked)
        var transaction = new Transaction("   ", ValidAccountId, "Deposit", ValidAmount, ValidTimestamp, null);

        // Assert
        Assert.AreEqual("   ", transaction.TransactionId);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Transaction_Constructor_NullAccountId_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var transaction = new Transaction(ValidTransactionId, null, "Deposit", ValidAmount, ValidTimestamp, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Transaction_Constructor_EmptyAccountId_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var transaction = new Transaction(ValidTransactionId, "", "Deposit", ValidAmount, ValidTimestamp, null);
    }

    [TestMethod]
    public void Transaction_Constructor_WhitespaceAccountId_ShouldCreateTransaction()
    {
        // Arrange & Act (whitespace is allowed since only null/empty are checked)
        var transaction = new Transaction(ValidTransactionId, "   ", "Deposit", ValidAmount, ValidTimestamp, null);

        // Assert
        Assert.AreEqual("   ", transaction.AccountId);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Transaction_Constructor_NullType_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, null, ValidAmount, ValidTimestamp, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Transaction_Constructor_EmptyType_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "", ValidAmount, ValidTimestamp, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Transaction_Constructor_InvalidType_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "InvalidType", ValidAmount, ValidTimestamp, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Transaction_Constructor_CaseInvalidType_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert (case sensitivity test)
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "deposit", ValidAmount, ValidTimestamp, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Transaction_Constructor_ZeroAmount_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Deposit", 0.00m, ValidTimestamp, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Transaction_Constructor_NegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Deposit", -100.00m, ValidTimestamp, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Transaction_Constructor_VerySmallNegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Deposit", -0.01m, ValidTimestamp, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Transaction_Constructor_DefaultTimestamp_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Deposit", ValidAmount, default(DateTime), null);
    }

    [TestMethod]
    public void Transaction_Constructor_MinValidAmount_ShouldCreateTransaction()
    {
        // Arrange & Act
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Deposit", 0.01m, ValidTimestamp, null);

        // Assert
        Assert.AreEqual(0.01m, transaction.Amount);
    }

    [TestMethod]
    public void Transaction_Constructor_LargeAmount_ShouldCreateTransaction()
    {
        // Arrange & Act
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Deposit", 999999.99m, ValidTimestamp, null);

        // Assert
        Assert.AreEqual(999999.99m, transaction.Amount);
    }

    [TestMethod]
    public void Transaction_Constructor_FutureTimestamp_ShouldCreateTransaction()
    {
        // Arrange
        var futureDate = DateTime.Now.AddDays(1);

        // Act
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Deposit", ValidAmount, futureDate, null);

        // Assert
        Assert.AreEqual(futureDate, transaction.Timestamp);
    }

    [TestMethod]
    public void Transaction_Constructor_PastTimestamp_ShouldCreateTransaction()
    {
        // Arrange
        var pastDate = DateTime.Now.AddDays(-1);

        // Act
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Deposit", ValidAmount, pastDate, null);

        // Assert
        Assert.AreEqual(pastDate, transaction.Timestamp);
    }

    [TestMethod]
    public void Transaction_Constructor_TransferWithNullRecipient_ShouldCreateTransaction()
    {
        // Arrange & Act
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Transfer", ValidAmount, ValidTimestamp, null);

        // Assert
        Assert.AreEqual(TransactionType.Transfer, transaction.Type);
        Assert.IsNull(transaction.RecipientAccountId);
    }

    [TestMethod]
    public void Transaction_Constructor_TransferWithEmptyRecipient_ShouldCreateTransaction()
    {
        // Arrange & Act
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Transfer", ValidAmount, ValidTimestamp, "");

        // Assert
        Assert.AreEqual(TransactionType.Transfer, transaction.Type);
        Assert.AreEqual("", transaction.RecipientAccountId);
    }

    [TestMethod]
    public void Transaction_Constructor_DepositWithRecipient_ShouldCreateTransaction()
    {
        // Arrange & Act (recipient should be ignored for deposits)
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Deposit", ValidAmount, ValidTimestamp, ValidRecipientAccountId);

        // Assert
        Assert.AreEqual(TransactionType.Deposit, transaction.Type);
        Assert.AreEqual(ValidRecipientAccountId, transaction.RecipientAccountId);
    }

    [TestMethod]
    public void Transaction_Constructor_WithdrawalWithRecipient_ShouldCreateTransaction()
    {
        // Arrange & Act (recipient should be ignored for withdrawals)
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Withdrawal", ValidAmount, ValidTimestamp, ValidRecipientAccountId);

        // Assert
        Assert.AreEqual(TransactionType.Withdrawal, transaction.Type);
        Assert.AreEqual(ValidRecipientAccountId, transaction.RecipientAccountId);
    }

    #endregion

    #region Property Tests

    [TestMethod]
    public void Transaction_Properties_ShouldBeSettable()
    {
        // Arrange
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Deposit", ValidAmount, ValidTimestamp, null);
        var newTransactionId = "TXN987654321";
        var newAccountId = "98765432100";
        var newAmount = 250.00m;
        var newTimestamp = DateTime.Now.AddHours(1);
        var newRecipientId = "11122233300";

        // Act
        transaction.TransactionId = newTransactionId;
        transaction.AccountId = newAccountId;
        transaction.Type = TransactionType.Withdrawal;
        transaction.Amount = newAmount;
        transaction.Timestamp = newTimestamp;
        transaction.RecipientAccountId = newRecipientId;

        // Assert
        Assert.AreEqual(newTransactionId, transaction.TransactionId);
        Assert.AreEqual(newAccountId, transaction.AccountId);
        Assert.AreEqual(TransactionType.Withdrawal, transaction.Type);
        Assert.AreEqual(newAmount, transaction.Amount);
        Assert.AreEqual(newTimestamp, transaction.Timestamp);
        Assert.AreEqual(newRecipientId, transaction.RecipientAccountId);
    }

    #endregion

    #region Edge Case Tests

    [TestMethod]
    public void Transaction_Constructor_LongTransactionId_ShouldCreateTransaction()
    {
        // Arrange
        var longTransactionId = new string('A', 1000); // Very long transaction ID

        // Act
        var transaction = new Transaction(longTransactionId, ValidAccountId, "Deposit", ValidAmount, ValidTimestamp, null);

        // Assert
        Assert.AreEqual(longTransactionId, transaction.TransactionId);
    }

    [TestMethod]
    public void Transaction_Constructor_LongAccountId_ShouldCreateTransaction()
    {
        // Arrange
        var longAccountId = new string('1', 50); // Very long account ID

        // Act
        var transaction = new Transaction(ValidTransactionId, longAccountId, "Deposit", ValidAmount, ValidTimestamp, null);

        // Assert
        Assert.AreEqual(longAccountId, transaction.AccountId);
    }

    [TestMethod]
    public void Transaction_Constructor_LongRecipientAccountId_ShouldCreateTransaction()
    {
        // Arrange
        var longRecipientId = new string('9', 50); // Very long recipient ID

        // Act
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Transfer", ValidAmount, ValidTimestamp, longRecipientId);

        // Assert
        Assert.AreEqual(longRecipientId, transaction.RecipientAccountId);
    }

    [TestMethod]
    public void Transaction_Constructor_SpecialCharactersInIds_ShouldCreateTransaction()
    {
        // Arrange
        var specialTransactionId = "TXN-123_456@789#";
        var specialAccountId = "ACC-987_654@321#";
        var specialRecipientId = "REC-111_222@333#";

        // Act
        var transaction = new Transaction(specialTransactionId, specialAccountId, "Transfer", ValidAmount, ValidTimestamp, specialRecipientId);

        // Assert
        Assert.AreEqual(specialTransactionId, transaction.TransactionId);
        Assert.AreEqual(specialAccountId, transaction.AccountId);
        Assert.AreEqual(specialRecipientId, transaction.RecipientAccountId);
    }

    [TestMethod]
    public void Transaction_Constructor_MaxDecimalAmount_ShouldCreateTransaction()
    {
        // Arrange & Act
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Deposit", decimal.MaxValue, ValidTimestamp, null);

        // Assert
        Assert.AreEqual(decimal.MaxValue, transaction.Amount);
    }

    [TestMethod]
    public void Transaction_Constructor_MinDateTimeTimestamp_ShouldCreateTransaction()
    {
        // Arrange
        var minDateTime = new DateTime(1900, 1, 1); // Valid but very old date

        // Act
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Deposit", ValidAmount, minDateTime, null);

        // Assert
        Assert.AreEqual(minDateTime, transaction.Timestamp);
    }

    [TestMethod]
    public void Transaction_Constructor_MaxDateTimeTimestamp_ShouldCreateTransaction()
    {
        // Arrange
        var maxDateTime = new DateTime(2099, 12, 31); // Valid but far future date

        // Act
        var transaction = new Transaction(ValidTransactionId, ValidAccountId, "Deposit", ValidAmount, maxDateTime, null);

        // Assert
        Assert.AreEqual(maxDateTime, transaction.Timestamp);
    }

    #endregion

    #region TransactionType Enum Tests

    [TestMethod]
    public void TransactionType_Enum_ShouldHaveExpectedValues()
    {
        // Arrange & Act & Assert
        Assert.IsTrue(Enum.IsDefined(typeof(TransactionType), TransactionType.Deposit));
        Assert.IsTrue(Enum.IsDefined(typeof(TransactionType), TransactionType.Withdrawal));
        Assert.IsTrue(Enum.IsDefined(typeof(TransactionType), TransactionType.Transfer));
    }

    [TestMethod]
    public void TransactionType_Enum_ShouldHaveCorrectIntValues()
    {
        // Arrange & Act & Assert
        Assert.AreEqual(0, (int)TransactionType.Deposit);
        Assert.AreEqual(1, (int)TransactionType.Withdrawal);
        Assert.AreEqual(2, (int)TransactionType.Transfer);
    }

    [TestMethod]
    public void TransactionType_Enum_ShouldParseFromString()
    {
        // Arrange & Act & Assert
        Assert.IsTrue(Enum.TryParse("Deposit", out TransactionType deposit));
        Assert.AreEqual(TransactionType.Deposit, deposit);

        Assert.IsTrue(Enum.TryParse("Withdrawal", out TransactionType withdrawal));
        Assert.AreEqual(TransactionType.Withdrawal, withdrawal);

        Assert.IsTrue(Enum.TryParse("Transfer", out TransactionType transfer));
        Assert.AreEqual(TransactionType.Transfer, transfer);
    }

    [TestMethod]
    public void TransactionType_Enum_ShouldParseValidNumericStrings()
    {
        // Arrange & Act & Assert (Enum.TryParse allows numeric strings for valid enum values)
        Assert.IsTrue(Enum.TryParse("0", out TransactionType deposit));
        Assert.AreEqual(TransactionType.Deposit, deposit);

        Assert.IsTrue(Enum.TryParse("1", out TransactionType withdrawal));
        Assert.AreEqual(TransactionType.Withdrawal, withdrawal);

        Assert.IsTrue(Enum.TryParse("2", out TransactionType transfer));
        Assert.AreEqual(TransactionType.Transfer, transfer);
    }

    [TestMethod]
    public void TransactionType_Enum_ShouldNotParseInvalidString()
    {
        // Arrange & Act & Assert
        Assert.IsFalse(Enum.TryParse("InvalidType", out TransactionType _));
        Assert.IsFalse(Enum.TryParse("deposit", out TransactionType _)); // case sensitive
        Assert.IsFalse(Enum.TryParse("", out TransactionType _));
        // Note: "123" might parse as an integer value, so removing this test case
    }

    [TestMethod]
    public void TransactionType_Enum_ShouldNotParseInvalidStrings()
    {
        // Arrange & Act & Assert
        // Note: Enum.TryParse is very permissive with numeric values, so we test only clearly invalid cases
        Assert.IsFalse(Enum.TryParse("abc", out TransactionType _)); // Non-numeric string
        Assert.IsFalse(Enum.TryParse("3.5", out TransactionType _)); // Decimal number
        Assert.IsFalse(Enum.TryParse("InvalidType", out TransactionType _)); // Invalid text
    }

    #endregion
}
