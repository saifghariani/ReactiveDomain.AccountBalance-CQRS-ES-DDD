﻿using ReactiveDomain.AccountBalance.Events;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;
using System;
using System.Collections.Generic;

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
        public List<AccountAggregate> Accounts = new List<AccountAggregate>();

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
            Accounts.Add(new AccountAggregate(message.Id, holderName, message));
            //redraw();
        }
        public void Handle(AccountBlockedEvent message)
        {
            state = message.AccountState;
            redraw();

        }
        public void Handle(AccountUnblockedEvent message)
        {
            state = message.AccountState;
            redraw();

        }
        public void Handle(AccountCashDepositedEvent message)
        {
            balance += (decimal)message.Amount;
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

            redraw();
        }
        public void Handle(AccountCashWithdrawnEvent message)
        {
            balance -= (decimal)message.Amount;
            redraw();
        }
        public void Handle(OverDraftLimitSetEvent message)
        {
            overDraftLimit = (decimal)message.OverDraftLimit;
            redraw();

        }
        public void Handle(DailyWireTransferLimitSetEvent message)
        {
            dailyWireTransferLimit = (decimal)message.DailyWireTransferLimit;
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
            Console.Clear();

            if (account != null)
                GetAccount(account);

            Console.WriteLine($"Holder's name = { holderName }");
            Console.WriteLine($"OverDraftLimit = { overDraftLimit }");
            Console.WriteLine($"DailyWireTransferLimit = { dailyWireTransferLimit }");
            Console.WriteLine($"Balance = { balance }");
            Console.WriteLine($"State = { state }");
        }


    }
}