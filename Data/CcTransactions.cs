using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace mmaAPI.Data
{
    public partial class CcTransactions
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int? PaymentOrderId { get; set; }
        public string Processor { get; set; }
        public int? CcNumber { get; set; }
        public string CcType { get; set; }
        public string Response { get; set; }
        public string Remarks { get; set; }
        public DateTime? TransactionDate { get; set; }

        public virtual OrderPaymentDetails PaymentOrder { get; set; }
    }
}
