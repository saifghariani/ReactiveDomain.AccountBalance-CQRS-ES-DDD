using System;
using System.Collections.Generic;
using System.Text;
using ReactiveDomain.AccountBalance.Commands;
using ReactiveDomain.Messaging;
using Xunit;

namespace ReactiveDomain.AccountBalance.Tests
{
    public class DepositCashTests : AccountTestsBase
    {
        [Theory]
        [InlineData(0.01)]
        [InlineData(100)]
        [InlineData(100000000)]
        public void CanDepositCash(decimal amount)
        {
            
            CmdHandler.Handle(new CreateAccountCommand
            {
                AccountId = AccountId,
                HolderName = "John Doe"
            });

            var cmd = new DepositCashCommand
            {
                AccountId = AccountId,
                Amount = amount
            };
            Assert.IsType<Success>(CmdHandler.Handle(cmd));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-0.01)]
        [InlineData(-100)]
        [InlineData(-100000000)]
        public void CannotDepositCashWithNonPositiveAmount(decimal amount)
        {
            CmdHandler.Handle(new CreateAccountCommand
            {
                AccountId = AccountId,
                HolderName = "John Doe"
            });

            var cmd = new DepositCashCommand
            {
                AccountId = AccountId,
                Amount = amount
            };
            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("Cannot deposit a negative amount", msg.Exception.Message);
        }

        [Fact]
        public void CannotDepositCashIntoInvalidAccount()
        {
            var cmd = new DepositCashCommand
            {
                AccountId = AccountId,
                Amount = Convert.ToDecimal(100m)
            };
            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("No account with this ID exists", msg.Exception.Message);

        }
    }
}
