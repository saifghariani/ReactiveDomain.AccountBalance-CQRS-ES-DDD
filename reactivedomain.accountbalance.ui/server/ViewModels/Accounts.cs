using DotNetify;
using ReactiveDomain.AccountBalance;
using System;
using System.Collections.Generic;
using System.Threading;

namespace reactivedomain.accountbalance.ui.server.ViewModels
{
    public class Accounts : BaseVM
    {
        private readonly IAccountService _accountService;
        private Timer _timer;
        public Accounts(IAccountService accountService)
        {
            _accountService = accountService;
            _timer = new Timer(state =>
            {
                if (CurrentAccount != null)
                {
                    CurrentAccount = _accountService.GetById(CurrentAccount.Id);
                    Changed(nameof(CurrentAccount));
                }
                AccountsList = _accountService.GetAll();
                Changed(nameof(AccountsList));
                PushUpdates();

            }, null, 0, 1000);
        }
        public override void Dispose() => _timer.Dispose();
        public IEnumerable<Account> AccountsList { get; set; }
        public string AccountsList_itemKey => nameof(AccountAggregate.Id);
        public Account CurrentAccount { get; set; }
        public string Message { get; set; }
        public Action<string> Add => (holderName) =>
        {
            var newRecord = new Account()
            {
                HolderName = holderName,
                State = "Active"
            };

            this.AddList(nameof(AccountsList), new Account()
            {
                Id = _accountService.Add(newRecord),
                HolderName = newRecord.HolderName,
                State = newRecord.State,
            });
        };
        public Action<string> SetOverDraftLimit => (overDraftLimit) =>
        {
            var msg = _accountService.SetOverDraftLimit(CurrentAccount.Id, decimal.Parse(overDraftLimit));
            if (msg.ToString().ToLower().Contains("success"))
                Message = "OverdraftLimit set";
            else
                Message = "Could not set OverdraftLimit";
            Changed(nameof(Message));
            //CurrentAccount = _accountService.GetById(CurrentAccount.Id);
            //Changed(nameof(CurrentAccount));
        };
        public Action<string> SetDailyWireTransferLimit => (dailyWireTransferLimit) =>
        {
            var msg = _accountService.SetDailyWireTransferLimit(CurrentAccount.Id, decimal.Parse(dailyWireTransferLimit));
            if (msg.ToString().ToLower().Contains("success"))
                Message = "DailyWireTransferLimit set";
            else
                Message = "Could not set DailyWireTransferLimit";
            Changed(nameof(Message));
            //CurrentAccount = _accountService.GetById(CurrentAccount.Id);
            //Changed(nameof(CurrentAccount));
        };

        public Action<string> Withdraw => (amount) =>
        {
            var msg = _accountService.Withdraw(CurrentAccount.Id, decimal.Parse(amount));

            //CurrentAccount = _accountService.GetById(CurrentAccount.Id);
            //Changed(nameof(CurrentAccount));
        };
        public Action<string> DepositCash => (amount) =>
        {
            var msg = _accountService.DepositCash(CurrentAccount.Id, decimal.Parse(amount));
            if (msg.ToString().ToLower().Contains("success"))
                Message = "Cash deposited";
            else
                Message = "Could not deposit cash";

            Changed(nameof(Message));
            //CurrentAccount = _accountService.GetById(CurrentAccount.Id);
            //Changed(nameof(CurrentAccount));
        };
        public Action<string> DepositCheck => (amount) =>
        {

            var msg = _accountService.DepositCheck(CurrentAccount.Id, decimal.Parse(amount));
            if (msg.ToString().ToLower().Contains("success"))
                Message = "Check deposited, the funds will be available on the next business day.";
            else
                Message = "Could not deposit check";
            Changed(nameof(Message));
            //CurrentAccount = _accountService.GetById(CurrentAccount.Id);
            //Changed(nameof(CurrentAccount));
        };

        public Action<Account> Select => (account) =>
        {
            if (account.State == "disconnect")
            {
                CurrentAccount = null;
                Message = string.Empty;
                Changed(nameof(CurrentAccount));
                Changed(nameof(Message));
            }
            else
            {
                var tempaccount = _accountService.GetById(account.Id);
                if (tempaccount != null)
                {
                    if (tempaccount.HolderName == account.HolderName)
                    {
                        CurrentAccount = tempaccount;
                        Message = string.Empty;
                        Changed(nameof(CurrentAccount));
                        Changed(nameof(Message));
                    }
                    else
                    {
                        Message = "Invalid HolderName";
                        Changed(nameof(Message));
                    }
                }
                else
                {
                    Message = "Invalid Account";
                    Changed(nameof(Message));
                }
            }

        };
    }
}
