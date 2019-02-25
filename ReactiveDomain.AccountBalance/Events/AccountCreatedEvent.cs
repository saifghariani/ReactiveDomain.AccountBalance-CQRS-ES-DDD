using ReactiveDomain.Messaging;
using System;
using Newtonsoft.Json;

namespace ReactiveDomain.AccountBalance.Events
{
    public class AccountCreatedEvent : Event
    {
        public Guid Id { get; set; }
        public string HolderName { get; set; }
        public string State { get; set; }

        public AccountCreatedEvent(CorrelatedMessage source) : base(source)
        { }
        [JsonConstructor]
        public AccountCreatedEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }
    }
}
