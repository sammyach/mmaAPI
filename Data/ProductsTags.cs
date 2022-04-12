using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace mmaAPI.Data
{
    public partial class ProductsTags
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? TagId { get; set; }
    }
}
