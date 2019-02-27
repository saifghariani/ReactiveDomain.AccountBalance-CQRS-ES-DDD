using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ReactiveDomain.AccountBalance.Commands;
using ReactiveDomain.Messaging;
using Xunit;

namespace ReactiveDomain.AccountBalance.Tests
{
    public class SetDailyWireTransferLimitTests : AccountTestsBase
    {
        [Theory]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(100)]
        [InlineData(100000000)]
        public void CanSetDailyWireTransferLimit(decimal limit)
        {
            CmdHandler.Handle(new CreateAccountCommand
            {
                AccountId = AccountId,
                HolderName = "John Doe"
            });

            var cmd = new SetDailyWireTransferLimitCommand
            {
                AccountId = AccountId,
                DailyWireTransferLimit = limit
            };

            Assert.IsType<Success>(CmdHandler.Handle(cmd));


        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-100)]
        [InlineData(-100000000)]
        public void CannotSetNegativeLimit(decimal limit)
        {
            CmdHandler.Handle(new CreateAccountCommand
            {
                AccountId = AccountId,
                HolderName = "John Doe"
            });

            var cmd = new SetDailyWireTransferLimitCommand
            {
                AccountId = AccountId,
                DailyWireTransferLimit = limit
            };
            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("dailyWireTransfer limit cannot be negative", msg.Exception.Message);


        }

        [Fact]
        public void CanAdjustLimitIntraday()
        {
            CmdHandler.Handle(new CreateAccountCommand
            {
                AccountId = AccountId,
                HolderName = "John Doe"
            });
            CmdHandler.Handle(new DepositCashCommand
            {
                AccountId = AccountId,
                Amount = 1000m
            });
            CmdHandler.Handle(new SetDailyWireTransferLimitCommand
            {
                AccountId = AccountId,
                DailyWireTransferLimit = 100m
            });

            CmdHandler.Handle(new WithdrawCashCommand()
            {
                AccountId = AccountId,
                Amount = 100m
            });
            CmdHandler.Handle(new SetDailyWireTransferLimitCommand
            {
                AccountId = AccountId,
                DailyWireTransferLimit = 200m
            });
            var cmd = new WithdrawCashCommand()
            {
                AccountId = AccountId,
                Amount = 100m
            };
            var msg = CmdHandler.Handle(cmd);
            Assert.IsType<Success>(msg);
        }
        [Fact]
        public void CannotExceedDailyWireTransferLimit()
        {
            CmdHandler.Handle(new CreateAccountCommand
            {
                AccountId = AccountId,
                HolderName = "John Doe"
            });
            CmdHandler.Handle(new DepositCashCommand
            {
                AccountId = AccountId,
                Amount = 1000m
            });
            CmdHandler.Handle(new SetDailyWireTransferLimitCommand
            {
                AccountId = AccountId,
                DailyWireTransferLimit = 100m
            });

            var cmd= new WithdrawCashCommand()
            {
                AccountId = AccountId,
                Amount = 101m
            };
            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.StartsWith("Account is blocked, you only have", msg.Exception.Message);
        }
        [Fact]
        public void CannotSetLimitOnInvalidAccount()
        {
            var cmd = new SetDailyWireTransferLimitCommand
            {
                AccountId = AccountId,
                DailyWireTransferLimit = 100m
            };

            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("No account with this ID exists", msg.Exception.Message);
        }
    }
}

