using System;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Events
{
    public class OverDraftLimitSetEvent : Message
    {
        public Guid AccountId { get; set; }
        public decimal OverDraftLimit { get; set; }
        public OverDraftLimitSetEvent(Guid accountId, decimal overDraftLimit)
        {
            AccountId = accountId;
            OverDraftLimit = overDraftLimit;
        }
    }
}