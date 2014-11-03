namespace Sage.Connector.Payments.Contracts.Data
{
    /// <summary>
    /// Payment Types
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// Cash
        /// </summary>
        Cash = 0,

        /// <summary>
        /// Check
        /// </summary>
        Check = 1,

        /// <summary>
        /// Credit Card
        /// </summary>
        CreditCard = 2,

        /// <summary>
        /// ACH
        /// </summary>
        Ach = 3
    }
}
