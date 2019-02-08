using System;
using Xunit;

namespace ReactiveDomain.AccountBalance.Tests
{
    public class AccountBalanceTest
    {
        [Fact]
        public void Account()
        {
            //var ExpectedResult = new
            //{
            //    Id = Guid.Parse("06ac5641-ede6-466f-9b37-dd8304d05a84"),
            //    _balance = 1050,
            //    _dailyWireTransferLimit = 300,
            //    _overDraftLimit = 500,
            //    _state = "Active",
            //    _withdrawnToday = 150
            //};

            var app = new Application();
            app.Bootstrap();
            Account acct;
            acct = app.repo.GetById<Account>(app._accountId);
            acct.Credit(1200);
            acct.SetDailyWireTransferLimit(300);
            acct.SetOverDraftLimit(500);
            acct.Debit(150);
            app.repo.Save(acct);

            acct = app.repo.GetById<Account>(app._accountId);
            
           
        }
    }
}
