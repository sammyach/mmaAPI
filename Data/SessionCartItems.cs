using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace mmaAPI.Data
{
    public partial class SessionCartItems
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
