using System;
using System.Collections.Generic;
using System.Text;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Commands
{
    public class WithdrawCashCommand : Command
    {
        public WithdrawCashCommand():base(NewRoot())
        {
            
        }

        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
