using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace mmaAPI.Data
{
    public partial class ProductInventory
    {
        public ProductInventory()
        {
            Products = new HashSet<Products>();
        }

        public int Id { get; set; }
        public int? Quantity { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public virtual ICollection<Products> Products { get; set; }
    }
}
