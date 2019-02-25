using EventStore.ClientAPI;
using ReactiveDomain.AccountBalance.Commands;
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
        private IListener listener;
        private AccountCommandHandler cmdHandler;
        public void Bootstrap()
        {
            IEventStoreConnection esConnection = EventStoreConnection.Create("ConnectTo=tcp://admin:changeit@localhost:1113");
            conn = new EventStoreConnectionWrapper(esConnection);
            esConnection.Connected += (_, __) => Console.WriteLine("Connected");
            esConnection.ConnectAsync().Wait();
            IStreamNameBuilder namer = new PrefixedCamelCaseStreamNameBuilder();
            IEventSerializer ser = new JsonMessageSerializer();
            repo = new StreamStoreRepository(namer, conn, ser);
            cmdHandler = new AccountCommandHandler(repo);
            AccountAggregate acct = null;
            try
            {
                var command = new CreateAccountCommand()
                {
                    AccountId = _accountId,
                    HolderName = _holderName
                };
                cmdHandler.Handle(command);

            }
            catch (Exception e)
            {
            }
            listener = new StreamListener("AccountAggregate", conn, namer, ser);
            _readModel = new BalanceReadModel(() => listener);
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
                    case "conn":
                        _accountId = Guid.Parse(cmd[1]);

                        acct = repo.GetById<AccountAggregate>(_accountId);
                        _holderName = acct.HolderName;
                        _readModel.redraw(acct);
                        break;
                    case "list":
                        foreach (var account in _readModel.Accounts)
                        {
                            Console.WriteLine(account.Id + "\t" + account.HolderName);
                        }
                        break;
                    case "new":
                        _accountId = Guid.NewGuid();

                        if (cmd.Length > 1)
                            _holderName = cmd[1];

                        var createCommand = new CreateAccountCommand()
                        {
                            AccountId = _accountId,
                            HolderName = _holderName
                        };
                        cmdHandler.Handle(createCommand);

                        break;
                    case "deposit":
                        acct = repo.GetById<AccountAggregate>(_accountId);
                        try
                        {
                            if (cmd[1].ToLower() == "cash")
                            {
                                var depositCashCommand = new DepositCashCommand()
                                {
                                    AccountId = acct.Id,
                                    Amount = decimal.Parse(cmd[2])
                                };
                                cmdHandler.Handle(depositCashCommand);
                            }
                            else if (cmd[1].ToLower() == "check")
                            {
                                var depositCheckCommand = new DepositCheckCommand()
                                {
                                    AccountId = acct.Id,
                                    Amount = decimal.Parse(cmd[2])
                                };
                                cmdHandler.Handle(depositCheckCommand);
                            }
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
                            var withdrawCommand = new WithdrawCashCommand
                            {
                                AccountId = acct.Id,
                                Amount = decimal.Parse(cmd[1])
                            };
                            cmdHandler.Handle(withdrawCommand);
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
                            var setOverDraftLimit = new SetOverDraftLimitCommand()
                            {
                                AccountId = acct.Id,
                                OverDraftLimit = decimal.Parse(cmd[1])
                            };
                            cmdHandler.Handle(setOverDraftLimit);
                            Console.WriteLine($"setting overdraftLimit to {cmd[1]}");
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
                            var setDailyWireTransferLimit = new SetDailyWireTransferLimitCommand()
                            {
                                AccountId = acct.Id,
                                DailyWireTransferLimit = decimal.Parse(cmd[1])
                            };
                            cmdHandler.Handle(setDailyWireTransferLimit);
                            Console.WriteLine($"setting dailyWireTransferLimit to {cmd[1]}");
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
