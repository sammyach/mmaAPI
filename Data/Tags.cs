using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace mmaAPI.Data
{
    public partial class Tags
    {
        public int Id { get; set; }
        public string TagName { get; set; }
        public string Description { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastDateModified { get; set; }
        public string ModifiedBy { get; set; }
    }
}
