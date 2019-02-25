using ReactiveDomain.Messaging;
using System;

namespace ReactiveDomain.AccountBalance.Commands
{
    public class DepositCheckCommand : Command
    {
        public DepositCheckCommand() : base(NewRoot())
        {

        }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }

    }
}
