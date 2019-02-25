using System;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Events
{
    public class DailyWireTransferLimitSetEvent : Event
    {
        public Guid AccountId { get; set; }
        public decimal DailyWireTransferLimit { get; set; }
        public DailyWireTransferLimitSetEvent(CorrelatedMessage source) : base(source)
        { }
        [JsonConstructor]
        public DailyWireTransferLimitSetEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }
    }
}