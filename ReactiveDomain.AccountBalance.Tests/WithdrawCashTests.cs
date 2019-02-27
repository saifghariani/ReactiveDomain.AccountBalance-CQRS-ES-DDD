using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ReactiveDomain.AccountBalance.Commands;
using ReactiveDomain.Messaging;
using Xunit;

namespace ReactiveDomain.AccountBalance.Tests
{
    public class WithdrawCashTests : AccountTestsBase
    {
        [Fact]
        public void CanWithdrawCash()
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
                DailyWireTransferLimit = 1000m
            });

            var cmd= new WithdrawCashCommand()
            {
                AccountId = AccountId,
                Amount = 100m
            };
            Assert.IsType<Success>(CmdHandler.Handle(cmd));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-0.01)]
        [InlineData(-100)]
        [InlineData(-100000000)]
        public void CannotWithdrawCashWithNonPositiveAmount(decimal amount)
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
                DailyWireTransferLimit = 1000m
            });

            var cmd = new WithdrawCashCommand()
            {
                AccountId = AccountId,
                Amount = amount
            };
            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("Cannot withdraw a negative amount", msg.Exception.Message);
        }

        [Fact]
        public void CannotWithdrawCashWithInsufficientFunds()
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

            var cmd = new WithdrawCashCommand()
            {
                AccountId = AccountId,
                Amount = 500m
            };
            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("Account is blocked, you exceeded your overdraft limit", msg.Exception.Message);
        }

        [Fact]
        public void CanWithdrawCashFromOverdraft()
        {
            CmdHandler.Handle(new CreateAccountCommand
            {
                AccountId = AccountId,
                HolderName = "John Doe"
            });
            CmdHandler.Handle(new SetOverDraftLimitCommand()
            {
                AccountId = AccountId,
                OverDraftLimit = 100m
            });
            CmdHandler.Handle(new DepositCashCommand
            {
                AccountId = AccountId,
                Amount = 100m
            });
            CmdHandler.Handle(new SetDailyWireTransferLimitCommand
            {
                AccountId = AccountId,
                DailyWireTransferLimit = 1000m
            });

            var cmd = new WithdrawCashCommand()
            {
                AccountId = AccountId,
                Amount = 200m
            };
            Assert.IsType<Success>(CmdHandler.Handle(cmd));
        }

        [Fact]
        public void CannotWithdrawCashFromInvalidAccount()
        {
            var cmd = new WithdrawCashCommand()
            {
                AccountId = AccountId,
                Amount = 200m
            };
            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("No account with this ID exists", msg.Exception.Message);
        }

        [Fact]
        public void CannotWithdrawFromBlockedAccount()
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

            var cmd = new WithdrawCashCommand()
            {
                AccountId = AccountId,
                Amount = 10m
            };
            CmdHandler.Handle(cmd);
            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("Account is blocked", msg.Exception.Message);
        }

    }
}
