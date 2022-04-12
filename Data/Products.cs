using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace mmaAPI.Data
{
    public partial class Products
    {
        public Products()
        {
            ProductDiscount = new HashSet<ProductDiscount>();
            ProductExtraDetails = new HashSet<ProductExtraDetails>();
            ProductImages = new HashSet<ProductImages>();
        }

        public int Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public int? CategoryId { get; set; }
        public int? InventoryId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string Remarks { get; set; }

        public virtual Categories Category { get; set; }
        public virtual ProductInventory Inventory { get; set; }
        public virtual ICollection<ProductDiscount> ProductDiscount { get; set; }
        public virtual ICollection<ProductExtraDetails> ProductExtraDetails { get; set; }
        public virtual ICollection<ProductImages> ProductImages { get; set; }
    }
}
