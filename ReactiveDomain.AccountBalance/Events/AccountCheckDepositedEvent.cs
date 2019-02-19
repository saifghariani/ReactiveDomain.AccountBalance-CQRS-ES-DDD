using System;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Events
{
    public class AccountCheckDepositedEvent : Message
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreditDate { get; set; }
        public AccountCheckDepositedEvent(Guid accountId, decimal amount)
        {
            AccountId = accountId;
            Amount = amount;
            CreditDate = DateTime.UtcNow;
        }
    }
}