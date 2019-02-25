using System;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Commands
{
    public class SetDailyWireTransferLimitCommand : Command
    {
        public SetDailyWireTransferLimitCommand() : base(NewRoot())
        {

        }
        public Guid AccountId { get; set; }
        public decimal DailyWireTransferLimit { get; set; }
    }
}
