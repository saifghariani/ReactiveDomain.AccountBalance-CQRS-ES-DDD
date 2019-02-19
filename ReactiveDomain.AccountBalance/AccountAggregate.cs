using System;
using System.ComponentModel.DataAnnotations;
using ReactiveDomain.AccountBalance.Events;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.AccountBalance
{
    public class AccountAggregate : EventDrivenStateMachine
    {
        public decimal Balance;
        public decimal OverDraftLimit;
        public decimal DailyWireTransferLimit;
        public string State;

        private decimal _withdrawnToday;

        public AccountAggregate()
        {
            setup();
        }
        public AccountAggregate(Guid id, string holderName) : this()
        {
            Raise(new AccountCreatedEvent(id, holderName));
        }
        class MySecretEvent : Message { }
        public void setup()
        {
            Register<AccountCreatedEvent>(evt =>
            {
                Id = evt.Id;
            });
            Register<AccountCashWithdrawnEvent>(Apply);
            Register<AccountCashDepositedEvent>(Apply);
            Register<AccountCheckDepositedEvent>(Apply);
            Register<DailyWireTransferLimitSetEvent>(Apply);
            Register<OverDraftLimitSetEvent>(Apply);
            Register<AccountBlockedEvent>(Apply);
            Register<AccountUnblockedEvent>(Apply);
        }

        private void Apply(AccountCheckDepositedEvent depositedEvent)
        {
            var depositDate = depositedEvent.CreditDate.AddDays(1);
            while (!(depositDate.DayOfWeek >= DayOfWeek.Monday && depositDate.DayOfWeek <= DayOfWeek.Friday))
            {
                depositDate = depositDate.AddDays(1);
            }
            if (DateTime.UtcNow.Date >= depositDate.Date)
                Balance += depositedEvent.Amount;
        }

        private void Apply(AccountUnblockedEvent unblockedEvent)
        {
            State = unblockedEvent.AccountState;
        }

        private void Apply(AccountBlockedEvent blockedEvent)
        {
            State = blockedEvent.AccountState;
        }

        private void Apply(AccountCashWithdrawnEvent @event)
        {
            if (@event.WithdrawDate.Date == DateTime.UtcNow.Date)
                _withdrawnToday += @event.Amount;
            else
                _withdrawnToday = 0;
            Balance -= @event.Amount;
        }
        private void Apply(AccountCashDepositedEvent @event)
        {
            Balance += @event.Amount;
        }
        private void Apply(DailyWireTransferLimitSetEvent @event)
        {
            DailyWireTransferLimit = @event.DailyWireTransferLimit;
        }
        private void Apply(OverDraftLimitSetEvent @event)
        {
            OverDraftLimit = @event.OverDraftLimit;
        }
        public void Credit(decimal amount, string type)
        {
            if (type.ToLower().Contains("cash"))
            {
                Raise(new AccountCashDepositedEvent(Id,amount));
                if (State.ToLower() == "blocked" && Balance >= 0)
                    Raise(new AccountUnblockedEvent(Id));
            }
            else if (type.ToLower().Contains("check"))
            {
                Raise(new AccountCheckDepositedEvent(Id, amount));
                if (State.ToLower() == "blocked" && Balance >= 0)
                    Raise(new AccountUnblockedEvent(Id));
            }
        }

        public void Debit(decimal amount)
        {
            if (amount < 0)
                throw new ValidationException("Cash Withdrawal cannot be a negative amount");

            if (State.ToLower() == "blocked")
                throw new ValidationException("Account is blocked");

            if (DailyWireTransferLimit <= _withdrawnToday + amount)
                Raise(new AccountBlockedEvent(Id));

            if (State.ToLower() == "active")
            {
                if (Balance - amount < 0 && Math.Abs(Balance - amount) > OverDraftLimit)
                    Raise(new AccountBlockedEvent(Id));
                else
                {
                    Raise(new AccountCashWithdrawnEvent(Id, amount));
                }
            }

        }

        public void SetDailyWireTransferLimit(decimal dailyWireTransferLimit)
        {
            if (dailyWireTransferLimit < 0)
                throw new ValidationException("dailyWireTransfer limit cannot be negative");

            Raise(new DailyWireTransferLimitSetEvent(Id, dailyWireTransferLimit));
        }

        public void SetOverDraftLimit(decimal overDraftLimit)
        {
            if (overDraftLimit < 0)
                throw new ValidationException("Overdraft limit cannot be negative");

            Raise(new OverDraftLimitSetEvent(Id, overDraftLimit));
        }
    }
}