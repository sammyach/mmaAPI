using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace mmaAPI.Data
{
    public partial class OrderPaymentDetails
    {
        public OrderPaymentDetails()
        {
            CcTransactions = new HashSet<CcTransactions>();
            MomoTransactions = new HashSet<MomoTransactions>();
        }

        public int Id { get; set; }
        public int? OrderId { get; set; }
        public decimal? AmountPayable { get; set; }
        public int? Status { get; set; }
        public int? PaymentTypeId { get; set; }
        public decimal? AmountReceived { get; set; }
        public DateTime? DateCreated { get; set; }
        public string ReferenceId { get; set; }
        public string Remarks { get; set; }

        public virtual Orders Order { get; set; }
        public virtual PaymentType PaymentType { get; set; }
        public virtual OrderPaymentStatus StatusNavigation { get; set; }
        public virtual ICollection<CcTransactions> CcTransactions { get; set; }
        public virtual ICollection<MomoTransactions> MomoTransactions { get; set; }
    }
}
