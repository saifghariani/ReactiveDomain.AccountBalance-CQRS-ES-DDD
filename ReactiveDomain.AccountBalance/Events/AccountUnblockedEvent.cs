using System;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Events
{
    public class AccountUnblockedEvent : Message
    {
        public Guid AccountId { get; set; }
        public string AccountState { get; set; }
        public AccountUnblockedEvent(Guid accountId)
        {
            AccountId = accountId;
            AccountState = "Active";
        }
    }
}