using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mmaAPI.Dtos
{
    public class LoginDto
    {
        //[Required]
        public string User { get; set; }
        //[Required]
        public string Password { get; set; }
    }

    public class NewAccountDto
    {
        //[Required]
        public string FirstName { get; set; }
        //[Required]
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
    }

    public class NewAddressDto
    {
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        //public int UserId { get; set; }
        public string StreetAddress { get; set; }
        public string DigitalAddress { get; set; }
        public bool? IsDefault { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
    }

    public class NewOrder
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }        
    }

    public class NewOrderDto
    {
        public List<NewOrder> Orders { get; set; }
        public int ShippingAddressId { get; set; }
    }

    public class FulFillPaymentDto
    {
        public int OrderId { get; set; }
        public string Reference { get; set; }
        public string Response { get; set; }
        //public decimal? AmountPaid { get; set; }
    }

    public class AddCartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemDto
    {        
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public string ImageUrl { get; set; }       
    }

    public class OrderOverviewDto
    {
        public string ImageUrl { get; set; }
        public string ProductName { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
    }

    public class SearchItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }

    }

    public class AddWishlistItemDto
    {
        public int ProductId { get; set; }
    }

    public class CreateItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }

    public class UpdateItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public string Remarks { get; set; }
        
    }


}
