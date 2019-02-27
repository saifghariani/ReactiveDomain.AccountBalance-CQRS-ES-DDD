using System;
using System.Collections.Generic;
using System.Text;

namespace ReactiveDomain.AccountBalance
{
    public class Account
    {
        public string Id { get; set; }
        public string HolderName { get; set; }
        public string State { get; set; }
        public decimal Balance { get; set; }
        public decimal DailyWireTransferLimit { get; set; }
        public decimal OverDraftLimit { get; set; }
    }
}
