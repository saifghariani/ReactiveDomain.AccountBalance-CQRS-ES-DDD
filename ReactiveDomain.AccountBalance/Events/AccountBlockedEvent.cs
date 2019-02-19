using System;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Events
{
    public class AccountBlockedEvent : Message
    {
        public Guid AccountId { get; set; }
        public string AccountState { get; set; }
        public AccountBlockedEvent(Guid accountId)
        {
            AccountId = accountId;
            AccountState = "Blocked";
        }
    }
}