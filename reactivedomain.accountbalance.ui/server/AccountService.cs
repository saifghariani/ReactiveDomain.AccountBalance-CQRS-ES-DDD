using EventStore.ClientAPI;
using ReactiveDomain.AccountBalance;
using ReactiveDomain.AccountBalance.Commands;
using ReactiveDomain.EventStore;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using System;
using System.Collections.Generic;

namespace reactivedomain.accountbalance.ui.server
{
    public class AccountService : IAccountService
    {
        private readonly BalanceReadModel _readModel;
        private readonly IRepository _repo;
        private readonly AccountCommandHandler _cmdHandler;

        public AccountService()
        {
            IEventStoreConnection esConnection = EventStoreConnection.Create("ConnectTo=tcp://admin:changeit@localhost:1113");
            var conn = new EventStoreConnectionWrapper(esConnection);
            esConnection.Connected += (_, __) => { };
                //Console.WriteLine("Connected");
            esConnection.ConnectAsync().Wait();
            IStreamNameBuilder namer = new PrefixedCamelCaseStreamNameBuilder();
            IEventSerializer ser = new JsonMessageSerializer();
            _repo = new StreamStoreRepository(namer, conn, ser);
            _cmdHandler = new AccountCommandHandler(_repo);

            var listener = new StreamListener("AccountAggregate", conn, namer, ser);
            _readModel = new BalanceReadModel(() => listener);
        }

        public Guid Add(Account account)
        {
            var accountId = Guid.NewGuid();
            var createAccountCommand = new CreateAccountCommand
            {
                AccountId = accountId,
                HolderName = account.HolderName,
            };
            _cmdHandler.Handle(createAccountCommand);
            return accountId;
        }

        public CommandResponse DepositCash(Guid accountId, decimal amount)
        {
            var account = _repo.GetById<AccountAggregate>(accountId);
            var depositCashCommand = new DepositCashCommand
            {
                AccountId = account.Id,
                Amount = amount
            };
            return _cmdHandler.Handle(depositCashCommand);
        }

        public CommandResponse DepositCheck(Guid accountId, decimal amount)
        {
            var account = _repo.GetById<AccountAggregate>(accountId);
            var depositCheckCommand = new DepositCheckCommand
            {
                AccountId = account.Id,
                Amount = amount
            };
            return _cmdHandler.Handle(depositCheckCommand);
        }
        public CommandResponse SetDailyWireTransferLimit(Guid accountId, decimal dailyWireTransferLimit)
        {
            var account = _repo.GetById<AccountAggregate>(accountId);
            var setDailyWireTransferLimitCommand = new SetDailyWireTransferLimitCommand()
            {
                AccountId = account.Id,
                DailyWireTransferLimit = dailyWireTransferLimit
            };
            return _cmdHandler.Handle(setDailyWireTransferLimitCommand);
        }

        public CommandResponse SetOverDraftLimit(Guid accountId, decimal overDraftLimit)
        {
            var account = _repo.GetById<AccountAggregate>(accountId);
            var setOverDraftLimitCommand = new SetOverDraftLimitCommand()
            {
                AccountId = account.Id,
                OverDraftLimit = overDraftLimit
            };
            return _cmdHandler.Handle(setOverDraftLimitCommand);
        }

        public CommandResponse Withdraw(Guid accountId, decimal amount)
        {
            var account = _repo.GetById<AccountAggregate>(accountId);
            var withdrawCommand = new WithdrawCashCommand
            {
                AccountId = account.Id,
                Amount = amount
            };
            return _cmdHandler.Handle(withdrawCommand);
        }

        public IList<Account> GetAll() => _readModel.Accounts;

        public Account GetById(Guid accountId)
        {
            var account = _repo.GetById<AccountAggregate>(accountId);
            return account.ToAccount();
        }
    }
}
