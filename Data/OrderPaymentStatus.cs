using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace mmaAPI.Data
{
    public partial class OrderPaymentStatus
    {
        public OrderPaymentStatus()
        {
            OrderPaymentDetails = new HashSet<OrderPaymentDetails>();
        }

        public int Id { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }

        public virtual ICollection<OrderPaymentDetails> OrderPaymentDetails { get; set; }
    }
}
