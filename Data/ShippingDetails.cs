using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace mmaAPI.Data
{
    public partial class ShippingDetails
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public decimal? ShippingMethod { get; set; }
        public decimal? ShippingCost { get; set; }
        public int? Status { get; set; }
        public int? DeliveryAddressId { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public string ReceipientDetails { get; set; }

        public virtual UserAddresses DeliveryAddress { get; set; }
        public virtual Orders Order { get; set; }
    }
}
