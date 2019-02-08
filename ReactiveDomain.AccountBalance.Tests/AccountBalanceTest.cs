using System;
using Xunit;

namespace ReactiveDomain.AccountBalance.Tests
{
    public class AccountBalanceTest
    {
        //[Fact]
        //public void Account()
        //{
        //    var ExpectedResult = new
        //    {
        //        Id = Guid.Parse("06ac5641-ede6-466f-9b37-dd8304d05a84"),
        //        _balance = 1050,
        //        _dailyWireTransferLimit = 300,
        //        _overDraftLimit = 500,
        //        _state = "Active",
        //    };

        //    var app = new Application();
        //    app.Bootstrap();
        //    Account acct;
        //    acct = app.repo.GetById<Account>(app._accountId);
        //    acct.Credit(1200);
        //    acct.SetDailyWireTransferLimit(300);
        //    acct.SetOverDraftLimit(500);
        //    acct.Debit(150);

        //    Assert.Equal(ExpectedResult._balance,acct.Balance);
        //    Assert.Equal(ExpectedResult._dailyWireTransferLimit,acct.DailyWireTransferLimit);
        //    Assert.Equal(ExpectedResult._overDraftLimit,acct.OverDraftLimit);
        //    Assert.Equal(ExpectedResult._state,acct.State);
        //}

        [Fact]
        public void ShouldCredit()
        {
            var ExpectedResult = 1200;
            var amount = (uint)1200;
            var app = new Application();
            app.Bootstrap();
            Account acct;
            acct = app.repo.GetById<Account>(app._accountId);
            acct.Credit(amount);
            Assert.Equal(ExpectedResult, acct.Balance);
        }

        [Fact]
        public void ShouldSetOverDraftLimit()
        {
            var ExpectedResult = 300;
            var limit = (uint)300;
            var app = new Application();
            app.Bootstrap();
            Account acct;
            acct = app.repo.GetById<Account>(app._accountId);
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
            Account acct;
            acct = app.repo.GetById<Account>(app._accountId);
            acct.SetDailyWireTransferLimit(limit);
            Assert.Equal(ExpectedResult, acct.DailyWireTransferLimit);
        }

        [Fact]
        public void TestingDailyLimitReached()
        {
            var limit = (uint)500;
            var app = new Application();
            app.Bootstrap();
            Account acct;
            acct = app.repo.GetById<Account>(app._accountId);
            acct.SetDailyWireTransferLimit(limit);
            acct.Credit(501);
            acct.Debit(250);

            var ex = Assert.Throws<ArgumentException>(() =>
             {
                 acct.Debit(251);
             });
            Assert.Null(ex.InnerException);
        }

        [Theory]
        [InlineData(500, "Active")]
        [InlineData(501, "Blocked")]
        public void TestingOverDraftLimit(uint amount, string state)
        {
            var limit = (uint)500;
            var app = new Application();
            app.Bootstrap();
            Account acct;
            acct = app.repo.GetById<Account>(app._accountId);
            acct.SetOverDraftLimit(limit);
            //to avoid daily limit exception
            acct.SetDailyWireTransferLimit(1000); 
            acct.Debit(amount);
            Assert.Equal(state, acct.State);
        }
    }
}
