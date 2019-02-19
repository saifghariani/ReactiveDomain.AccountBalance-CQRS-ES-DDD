using ReactiveDomain.Messaging;
using System;

namespace ReactiveDomain.AccountBalance.Events
{
    public class AccountCreatedEvent : Message
    {
        public readonly Guid Id;
        public string HolderName { get; set; }
        public string State { get; set; }

        public AccountCreatedEvent(Guid id, string holderName)
        {
            Id = id;
            HolderName = holderName;
            State = "Active";
        }
    }
}
