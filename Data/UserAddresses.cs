using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace mmaAPI.Data
{
    public partial class UserAddresses
    {
        public UserAddresses()
        {
            ShippingDetails = new HashSet<ShippingDetails>();
        }

        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? TypeId { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string StreetAddress { get; set; }
        public string DigitalAddress { get; set; }
        public bool? IsDefault { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public virtual AddressTypes Type { get; set; }
        public virtual Users User { get; set; }
        public virtual ICollection<ShippingDetails> ShippingDetails { get; set; }
    }
}
