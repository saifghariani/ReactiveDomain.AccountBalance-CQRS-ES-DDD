using System;
using System.Collections;
using System.Collections.Generic;
using ReactiveDomain.AccountBalance;

namespace reactivedomain.accountbalance.ui.server
{
    public interface IAccountService
    {
        IList<AccountAggregate> GetAll();
        Guid Add(string holderName);
        void Deposit(Guid accountId, decimal amount);
    }
}
