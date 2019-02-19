using System;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Events
{
    public class AccountCashDepositedEvent : Message
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public AccountCashDepositedEvent(Guid accountId, decimal amount)
        {
            AccountId = accountId;
            Amount = amount;
        }
    }
}