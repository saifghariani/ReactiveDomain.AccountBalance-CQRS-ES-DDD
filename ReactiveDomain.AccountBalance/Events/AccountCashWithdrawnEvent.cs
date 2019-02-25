using System;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Events
{
    public class AccountCashWithdrawnEvent : Event
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime WithdrawDate { get; set; }
        public AccountCashWithdrawnEvent(CorrelatedMessage source):base (source)
        { }
        [JsonConstructor]
        public AccountCashWithdrawnEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }
    }
}