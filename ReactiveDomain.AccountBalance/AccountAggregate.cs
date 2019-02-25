using ReactiveDomain.AccountBalance.Events;
using ReactiveDomain.Messaging;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReactiveDomain.AccountBalance
{
    public class AccountAggregate : EventDrivenStateMachine
    {
        public decimal Balance;
        public decimal OverDraftLimit;
        public decimal DailyWireTransferLimit;
        public string State;
        public string HolderName;

        private decimal _withdrawnToday;

        public AccountAggregate()
        {
            setup();
        }
        public AccountAggregate(Guid id, string holderName, CorrelatedMessage source) : this()
        {
            var accountCreated = new AccountCreatedEvent(source)
            {
                HolderName = holderName,
                Id = id,
                State = "Active"
            };
            Raise(accountCreated);
        }
        class MySecretEvent : Message { }
        public void setup()
        {
            Register<AccountCreatedEvent>(evt =>
            {
                Id = evt.Id;
                HolderName = evt.HolderName;
                State = evt.State;
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
        public void Credit(decimal amount, string type, CorrelatedMessage source)
        {
            if (type.ToLower().Contains("cash"))
            {
                var cashDeposited = new AccountCashDepositedEvent(source)
                {
                    AccountId = Id,
                    Amount = amount
                };
                Raise(cashDeposited);
                if (State.ToLower() == "blocked" && Balance >= 0)
                {
                    var unblocked = new AccountUnblockedEvent(source)
                    {
                        AccountId = Id,
                        AccountState = "Active"
                    };
                    Raise(unblocked);
                }
            }
            else if (type.ToLower().Contains("check"))
            {
                var checkDeposited = new AccountCheckDepositedEvent(source)
                {
                    AccountId = Id,
                    Amount = amount,
                    CreditDate = DateTime.UtcNow
                };
                Raise(checkDeposited);
                if (State.ToLower() == "blocked" && Balance >= 0)
                {
                    var unblocked = new AccountUnblockedEvent(source)
                    {
                        AccountId = Id,
                        AccountState = "Active"
                    };
                    Raise(unblocked);
                }
            }
        }

        public void Debit(decimal amount, CorrelatedMessage source)
        {
            if (amount < 0)
                throw new ValidationException("Cash Withdrawal cannot be a negative amount");

            if (State.ToLower() == "blocked")
                throw new ValidationException("Account is blocked");

            if (DailyWireTransferLimit <= _withdrawnToday + amount)
            {
                var blocked = new AccountBlockedEvent(source)
                {
                    AccountId = Id,
                    AccountState = "Blocked"
                };
                Raise(blocked);
            }

            if (State.ToLower() == "active")
            {
                if (Balance - amount < 0 && Math.Abs(Balance - amount) > OverDraftLimit)
                {
                    var blocked = new AccountBlockedEvent(source)
                    {
                        AccountId = Id,
                        AccountState = "Blocked"
                    };
                    Raise(blocked);
                }
                else
                {
                    var withdrawn = new AccountCashWithdrawnEvent(source)
                    {
                        AccountId = Id,
                        Amount = amount,
                        WithdrawDate = DateTime.UtcNow
                    };
                    Raise(withdrawn);
                }
            }

        }

        public void SetDailyWireTransferLimit(decimal dailyWireTransferLimit, CorrelatedMessage source)
        {
            if (dailyWireTransferLimit < 0)
                throw new ValidationException("dailyWireTransfer limit cannot be negative");
            var dailyWireTransferLimitSet = new DailyWireTransferLimitSetEvent(source)
            {
                AccountId = Id,
                DailyWireTransferLimit = dailyWireTransferLimit
            };
            Raise(dailyWireTransferLimitSet);
        }

        public void SetOverDraftLimit(decimal overDraftLimit, CorrelatedMessage source)
        {
            if (overDraftLimit < 0)
                throw new ValidationException("Overdraft limit cannot be negative");

            var overDraftLimitSetEvent = new OverDraftLimitSetEvent(source)
            {
                AccountId = Id,
                OverDraftLimit = overDraftLimit
            };
            Raise(overDraftLimitSetEvent);
        }
    }
}