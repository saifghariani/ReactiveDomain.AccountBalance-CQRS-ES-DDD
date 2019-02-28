using System;
using System.Collections.Generic;
using System.Text;
using ReactiveDomain.AccountBalance.Commands;
using ReactiveDomain.Messaging;
using Xunit;

namespace ReactiveDomain.AccountBalance.Tests
{
    public class SetOverdraftLimitTests : AccountTestsBase
    {
        [Theory]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(100)]
        [InlineData(100000000)]
        public void CanSetOverDraftLimit(decimal limit)
        {
            CmdHandler.Handle(new CreateAccountCommand
            {
                AccountId = AccountId,
                HolderName = "John Doe"
            });

            var cmd = new SetOverDraftLimitCommand
            {
                AccountId = AccountId,
                OverDraftLimit = limit
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

            var cmd = new SetOverDraftLimitCommand
            {
                AccountId = AccountId,
                OverDraftLimit = limit
            };
            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("Overdraft limit cannot be negative", msg.Exception.Message);


        }

        [Fact]
        public void CannotExceedOverDraftLimit()
        {
            CmdHandler.Handle(new CreateAccountCommand
            {
                AccountId = AccountId,
                HolderName = "John Doe"
            });
            
            CmdHandler.Handle(new SetDailyWireTransferLimitCommand
            {
                AccountId = AccountId,
                DailyWireTransferLimit = 1000m
            });
            CmdHandler.Handle(new SetOverDraftLimitCommand
            {
                AccountId = AccountId,
                OverDraftLimit = 100m
            });

            var cmd = new WithdrawCashCommand()
            {
                AccountId = AccountId,
                Amount = 101m
            };
            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("Account is blocked, you exceeded your overdraft limit", msg.Exception.Message);
        }
        [Fact]
        public void CannotSetLimitOnInvalidAccount()
        {
            var cmd = new SetOverDraftLimitCommand
            {
                AccountId = AccountId,
                OverDraftLimit = 100m
            };

            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("No account with this ID exists", msg.Exception.Message);
        }
    }
}
