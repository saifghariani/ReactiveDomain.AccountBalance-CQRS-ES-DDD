﻿using EventStore.ClientAPI;
using ReactiveDomain.EventStore;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
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
        private IRepository repo;
        private Guid _accountId = Guid.Parse("06AC5641-EDE6-466F-9B37-DD8304D05A84");
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
            Account acct = null;
            try
            {
                repo.Save(new Account(_accountId, _holderName));
            }
            catch (Exception e)
            {
            }
            IListener listener = new StreamListener("Account", conn, namer, ser);
            _readModel = new BalanceReadModel(() => listener, _accountId);
        }
        public void Run()
        {
            var cmd = new[] { "" };
            Account acct;
            do
            {

                cmd = Console.ReadLine().Split(' ');
                switch (cmd[0].ToLower())
                {
                    case "credit":
                        acct = repo.GetById<Account>(_accountId);
                        acct.Credit(uint.Parse(cmd[1]));
                        repo.Save(acct);
                        Console.WriteLine($"got credit {cmd[1]}");
                        break;
                    case "debit":
                        try
                        {

                            acct = repo.GetById<Account>(_accountId);
                            acct.Debit(uint.Parse(cmd[1]));
                            repo.Save(acct);
                            Console.WriteLine($"got debit {cmd[1]}");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        break;
                    case "overdraftlimit":
                        acct = repo.GetById<Account>(_accountId);
                        acct.SetOverDraftLimit(uint.Parse(cmd[1]));
                        repo.Save(acct);
                        Console.WriteLine($"setting overdraftLimit to {cmd[1]}");
                        break;
                    case "dailywiretransferlimit":
                        acct = repo.GetById<Account>(_accountId);
                        acct.SetDailyWireTransferLimit(uint.Parse(cmd[1]));
                        repo.Save(acct);
                        Console.WriteLine($"setting dailyWireTransferLimit to {cmd[1]}");
                        break;
                }

            } while (cmd[0].ToLower() != "exit");
        }
    }

    public class BalanceReadModel :
        ReadModelBase,
        IHandle<Credit>,
        IHandle<Debit>,
        IHandle<BlockAccount>,
        IHandle<UnblockAccount>,
        IHandle<SetOverDraftLimit>,
        IHandle<SetDailyWireTransferLimit>
    {
        public BalanceReadModel(Func<IListener> listener, Guid accountId) : base(listener)
        {
            EventStream.Subscribe<Credit>(this);
            EventStream.Subscribe<Debit>(this);
            EventStream.Subscribe<BlockAccount>(this);
            EventStream.Subscribe<UnblockAccount>(this);
            EventStream.Subscribe<SetOverDraftLimit>(this);
            EventStream.Subscribe<SetDailyWireTransferLimit>(this);
            Start<Account>(accountId);
        }

        private int balance;
        private string state;
        private int overDraftLimit;
        private int dailyWireTransferLimit;

        public void Handle(BlockAccount message)
        {
            state = message.AccountState;
            redraw();

        }
        public void Handle(UnblockAccount message)
        {
            state = message.AccountState;
            redraw();

        }
        public void Handle(Credit message)
        {
            balance += (int)message.Amount;
            redraw();

        }
        public void Handle(Debit message)
        {
            balance -= (int)message.Amount;
            redraw();
        }
        public void Handle(SetOverDraftLimit message)
        {
            overDraftLimit = (int)message.OverDraftLimit;
            redraw();

        }
        public void Handle(SetDailyWireTransferLimit message)
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
    public class Account : EventDrivenStateMachine
    {
        private long _balance;
        private long _overDraftLimit;
        private long _dailyWireTransferLimit;
        private string _state;

        public Account()
        {
            setup();
        }
        public Account(Guid id, string holderName) : this()
        {
            Raise(new AccountCreated(id, holderName));
        }
        class MySecretEvent : Message { }
        public void setup()
        {
            Register<AccountCreated>(evt =>
            {
                Id = evt.Id;
                _state = evt.State;
            });
            Register<Debit>(Apply);
            Register<Credit>(Apply);
            Register<SetDailyWireTransferLimit>(Apply);
            Register<SetOverDraftLimit>(Apply);
            Register<BlockAccount>(Apply);
            Register<UnblockAccount>(Apply);
        }

        private void Apply(UnblockAccount @event)
        {
            _state = @event.AccountState;
        }

        private void Apply(BlockAccount @event)
        {
            _state = @event.AccountState;
        }

        private void Apply(Debit @event)
        {
            _balance -= @event.Amount;
        }
        private void Apply(Credit @event)
        {
            _balance += @event.Amount;
        }
        private void Apply(SetDailyWireTransferLimit @event)
        {
            _dailyWireTransferLimit = @event.DailyWireTransferLimit;
        }
        private void Apply(SetOverDraftLimit @event)
        {
            _overDraftLimit = @event.OverDraftLimit;
        }
        public void Credit(uint amount)
        {
            if (_state.ToLower() == "blocked" && _balance + amount >= 0)
                Raise(new UnblockAccount());

            Raise(new Credit(amount));
        }

        public void Debit(uint amount)
        {
            //Ensure.LessThanOrEqualTo(_dailyWireTransferLimit, withdrawntoday, "Balance");
            if (_state.ToLower() == "active")
            {
                if (_balance - amount < 0 && Math.Abs(_balance - amount) > _overDraftLimit)
                    Raise(new BlockAccount());
                else
                    Raise(new Debit(amount));
            }

        }

        public void SetDailyWireTransferLimit(uint dailyWireTransferLimit)
        {
            //nothing to check
            Raise(new SetDailyWireTransferLimit(dailyWireTransferLimit));
        }

        public void SetOverDraftLimit(uint overDraftLimit)
        {
            //nothing to check
            Raise(new SetOverDraftLimit(overDraftLimit));
        }
    }
    public class AccountCreated : Message
    {
        public readonly Guid Id;
        public readonly string HolderName;
        public readonly string State;

        public AccountCreated(Guid id, string holderName)
        {
            Id = id;
            HolderName = holderName;
            State = "Active";
        }
    }

    public class Debit : Message
    {
        public uint Amount;
        public Debit(uint amount)
        {
            Amount = amount;
        }
    }

    public class Credit : Message
    {
        public uint Amount;

        public Credit(uint amount)
        {
            Amount = amount;
        }
    }

    public class SetOverDraftLimit : Message
    {
        public uint OverDraftLimit;
        public SetOverDraftLimit(uint overDraftLimit)
        {
            OverDraftLimit = overDraftLimit;
        }
    }

    public class SetDailyWireTransferLimit : Message
    {
        public uint DailyWireTransferLimit;
        public SetDailyWireTransferLimit(uint dailyWireTransferLimit)
        {
            DailyWireTransferLimit = dailyWireTransferLimit;
        }
    }
    public class BlockAccount : Message
    {
        public string AccountState;
        public BlockAccount()
        {
            AccountState = "Blocked";
        }
    }
    public class UnblockAccount : Message
    {
        public string AccountState;
        public UnblockAccount()
        {
            AccountState = "Active";
        }
    }
}