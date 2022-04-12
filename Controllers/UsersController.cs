using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using mmaAPI.Data;
using mmaAPI.Dtos;
using mmaAPI.Helpers;

namespace mmaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MMADBContext _context;
        private readonly AppSettings _appSettings;

        public UsersController(MMADBContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUsers(int id)
        {
            var user = await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [Route("withaddresses/{id}")]
        [HttpGet]
        public async Task<ActionResult<Users>> GetCustomersWithAddresses(int id)
        {
            var user = await _context.Users.Include(x => x.UserAddresses).Where(x => x.Id == id).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers(int id, Users users)
        {
            if (id != users.Id)
            {
                return BadRequest();
            }

            _context.Entry(users).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Users>> PostUsers(Users users)
        {
            _context.Users.Add(users);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsers", new { id = users.Id }, users);
        }

        [Route("auth/login")]
        [HttpPost]
        public async Task<ActionResult<Users>> Login(LoginDto login)
        {
            var user = await _context.Users.Where(x => x.Email == login.User || x.Phone == login.User && x.Active == true).FirstOrDefaultAsync();
            if (user == null) return NotFound();
            
            if(user.Password == login.Password)
            {
                //login user
                // authentication successful so generate jwt token
                var token = await generateJwtToken(user);
                //return Ok(new { Token = token, Email = user.Email, Phone = user.Phone, Username = user.Username, Id = user.Id, FirstName = user.FirstName, LastName = user.LastName});
                return Ok(new { Token = token });

            }
            return BadRequest();
            //_context.Users.Add(users);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetUsers", new { id = users.Id }, users);
        }

        [Route("auth/register")]
        [HttpPost]
        public async Task<IActionResult> Register (NewAccountDto data)
        {
            var user = new Users { FirstName = data.FirstName, LastName = data.LastName, Email = data.Email, Password = data.Password, Phone = data.Phone, Active = true, DateCreated = DateTime.Now, CreatedBy = "SELF" };
            user.Username = data.Email.Substring(0,data.Email.IndexOf('@'));
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            if(user?.Id > 0)
            {
                var token = await generateJwtToken(user);
                return Ok(new { Token = token });
            }
            //return Ok(new { Email = user.Email, Phone = user.Phone, Username = user.Username, Id = user.Id, FirstName = user.FirstName, LastName = user.LastName });
            return BadRequest();
        }

        [Authorize]
        [Route("address/new")]
        [HttpPost]
        public async Task<IActionResult> AddNewAddress(NewAddressDto data)
        {
            var user = (Users)HttpContext.Items["User"];
            if (user == null) return BadRequest();

            var address = new UserAddresses { FullName = data.FullName, Phone = data.Phone, City = data.City, Country = data.Country, Region = data.Region, Description = data.Description, StreetAddress = data.StreetAddress, DigitalAddress = data.DigitalAddress, DateCreated = DateTime.Now, UserId = user.Id };
            _context.UserAddresses.Add(address);
            await _context.SaveChangesAsync();
            var res = await _context.Users.Include(x => x.UserAddresses).Where(x => x.Id == user.Id).FirstOrDefaultAsync();
            return Ok(res);
        }

        [Authorize]
        [Route("orders/all")]
        [HttpGet]
        public async Task<IActionResult> GetAllCustomerOrders()
        {
            var user = (Users)HttpContext.Items["User"];
            if (user == null) return BadRequest();

            var orders = await _context.Orders.Include(x=>x.OrderItems).Where(o => o.UserId == user.Id).OrderByDescending(x=>x.Id).ToListAsync();
            if (orders == null) return NotFound();
            var orderItems = new List<OrderOverviewDto>();
            foreach(var item in orders)
            {
                foreach(var ordrItm in item.OrderItems)
                {
                    var orderItm = new OrderOverviewDto { DateCreated = item.DateCreated, OrderId = item.Id, ProductName = ordrItm.ProductName };
                    orderItm.ImageUrl = _context.ProductImages.Where(x => x.ProductId == ordrItm.ProductId).FirstOrDefault()?.ImageUrl;
                    orderItm.Status = _context.OrderStatus.Where(x => x.Id == item.Status).FirstOrDefault()?.Status;

                    orderItems.Add(orderItm);
                }
            }
            return Ok(new { Orders = orderItems });
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Users>> DeleteUsers(int id)
        {
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            _context.Users.Remove(users);
            await _context.SaveChangesAsync();

            return users;
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private async Task<string> generateJwtToken(Users user)
        {
            // generate token that is valid for 1 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var authClaims = new List<Claim> {
                    new Claim("id", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("first_name", user.FirstName),
                    new Claim("last_name", user.LastName)
                };
            var userRole = _context.UserRoles.Include(r=>r.Role).Where(x => x.UserId == user.Id).FirstOrDefault();
            //foreach (var userRole in user.UserRoles)
            //{
            //    authClaims.Add(new Claim(ClaimTypes.Role, userRole.Role.Role));
            //}
            if (userRole != null)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole.Role.Role));
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
