using System;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Events
{
    public class DailyWireTransferLimitSetEvent : Message
    {
        public Guid AccountId { get; set; }
        public decimal DailyWireTransferLimit { get; set; }
        public DailyWireTransferLimitSetEvent(Guid accountId, decimal dailyWireTransferLimit)
        {
            AccountId = accountId;
            DailyWireTransferLimit = dailyWireTransferLimit;
        }
    }
}