using System;
using ReactiveDomain.AccountBalance.Events;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;

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
        public BalanceReadModel(Func<IListener> listener, Guid accountId) : base(listener)
        {
            EventStream.Subscribe<AccountCashDepositedEvent>(this);
            EventStream.Subscribe<AccountCheckDepositedEvent>(this);
            EventStream.Subscribe<AccountCashWithdrawnEvent>(this);
            EventStream.Subscribe<AccountBlockedEvent>(this);
            EventStream.Subscribe<AccountUnblockedEvent>(this);
            EventStream.Subscribe<OverDraftLimitSetEvent>(this);
            EventStream.Subscribe<DailyWireTransferLimitSetEvent>(this);
            EventStream.Subscribe<AccountCreatedEvent>(this);
            Start<AccountAggregate>(accountId);
        }

        private int balance;
        private string state;
        private string holderName;
        private int overDraftLimit;
        private int dailyWireTransferLimit;
        public void Handle(AccountCreatedEvent message)
        {
            state = message.State;
            holderName = message.HolderName;
            redraw();
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
            balance += (int)message.Amount;
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
                balance += (int)message.Amount;

            redraw();
        }
        public void Handle(AccountCashWithdrawnEvent message)
        {
            balance -= (int)message.Amount;
            redraw();
        }
        public void Handle(OverDraftLimitSetEvent message)
        {
            overDraftLimit = (int)message.OverDraftLimit;
            redraw();

        }
        public void Handle(DailyWireTransferLimitSetEvent message)
        {
            dailyWireTransferLimit = (int)message.DailyWireTransferLimit;
            redraw();

        }

        private void redraw()
        {
            Console.Clear();
            Console.WriteLine($"OverDraftLimit = { overDraftLimit }");
            Console.WriteLine($"DailyWireTransferLimit = { dailyWireTransferLimit }");
            Console.WriteLine($"Balance = { balance }");
            Console.WriteLine($"State = { state }");
        }

        
    }
}