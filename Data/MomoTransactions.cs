using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace mmaAPI.Data
{
    public partial class MomoTransactions
    {
        public int Id { get; set; }
        public int? OrderPaymentId { get; set; }
        public string Processor { get; set; }
        public string PhoneNumber { get; set; }
        public string Response { get; set; }
        public string Code { get; set; }
        public DateTime? TransactionDate { get; set; }

        public virtual OrderPaymentDetails OrderPayment { get; set; }
    }
}
