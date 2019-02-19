using System;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Events
{
    public class AccountCashWithdrawnEvent : Message
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime WithdrawDate { get; set; }
        public AccountCashWithdrawnEvent(Guid accountId, decimal amount)
        {
            AccountId = accountId;
            Amount = amount;
            WithdrawDate = DateTime.UtcNow;
        }
    }
}