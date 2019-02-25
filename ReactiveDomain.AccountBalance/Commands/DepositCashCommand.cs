using System;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Commands
{
    public class DepositCashCommand : Command
    {
        public DepositCashCommand() : base(NewRoot())
        {

        }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }

    }
}
