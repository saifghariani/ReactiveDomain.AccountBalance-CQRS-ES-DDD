using System;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance.Commands
{
    public class SetOverDraftLimitCommand : Command
    {
        public SetOverDraftLimitCommand() : base(NewRoot())
        {

        }
        public Guid AccountId { get; set; }
        public  decimal OverDraftLimit { get; set; }
    }
}
