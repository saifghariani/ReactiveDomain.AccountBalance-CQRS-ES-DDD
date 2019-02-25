using ReactiveDomain.Messaging;
using System;

namespace ReactiveDomain.AccountBalance.Commands
{
    public class CreateAccountCommand : Command
    {
        public CreateAccountCommand() : base(NewRoot())
        { }

        public Guid AccountId { get; set; }
        public string HolderName { get; set; }
    }
}
