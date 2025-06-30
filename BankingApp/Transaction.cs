using System.ComponentModel;

namespace BankingApp
{
    public enum TransactionType
    {
        [Description("Deposit")]
        Deposit,

        [Description("Withdrawal")]
        Withdrawal,

        [Description("Transfer")]
        Transfer
        
    
    }
    
    public class Transaction
{
    // Properties to hold transaction information
    public string TransactionId { get; set; }
    public string AccountId { get; set; } // The account number involved in the transaction
    public TransactionType Type { get; set; }      // e.g., "Deposit", "Withdrawal", "Transfer"
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public string? RecipientAccountId { get; set; } // For transfer transactions, the receiving account

        /// <summary>
        /// Initializes a new instance of the Transaction class.
        /// </summary>
        /// <param name="transactionId">A unique identifier for the transaction.</param>
        /// <param name="accountId">The account number from which the transaction originates or is applied to.</param>
        /// <param name="type">The type of transaction (e.g., "Deposit", "Withdrawal", "Transfer").</param>
        /// <param name="amount">The amount of money involved in the transaction.</param>
        /// <param name="timestamp">The date and time the transaction occurred.</param>
        /// <param name="recipientAccountId">Optional: The account number of the recipient for transfer transactions.</param>
        /// <returns>A new Transaction object.</returns>
        /// <remarks>
        /// Requirements:
        /// - TransactionId, AccountId, and Type should not be null or empty.
        /// - Amount must be a positive value.
        /// - Timestamp should represent a valid date and time.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if any input parameters are invalid (e.g., null, empty, or incorrect values).</exception>
        public Transaction(string transactionId, string accountId, TransactionType type, decimal amount, DateTime timestamp, string? recipientAccountId)
        {
            if (string.IsNullOrEmpty(transactionId))
                throw new ArgumentException("Transaction ID cannot be null or empty.", nameof(transactionId));
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentException("Account ID cannot be null or empty.", nameof(accountId));
            
            if (amount <= 0)
                throw new ArgumentException("Amount must be a positive value.", nameof(amount));
            if (timestamp == default)
                throw new ArgumentException("Timestamp must be a valid date and time.", nameof(timestamp));
            if (type == TransactionType.Transfer && string.IsNullOrEmpty(recipientAccountId))
                throw new ArgumentException("Recipient account ID cannot be null or empty for transfer transactions.", nameof(recipientAccountId));
            
            if (recipientAccountId != null && recipientAccountId.Length != 11)
                throw new ArgumentException("Recipient account ID must be exactly 11 digits long.", nameof(recipientAccountId));
            if (accountId.Length != 11)
                throw new ArgumentException("Account ID must be exactly 11 digits long.", nameof(accountId));
        

            this.TransactionId = transactionId;
            this.AccountId = accountId;
            this.Type = type;
            this.Amount = amount;
            this.Timestamp = timestamp;

            this.RecipientAccountId = recipientAccountId;
        }
      
    }
}
