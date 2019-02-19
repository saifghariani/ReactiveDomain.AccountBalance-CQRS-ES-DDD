using System;
using Xunit;

namespace ReactiveDomain.AccountBalance.Tests
{
    public class AccountBalanceTest
    {
        
        [Fact]
        public void AccountCreated()
        {
            var app = new Application();
            app.Bootstrap();
            AccountAggregate acct;
            acct = app.repo.GetById<AccountAggregate>(app._accountId);
            Assert.NotNull(acct);
        }

        [Fact]
        public void ShouldCreditCash()
        {
            var ExpectedResult = 1200;
            var amount = (uint)1200;
            var app = new Application();
            app.Bootstrap();
            AccountAggregate acct;
            acct = app.repo.GetById<AccountAggregate>(app._accountId);
            acct.Credit(amount, "cash");
            Assert.Equal(ExpectedResult, acct.Balance);
        }

        [Fact]
        public void ShouldCreditCheck()
        {
            var ExpectedResult = 1200;
            var amount = (uint)1200;
            var app = new Application();
            app.Bootstrap();
            AccountAggregate acct;
            acct = app.repo.GetById<AccountAggregate>(app._accountId);
            acct.Credit(amount, "check");
            Assert.Equal(ExpectedResult, acct.Balance);
        }

        [Fact]
        public void ShouldDebit()
        {
            var ExpectedResult = 1000;
            var amount = (uint)200;
            var app = new Application();
            app.Bootstrap();
            AccountAggregate acct;
            acct = app.repo.GetById<AccountAggregate>(app._accountId);
            //to avoid daily Limit
            acct.SetDailyWireTransferLimit(1000);
            acct.Credit(1200, "cash");
            acct.Debit(amount);
            Assert.Equal(ExpectedResult, acct.Balance);
        }

        [Fact]
        public void ShouldSetOverDraftLimit()
        {
            var ExpectedResult = 300;
            var limit = (uint)300;
            var app = new Application();
            app.Bootstrap();
            AccountAggregate acct;
            acct = app.repo.GetById<AccountAggregate>(app._accountId);
            acct.SetOverDraftLimit(limit);
            Assert.Equal(ExpectedResult, acct.OverDraftLimit);
        }

        [Fact]
        public void ShouldSetDailyWireTransferLimit()
        {
            var ExpectedResult = 500;
            var limit = (uint)500;
            var app = new Application();
            app.Bootstrap();
            AccountAggregate acct;
            acct = app.repo.GetById<AccountAggregate>(app._accountId);
            acct.SetDailyWireTransferLimit(limit);
            Assert.Equal(ExpectedResult, acct.DailyWireTransferLimit);
        }

        [Fact]
        public void TestingDailyLimitReached()
        {
            var limit = (uint)500;
            var ExpectedResult = "Blocked";
            var app = new Application();
            app.Bootstrap();
            AccountAggregate acct;
            acct = app.repo.GetById<AccountAggregate>(app._accountId);
            acct.SetDailyWireTransferLimit(limit);
            acct.Credit(501, "cash");
            acct.Debit(250);
            acct.Debit(251);
            Assert.Equal(ExpectedResult, acct.State);
            
        }

        [Theory]
        [InlineData(500, "Active")]
        [InlineData(501, "Blocked")]
        public void TestingOverDraftLimit(uint amount, string state)
        {
            var limit = (uint)500;
            var app = new Application();
            app.Bootstrap();
            AccountAggregate acct;
            acct = app.repo.GetById<AccountAggregate>(app._accountId);
            acct.SetOverDraftLimit(limit);
            //to avoid daily limit exception
            acct.SetDailyWireTransferLimit(1000); 
            acct.Debit(amount);
            Assert.Equal(state, acct.State);
        }

        [Fact]
        public void ShouldUnblockAccount()
        {
            var limit = (uint)500;
            var ExpectedResult = "Active";
            var app = new Application();
            app.Bootstrap();
            AccountAggregate acct;
            acct = app.repo.GetById<AccountAggregate>(app._accountId);
            acct.SetOverDraftLimit(limit);
            //to avoid daily limit exception
            acct.SetDailyWireTransferLimit(1000);
            //to block account
            acct.Debit(501);
            Assert.Equal("Blocked", acct.State);

            acct.Credit(501, "cash");
            Assert.Equal("Active", acct.State);

        }
    }
}
