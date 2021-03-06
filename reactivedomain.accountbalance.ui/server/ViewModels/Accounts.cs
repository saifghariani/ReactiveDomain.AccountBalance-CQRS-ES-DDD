﻿using DotNetify;
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
        private int _messagedelay = 0;
        public Accounts(IAccountService accountService)
        {
            _accountService = accountService;
            _timer = new Timer(state =>
            {
                if (CurrentAccount != null)
                {
                    CurrentAccount = _accountService.GetById(Guid.Parse(CurrentAccount.Id));
                    Changed(nameof(CurrentAccount));
                }

                if (_messagedelay == 0)
                {
                    Message = string.Empty;
                    Changed(nameof(Message));
                }
                AccountsList = _accountService.GetAll();
                Changed(nameof(AccountsList));
                PushUpdates();
                _messagedelay--;

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
                Id = Guid.NewGuid().ToString(),
                HolderName = holderName,
                State = "Active"
            };
            dynamic msg = _accountService.Add(newRecord);
            if (msg.ToString().ToLower().Contains("success"))
            {
                this.AddList(nameof(AccountsList), new Account()
                {
                    Id = newRecord.Id,
                    HolderName = newRecord.HolderName,
                    State = newRecord.State,
                });
                Message = "Account created";
            }
            else
                Message = msg.Exception.Message;
            _messagedelay = 5;
            Changed(nameof(Message));
        };
        public Action<string> SetOverDraftLimit => (overDraftLimit) =>
        {
            dynamic msg = _accountService.SetOverDraftLimit(Guid.Parse(CurrentAccount.Id), decimal.Parse(overDraftLimit));
            if (msg.ToString().ToLower().Contains("success"))
                Message = "Overdraft Limit Set";
            else
                Message = msg.Exception.Message;
            _messagedelay = 5;
            Changed(nameof(Message));
        };
        public Action<string> SetDailyWireTransferLimit => (dailyWireTransferLimit) =>
        {
            dynamic msg = _accountService.SetDailyWireTransferLimit(Guid.Parse(CurrentAccount.Id), decimal.Parse(dailyWireTransferLimit));
            if (msg.ToString().ToLower().Contains("success"))
                Message = "Daily Wire Transfer Limit Set";
            else
                Message = msg.Exception.Message;
            _messagedelay = 5;
            Changed(nameof(Message));
        };

        public Action<string> Withdraw => (amount) =>
        {
            dynamic msg = _accountService.Withdraw(Guid.Parse(CurrentAccount.Id), decimal.Parse(amount));
            if (msg.ToString().ToLower().Contains("success"))
                Message = "Cash Withdrawn";
            else
                Message = msg.Exception.Message;
            _messagedelay = 5;
            Changed(nameof(Message));
        };
        public Action<string> DepositCash => (amount) =>
        {
            dynamic msg = _accountService.DepositCash(Guid.Parse(CurrentAccount.Id), decimal.Parse(amount));
            if (msg.ToString().ToLower().Contains("success"))
                Message = "Cash deposited";
            else
                Message = msg.Exception.Message;
            _messagedelay = 5;
            Changed(nameof(Message));
        };
        public Action<string> DepositCheck => (amount) =>
        {

            dynamic msg = _accountService.DepositCheck(Guid.Parse(CurrentAccount.Id), decimal.Parse(amount));
            if (msg.ToString().ToLower().Contains("success"))
                Message = "Check deposited, the funds will be available on the next business day.";
            else
                Message = msg.Exception.Message;
            _messagedelay = 5;
            Changed(nameof(Message));
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
                Guid.TryParse(account.Id, out Guid accountId);
                var tempaccount = _accountService.GetById(accountId);
                if (tempaccount.Id != Guid.Empty.ToString())
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
                        _messagedelay = 5;
                        Changed(nameof(Message));
                    }
                }
                else
                {
                    Message = "Invalid Account";
                    _messagedelay = 5;
                    Changed(nameof(Message));
                }
            }

        };
    }
}
