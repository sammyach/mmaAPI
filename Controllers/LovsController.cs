using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mmaAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mmaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LovsController : ControllerBase
    {
        private readonly MMADBContext _context;

        public LovsController(MMADBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Route("orderstatus/all")]
        public async Task<IActionResult> GetAllOrderStatus()
        {
            var data = await _context.OrderStatus.ToListAsync();
            return Ok(data);
        }        
    }
}
