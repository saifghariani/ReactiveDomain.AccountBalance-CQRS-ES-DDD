using ReactiveDomain.AccountBalance.Commands;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReactiveDomain.AccountBalance
{
    public class AccountCommandHandler : IHandleCommand<CreateAccountCommand>
        , IHandleCommand<DepositCashCommand>
        , IHandleCommand<DepositCheckCommand>
        , IHandleCommand<SetDailyWireTransferLimitCommand>
        , IHandleCommand<SetOverDraftLimitCommand>
        , IHandleCommand<WithdrawCashCommand>
    {
        readonly IRepository _repository;

        public AccountCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public CommandResponse Handle(CreateAccountCommand command)
        {
            try
            {
                if (_repository.TryGetById<AccountAggregate>(command.AccountId, out var _))
                    throw new ValidationException("An account with this ID already exists");
                if(string.IsNullOrWhiteSpace(command.HolderName))
                    throw new ValidationException("HolderName can't be empty");

                var account = new AccountAggregate(command.AccountId, command.HolderName, command);
                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }

        public CommandResponse Handle(DepositCashCommand command)
        {
            try
            {
                if (!_repository.TryGetById<AccountAggregate>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                account.Credit(command.Amount, "cash", command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }

        public CommandResponse Handle(DepositCheckCommand command)
        {
            try
            {
                if (!_repository.TryGetById<AccountAggregate>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                account.Credit(command.Amount, "check", command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }

        public CommandResponse Handle(SetDailyWireTransferLimitCommand command)
        {
            try
            {
                if (!_repository.TryGetById<AccountAggregate>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                account.SetDailyWireTransferLimit(command.DailyWireTransferLimit, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }

        public CommandResponse Handle(SetOverDraftLimitCommand command)
        {
            try
            {
                if (!_repository.TryGetById<AccountAggregate>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                account.SetOverDraftLimit(command.OverDraftLimit, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }

        public CommandResponse Handle(WithdrawCashCommand command)
        {
            try
            {
                if (!_repository.TryGetById<AccountAggregate>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                var msg =account.Debit(command.Amount, command);
                _repository.Save(account);
                if (!string.IsNullOrEmpty(msg))
                    throw new ValidationException(msg);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }

    }
}
