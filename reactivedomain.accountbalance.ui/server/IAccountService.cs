using System;
using System.Collections;
using System.Collections.Generic;
using ReactiveDomain.AccountBalance;
using ReactiveDomain.Messaging;

namespace reactivedomain.accountbalance.ui.server
{
    public interface IAccountService
    {
        IList<Account> GetAll();
        Guid Add(Account account);
        Account GetById(Guid accountId);
        CommandResponse DepositCash(Guid accountId, decimal amount);
        CommandResponse DepositCheck(Guid accountId, decimal amount);
        CommandResponse Withdraw(Guid accountId, decimal amount);
        CommandResponse SetOverDraftLimit(Guid accountId, decimal overDraftLimit);
        CommandResponse SetDailyWireTransferLimit(Guid accountId, decimal dailyWireTransferLimit);
        
    }
}
