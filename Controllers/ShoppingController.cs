using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mmaAPI.Data;
using mmaAPI.Dtos;
using mmaAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mmaAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private readonly MMADBContext _context;

        public ShoppingController(MMADBContext context)
        {
            _context = context;
        }

        [Authorize]
        [Route("calculate/shippingcost")]
        [HttpGet]
        public async Task<IActionResult> CalculateShippingCost(int addressId)
        {
            const decimal SHIPPINGWITHINACCRA = 15m;
            //check address
            var user = (Users)HttpContext.Items["User"];
            var shippingCost = SHIPPINGWITHINACCRA + 0;
            return Ok(shippingCost);
        }

        [Authorize]
        [Route("add/shippingaddress")]
        [HttpPost]
        public async Task<IActionResult> AddShippingAddress(NewAddressDto data)
        {
            var user = (Users)HttpContext.Items["User"];
            if (user == null) return Forbid();

            var address = new UserAddresses { FullName = data.FullName, Phone = data.Phone, City = data.City, Country = data.Country, Region = data.Region, Description = data.Description, StreetAddress = data.StreetAddress, DigitalAddress = data.DigitalAddress, DateCreated = DateTime.Now, UserId = user.Id, IsDefault = data.IsDefault };
            _context.UserAddresses.Add(address);
            try
            {
                await _context.SaveChangesAsync();

                return Ok(address.Id);
            }
            catch (Exception e)
            {
                return BadRequest();
            }

        }

        [Authorize]
        [Route("place/order")]
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(NewOrderDto data)
        {
            //var mp = data.ToDictionary(x => x, x => x);

            var user = (Users)HttpContext.Items["User"];
            if (user == null) return Unauthorized();
            var orders = data.Orders;
            var ids = new List<int>();
            //grab all the ids so we fetch their products with 'select in' statement
            foreach (var d in orders)
            {
                ids.Add(d.ProductId);
            }
            //var ids = data.ProductIds;
            //List<Products> products = new List<Products>();
            var newOrder = new Orders();
            newOrder.DateCreated = DateTime.Now;
            newOrder.UserId = user.Id;
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();
            List<OrderItems> orderItems = new List<OrderItems>();
            decimal? totalAmount = 0m;
            var products = await _context.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
            foreach (var product in products)
            {
                var qty = orders.Where(p => p.ProductId == product.Id).FirstOrDefault().Quantity;
                orderItems.Add(new OrderItems { ProductId = product.Id, ProductName = product.Name, Quantity = qty, UnitPrice = product.Price, OrderId = newOrder.Id });
                totalAmount += product.Price * qty;
            }
            newOrder.TotalAmount = totalAmount;
            newOrder.Status = 1; // Pending
            _context.OrderItems.AddRange(orderItems);

            var shippingAddress = await _context.UserAddresses.Where(x => x.UserId == user.Id && x.Id == data.ShippingAddressId).FirstOrDefaultAsync();

            //let save shipping details
            var shipping = new ShippingDetails();
            shipping.DeliveryAddressId = data.ShippingAddressId;
            shipping.EstimatedDeliveryDate = DateTime.Now.AddDays(3);
            shipping.OrderId = newOrder.Id;
            shipping.ReceipientDetails = $"{shippingAddress?.FullName};{shippingAddress?.Phone}";
            shipping.Status = 1; //PENDING HIPPING

            _context.ShippingDetails.Add(shipping);

            //save order payment details too
            var orderPayment = new OrderPaymentDetails();
            orderPayment.AmountPayable = newOrder.TotalAmount;
            orderPayment.OrderId = newOrder.Id;
            orderPayment.DateCreated = DateTime.Now;
            orderPayment.Status = 1; //Awaiting payment

            _context.OrderPaymentDetails.Add(orderPayment);

            await _context.SaveChangesAsync();


            var order = await _context.Orders.Include(o => o.OrderItems).Where(x => x.Id == newOrder.Id).FirstOrDefaultAsync();


            return Ok(order);
        }

        [Authorize]
        [Route("order/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var user = (Users)HttpContext.Items["User"];
            if (user == null) return Unauthorized();

            if (id <= 0) return BadRequest();

            var order = await _context.Orders.Include(o => o.OrderItems).Include(p => p.OrderPaymentDetails).Include(p => p.ShippingDetails).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (order == null) return NotFound();
            var orderItems = new List<OrderOverviewDto>();

            foreach (var ordrItm in order.OrderItems)   
            {
                var orderItm = new OrderOverviewDto { DateCreated = order.DateCreated, OrderId = order.Id, ProductName = ordrItm.ProductName, Quantity = ordrItm.Quantity, UnitPrice = ordrItm.UnitPrice };
                orderItm.ImageUrl = _context.ProductImages.Where(x => x.ProductId == ordrItm.ProductId).FirstOrDefault()?.ImageUrl;
                orderItm.Status = _context.OrderStatus.Where(x => x.Id == order.Status).FirstOrDefault()?.Status;

                orderItems.Add(orderItm);
            }

            var address = new UserAddresses();
            if (order.ShippingDetails.Count() > 0)
            {
                address = await _context.UserAddresses.Where(x => x.Id == order.ShippingDetails.FirstOrDefault().DeliveryAddressId).FirstOrDefaultAsync();
            }

            //var paymentDetails = await _context.OrderPaymentDetails.Where(x=>x.Id == order.OrderPaymentDetails.FirstOrDefault().)

            return Ok(new { Order = order, OrderItems = orderItems, ShippingAddress = address, UserEmail = user.Email });
        }

        [Authorize]
        [Route("order/fulfillpayment")]
        [HttpPut]
        public async Task<IActionResult> FulfillPayment(FulFillPaymentDto data)
        {
            var uuid = HttpContext.Items["uuid"]?.ToString();
            var user = (Users)HttpContext.Items["User"];
            if (user == null) return BadRequest();

            if (data.OrderId <= 0) return BadRequest();

            var order = await _context.Orders.Include(o => o.OrderItems).Where(x => x.Id == data.OrderId).FirstOrDefaultAsync();

            if (order != null)
            {
                order.Status = 2;//paid, awaiting shipment
            }

            var orderPayment = await _context.OrderPaymentDetails.Where(x => x.OrderId == data.OrderId).FirstOrDefaultAsync();
            if (orderPayment != null)
            {
                orderPayment.Status = 3; //payment complete  
                orderPayment.ReferenceId = data.Reference;
                orderPayment.Remarks = data.Response;
            }

            //reconcile product qty
            //get qty from order items then debit accordingly

            var ids = new List<int>();

            var orderItems = order.OrderItems;
            foreach (var item in orderItems)
            {
                ids.Add(item.ProductId);
            }
            var products = await _context.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
            foreach (var product in products)
            {
                var orderItem = orderItems.Where(x => x.ProductId == product.Id).FirstOrDefault();
                product.Quantity = product.Quantity - orderItem.Quantity;
            }

            EmptyCart(uuid);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [Route("order/cancel/{id}")]
        [HttpPatch]
        public async Task<IActionResult> CancelOrder(int id)
        {
            if (id <= 0) return BadRequest();
            var order = await _context.Orders.FindAsync(id);
            order.Status = 6;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize]
        [Route("wishlist/additem")]
        [HttpPost]
        public async Task<IActionResult> AddItemToFavorites(AddWishlistItemDto data)
        {
            if (data.ProductId <= 0) return BadRequest();

            var user = (Users)HttpContext.Items["User"];
            if (user == null) return BadRequest();
            //if user already has item as fav ignore
            var itm = _context.UserWishlists.Where(x => x.UserId == user.Id && x.ProductId == data.ProductId).FirstOrDefault();
            if (itm != null) return BadRequest("Item already exists.");
            try
            {
                var favorite = new UserWishlists { ProductId = data.ProductId, UserId = user.Id, DateCreated = DateTime.Now };
                _context.UserWishlists.Add(favorite);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }     

            return Ok();
        }

        [Authorize]
        [Route("wishlist/items")]
        [HttpGet]
        public async Task<IActionResult> GetItemsInWishlistt()
        {

            var user = (Users)HttpContext.Items["User"];
            if (user == null) return BadRequest();

            var userfavorites = await _context.UserWishlists.Where(x => x.UserId == user.Id) .Select(c=>c.ProductId).ToListAsync();
            var products = await _context.Products.Where(p => userfavorites.Contains(p.Id)).Include(x=>x.ProductImages).ToListAsync();
            return Ok(products);
        }

        [Authorize]
        [Route("wishlist/removeitem/{id}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveFromWishlist(int id)
        {
            if (id <= 0) return BadRequest();
            _context.UserWishlists.Remove(_context.UserWishlists.Where(x => x.ProductId == id).FirstOrDefault());
            await _context.SaveChangesAsync();
            return Ok();
        }


        [Route("cart/additems")]
        [HttpPost]
        public async Task<IActionResult> AddItemsToCart(CartItemDto data)
        {
            var uuid = HttpContext.Items["uuid"]?.ToString();
            if(string.IsNullOrWhiteSpace(uuid)) //it's probably a new session
            {
                uuid = Guid.NewGuid().ToString();
            }           
            
            
            var item = await _context.SessionCartItems.Where(x => x.ProductId == data.ProductId && x.SessionId == uuid).FirstOrDefaultAsync();
            if(item != null) //item already exists for this session. just update the quantity
            {
                item.Quantity += data.Quantity;
            }
            else
            {
                var cartItem = new SessionCartItems { ProductId = data.ProductId, ProductName = data.ProductName, Quantity = data.Quantity, UnitPrice = data.UnitPrice, ImageUrl = data.ImageUrl, DateCreated = DateTime.Now };
                cartItem.SessionId = uuid;
                _context.Add(cartItem);
            }
            await _context.SaveChangesAsync();
            var userShoppingitems = await _context.SessionCartItems.Where(x => x.SessionId == uuid).ToListAsync();
            return Ok(new { Uuid = uuid, CartItems = userShoppingitems});


        }

        [Route("cart/items")]
        [HttpGet]
        public async Task<IActionResult> GetItemsInCart()
        {
            
            var uuid = HttpContext.Items["uuid"]?.ToString();
            
            var userShoppingitems = await _context.SessionCartItems.Where(x => x.SessionId == uuid).ToListAsync();
           
            return Ok(userShoppingitems);            
        }

        [Route("cart/removeitem/{id}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveItem(int id)
        {
            if (id <= 0) return BadRequest();
            var uuid = HttpContext.Items["uuid"]?.ToString();
            _context.SessionCartItems.Remove(_context.SessionCartItems.Where(x => x.SessionId == uuid && x.ProductId == id).FirstOrDefault());
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Route("cart/items/empty")]
        [HttpDelete]
        public async Task<IActionResult> EmptyCartItems()
        {
            var uuid = HttpContext.Items["uuid"]?.ToString();
            EmptyCart(uuid);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private void EmptyCart(string uuid)
        {
            _context.SessionCartItems.RemoveRange(_context.SessionCartItems.Where(x => x.SessionId == uuid));
        }

        private bool ItemExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
