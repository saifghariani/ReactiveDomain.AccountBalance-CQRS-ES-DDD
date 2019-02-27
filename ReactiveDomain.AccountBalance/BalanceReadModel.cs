using ReactiveDomain.AccountBalance.Events;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactiveDomain.AccountBalance
{
    public class BalanceReadModel :
        ReadModelBase,
        IHandle<AccountCashDepositedEvent>,
        IHandle<AccountCreatedEvent>,
        IHandle<AccountCheckDepositedEvent>,
        IHandle<AccountCashWithdrawnEvent>,
        IHandle<AccountBlockedEvent>,
        IHandle<AccountUnblockedEvent>,
        IHandle<OverDraftLimitSetEvent>,
        IHandle<DailyWireTransferLimitSetEvent>
    {
        public List<Account> Accounts = new List<Account>();

        public BalanceReadModel(Func<IListener> listener) : base(listener)
        {
            EventStream.Subscribe<AccountCashDepositedEvent>(this);
            EventStream.Subscribe<AccountCheckDepositedEvent>(this);
            EventStream.Subscribe<AccountCashWithdrawnEvent>(this);
            EventStream.Subscribe<AccountBlockedEvent>(this);
            EventStream.Subscribe<AccountUnblockedEvent>(this);
            EventStream.Subscribe<OverDraftLimitSetEvent>(this);
            EventStream.Subscribe<DailyWireTransferLimitSetEvent>(this);
            EventStream.Subscribe<AccountCreatedEvent>(this);
            Start<AccountAggregate>(null, true);
        }

        private decimal balance;
        private string state;
        private string holderName;
        private decimal overDraftLimit;
        private decimal dailyWireTransferLimit;
        public void Handle(AccountCreatedEvent message)
        {
            state = message.State;
            holderName = message.HolderName;
            Accounts.Add(new Account
            {
                Id = message.Id.ToString(),
                HolderName = message.HolderName,
                State = message.State
            });
        }
        public void Handle(AccountBlockedEvent message)
        {
            state = message.AccountState;
            Accounts.First(acct => acct.Id == message.AccountId.ToString()).State=state;
            redraw();
        }
        public void Handle(AccountUnblockedEvent message)
        {
            state = message.AccountState;
            Accounts.First(acct => acct.Id == message.AccountId.ToString()).State = state;
            redraw();

        }
        public void Handle(AccountCashDepositedEvent message)
        {
            balance += (decimal)message.Amount;
            Accounts.First(acct => acct.Id == message.AccountId.ToString()).Balance = balance;
            redraw();

        }
        public void Handle(AccountCheckDepositedEvent message)
        {
            var depositDate = message.CreditDate.AddDays(1);
            while (!(depositDate.DayOfWeek >= DayOfWeek.Monday && depositDate.DayOfWeek <= DayOfWeek.Friday))
            {
                depositDate = depositDate.AddDays(1);
            }
            if (DateTime.UtcNow.Date >= depositDate.Date)
                balance += (decimal)message.Amount;

            Accounts.First(acct => acct.Id == message.AccountId.ToString()).Balance = balance;
            redraw();
        }
        public void Handle(AccountCashWithdrawnEvent message)
        {
            balance -= (decimal)message.Amount;
            Accounts.First(acct => acct.Id == message.AccountId.ToString()).Balance = balance;
            redraw();
        }
        public void Handle(OverDraftLimitSetEvent message)
        {
            overDraftLimit = (decimal)message.OverDraftLimit;
            Accounts.First(acct => acct.Id == message.AccountId.ToString()).OverDraftLimit = overDraftLimit;
            redraw();

        }
        public void Handle(DailyWireTransferLimitSetEvent message)
        {
            dailyWireTransferLimit = (decimal)message.DailyWireTransferLimit;
            Accounts.First(acct => acct.Id == message.AccountId.ToString()).DailyWireTransferLimit = dailyWireTransferLimit;
            redraw();

        }

        public void GetAccount(AccountAggregate account)
        {

            holderName = account.HolderName;
            overDraftLimit = account.OverDraftLimit;
            dailyWireTransferLimit = account.DailyWireTransferLimit;
            balance = account.Balance;
            state = account.State;

        }
        public void redraw(AccountAggregate account = null)
        {
            ///Console.Clear();

            if (account != null)
                GetAccount(account);

            //Console.WriteLine($"Holder's name = { holderName }");
            //Console.WriteLine($"OverDraftLimit = { overDraftLimit }");
            //Console.WriteLine($"DailyWireTransferLimit = { dailyWireTransferLimit }");
            //Console.WriteLine($"Balance = { balance }");
            //Console.WriteLine($"State = { state }");
        }


    }
}