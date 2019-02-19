using EventStore.ClientAPI;
using ReactiveDomain.EventStore;
using ReactiveDomain.Foundation;
using System;

namespace ReactiveDomain.AccountBalance
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var app = new Application();
            app.Bootstrap();
            app.Run();
            Console.ReadKey();
        }
    }

    public class Application
    {
        private IStreamStoreConnection conn;
        public IRepository repo;
        public Guid _accountId = Guid.Parse("06AC5641-EDE6-466F-9B37-DD8304D05A84");
        private string _holderName = "Saif";
        private BalanceReadModel _readModel;
        public void Bootstrap()
        {
            IEventStoreConnection esConnection = EventStoreConnection.Create("ConnectTo=tcp://admin:changeit@localhost:1113");
            conn = new EventStoreConnectionWrapper(esConnection);
            esConnection.Connected += (_, __) => Console.WriteLine("Connected");
            esConnection.ConnectAsync().Wait();
            IStreamNameBuilder namer = new PrefixedCamelCaseStreamNameBuilder();
            IEventSerializer ser = new JsonMessageSerializer();
            repo = new StreamStoreRepository(namer, conn, ser);
            AccountAggregate acct = null;
            try
            {
                repo.Save(new AccountAggregate(_accountId, _holderName));
            }
            catch (Exception e)
            {
            }
            IListener listener = new StreamListener("AccountAggregate", conn, namer, ser);
            _readModel = new BalanceReadModel(() => listener, _accountId);
        }
        public void Run()
        {
            var cmd = new[] { "" };
            AccountAggregate acct;
            do
            {

                cmd = Console.ReadLine().Split(' ');
                switch (cmd[0].ToLower())
                {
                    case "new":

                        repo.Save(new AccountAggregate(_accountId, _holderName));

                        break;
                    case "deposit":
                        acct = repo.GetById<AccountAggregate>(_accountId);
                        try
                        {
                            acct.Credit(decimal.Parse(cmd[2]), cmd[1]);
                            repo.Save(acct);
                            Console.WriteLine($"Deposit {cmd[2]}");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Command should be:\n\tdeposit {type} {amount}\n\ttype : 'cash' or 'check'\n\tamount : number");
                        }
                        break;
                    case "withdraw":
                        try
                        {

                            acct = repo.GetById<AccountAggregate>(_accountId);
                            acct.Debit(decimal.Parse(cmd[1]));
                            repo.Save(acct);
                            Console.WriteLine($"Withdraw {cmd[1]}");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        break;
                    case "overdraft":
                        acct = repo.GetById<AccountAggregate>(_accountId);
                        try
                        {
                            acct.SetOverDraftLimit(decimal.Parse(cmd[1]));
                            Console.WriteLine($"setting overdraftLimit to {cmd[1]}");
                            repo.Save(acct);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case "daily":
                        acct = repo.GetById<AccountAggregate>(_accountId);
                        try
                        {
                            acct.SetDailyWireTransferLimit(decimal.Parse(cmd[1]));
                            Console.WriteLine($"setting dailyWireTransferLimit to {cmd[1]}");
                            repo.Save(acct);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                }
            } while (cmd[0].ToLower() != "exit");
        }
    }

}
