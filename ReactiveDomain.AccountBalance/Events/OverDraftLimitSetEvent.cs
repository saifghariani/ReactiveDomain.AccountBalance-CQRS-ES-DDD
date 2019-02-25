using Newtonsoft.Json;
using ReactiveDomain.Messaging;
using System;

namespace ReactiveDomain.AccountBalance.Events
{
    public class OverDraftLimitSetEvent : Event
    {
        public Guid AccountId { get; set; }
        public decimal OverDraftLimit { get; set; }
        public OverDraftLimitSetEvent(CorrelatedMessage source) : base(source)
        { }
        [JsonConstructor]
        public OverDraftLimitSetEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }
    }
}