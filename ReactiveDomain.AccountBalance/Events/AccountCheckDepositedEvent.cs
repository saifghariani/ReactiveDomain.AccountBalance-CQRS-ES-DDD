using System;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Events
{
    public class AccountCheckDepositedEvent : Event
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreditDate { get; set; }
        public AccountCheckDepositedEvent(CorrelatedMessage source) : base(source)
        { }
        [JsonConstructor]
        public AccountCheckDepositedEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }
    }
}