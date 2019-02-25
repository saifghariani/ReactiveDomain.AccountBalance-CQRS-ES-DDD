using System;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Events
{
    public class AccountBlockedEvent : Event
    {
        public Guid AccountId { get; set; }
        public string AccountState { get; set; }
        public AccountBlockedEvent(CorrelatedMessage source):base(source)
        { }
        [JsonConstructor]
        public AccountBlockedEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }
    }
}