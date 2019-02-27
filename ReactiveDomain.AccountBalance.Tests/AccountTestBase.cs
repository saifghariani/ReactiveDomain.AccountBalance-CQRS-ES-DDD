using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using ReactiveDomain.EventStore;
using ReactiveDomain.Foundation;

namespace ReactiveDomain.AccountBalance.Tests
{
    public abstract class AccountTestsBase 
    {
        protected BalanceReadModel ReadModel;
        protected Guid AccountId { get; }
        protected AccountCommandHandler CmdHandler { get; }
        protected IRepository Repo;
        protected AccountTestsBase()
        {
            AccountId = Guid.NewGuid();
            IEventStoreConnection esConnection = EventStoreConnection.Create("ConnectTo=tcp://admin:changeit@localhost:1113");
            var conn = new EventStoreConnectionWrapper(esConnection);
            esConnection.Connected += (_, __) => { };
            esConnection.ConnectAsync().Wait();
            IStreamNameBuilder namer = new PrefixedCamelCaseStreamNameBuilder("Tests");
            IEventSerializer ser = new JsonMessageSerializer();
            Repo = new StreamStoreRepository(namer, conn, ser);
            CmdHandler = new AccountCommandHandler(Repo);

            var listener = new StreamListener("AccountAggregate", conn, namer, ser);
            ReadModel = new BalanceReadModel(() => listener);
            


        }
        
    }
}
