using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace mmaAPI.Data
{
    public partial class Orders
    {
        public Orders()
        {
            OrderItems = new HashSet<OrderItems>();
            OrderPaymentDetails = new HashSet<OrderPaymentDetails>();
            ShippingDetails = new HashSet<ShippingDetails>();
        }

        public int Id { get; set; }
        public int? UserId { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? Status { get; set; }
        public string Remarks { get; set; }
        public DateTime? DateCreated { get; set; }

        public virtual OrderStatus StatusNavigation { get; set; }
        public virtual Users User { get; set; }
        public virtual ICollection<OrderItems> OrderItems { get; set; }
        public virtual ICollection<OrderPaymentDetails> OrderPaymentDetails { get; set; }
        public virtual ICollection<ShippingDetails> ShippingDetails { get; set; }
    }
}
