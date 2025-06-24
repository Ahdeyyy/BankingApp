namespace BankingApp 
{
    public class Account
{
    // Properties to hold account information
    public string Name { get; set; }
    public string AccountNumber { get; set; }
    public string Pin { get; set; }
    public decimal Balance { get; set; }

        /// <summary>
        /// Initializes a new instance of the Account class.
        /// </summary>
        /// <param name="name">The name of the account holder.</param>
        /// <param name="accountNumber">The unique 9-digit account number.</param>
        /// <param name="pin">The personal identification number for the account.</param>
        /// <param name="balance">The initial balance of the account.</param>
        /// <returns>A new Account object.</returns>
        /// <remarks>
        /// Requirements:
        /// - Name, AccountNumber, and Pin should not be null or empty.
        /// - AccountNumber should be a 9-digit string.
        /// - Balance should not be negative initially.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if any input parameters are invalid (e.g., null, empty, or incorrect format).</exception>
        public Account(string name, string accountNumber, string pin, decimal balance)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            if (string.IsNullOrEmpty(accountNumber) || accountNumber.Length != 9 || !long.TryParse(accountNumber, out _))
                throw new ArgumentException("Account number must be a 9-digit string.", nameof(accountNumber));
            if (string.IsNullOrEmpty(pin))
                throw new ArgumentException("Pin cannot be null or empty.", nameof(pin));      
            if (balance < 0)
                throw new ArgumentException("Balance cannot be negative.", nameof(balance));
            
            this.AccountNumber = accountNumber;
            this.Name = name;
            this.Pin = pin;
            this.Balance = balance;
       
    }
}
}