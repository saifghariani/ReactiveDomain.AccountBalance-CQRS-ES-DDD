using System;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Events
{
    public class AccountUnblockedEvent : Event
    {
        public Guid AccountId { get; set; }
        public string AccountState { get; set; }
        public AccountUnblockedEvent(CorrelatedMessage source) : base(source)
        { }
        [JsonConstructor]
        public AccountUnblockedEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }
    }
}