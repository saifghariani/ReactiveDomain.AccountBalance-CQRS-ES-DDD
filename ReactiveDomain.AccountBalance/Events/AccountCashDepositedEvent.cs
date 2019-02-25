using System;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Events
{
    public class AccountCashDepositedEvent : Event
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public AccountCashDepositedEvent(CorrelatedMessage source) : base(source)
        { }
        [JsonConstructor]
        public AccountCashDepositedEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }
    }
}