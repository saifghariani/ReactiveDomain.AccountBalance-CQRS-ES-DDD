using System.Collections.Generic;
using DotNetify;
using ReactiveDomain.AccountBalance;

namespace reactivedomain.accountbalance.ui.server.ViewModels
{
    public class Accounts : BaseVM
    {
        private readonly IAccountService _accountService;

        public Accounts(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public IEnumerable<AccountAggregate> AccountsList => _accountService.GetAll();
        public string AccountsList_itemKey => nameof(AccountAggregate.Id);

    }
}
