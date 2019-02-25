using EventStore.ClientAPI;
using ReactiveDomain.AccountBalance;
using ReactiveDomain.EventStore;
using ReactiveDomain.Foundation;
using System;
using System.Collections.Generic;

namespace reactivedomain.accountbalance.ui.server
{
    public class AccountService : IAccountService
    {
        private readonly BalanceReadModel _readModel;
        private IRepository repo;
        private AccountCommandHandler cmdHandler;

        public AccountService()
        {
            IEventStoreConnection esConnection = EventStoreConnection.Create("ConnectTo=tcp://admin:changeit@localhost:1113");
            var conn = new EventStoreConnectionWrapper(esConnection);
            esConnection.Connected += (_, __) => Console.WriteLine("Connected");
            esConnection.ConnectAsync().Wait();
            IStreamNameBuilder namer = new PrefixedCamelCaseStreamNameBuilder();
            IEventSerializer ser = new JsonMessageSerializer();
            repo = new StreamStoreRepository(namer, conn, ser);
            cmdHandler = new AccountCommandHandler(repo);
            AccountAggregate acct = null;

            var listener = new StreamListener("AccountAggregate", conn, namer, ser);
            _readModel = new BalanceReadModel(() => listener);
        }

        public Guid Add(string holderName)
        {
            var accountId = Guid.NewGuid();
            //repo.Save(new AccountAggregate(accountId, holderName));
            return accountId;
        }

        public void Deposit(Guid accountId, decimal amount)
        {
            repo.TryGetById(accountId, out AccountAggregate account);
            //account.Debit(amount);
            repo.Save(account);
        }

        public IList<AccountAggregate> GetAll() => _readModel.Accounts;

    }
}
