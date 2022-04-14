using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using mmaAPI.Data;
using mmaAPI.Dtos;
using mmaAPI.Helpers;
using mmaAPI.Services;

namespace mmaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly MMADBContext _context;
        private readonly IFileService _fileService;

        public ProductController(MMADBContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> GetProductItems()
        {
            var user = (Users)HttpContext.Items["User"];
            
            //HttpContext.Session.SetString("XXXID", "123456789");
            //await HttpContext.Session.LoadAsync();
            //var session = HttpContext.Session;
            return await _context.Products.Include(p=>p.ProductImages).Include(c=>c.Category).ToListAsync();
        }

        [HttpGet]
        [Route("category/{id}")]
        public async Task<ActionResult<IEnumerable<Products>>> GetProductByCategory(int? id)
        {
            var user = (Users)HttpContext.Items["User"];

            var products = new List<Products>();
            if (id != null && id > 0)
            {
                products = await _context.Products.Where(x => x.CategoryId == id).Include(p => p.ProductImages).Include(c => c.Category).ToListAsync();
            }
            else
            {
                products = await _context.Products.Include(p => p.ProductImages).Include(c => c.Category).ToListAsync();
            }
            //HttpContext.Session.SetString("XXXID", "123456789");
            //await HttpContext.Session.LoadAsync();
            //var session = HttpContext.Session;
            return Ok(products);
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> GetProductItems(int id)
        {
            var productItem = await _context.Products.Where(p=>p.Id == id).Include(c=>c.ProductImages).FirstOrDefaultAsync();

            if (productItem == null)
            {
                return NotFound();
            }

            return productItem;
        }

        // PUT: api/Product/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductItems(int id, Products productItems)
        {
            if (id != productItems.Id)
            {
                return BadRequest();
            }

            _context.Entry(productItems).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductItemsExists(id))
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

        // POST: api/Product
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Products>> PostProductItems(Products productItems)
        {
            _context.Products.Add(productItems);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductItems", new { id = productItems.Id }, productItems);
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Products>> DeleteProductItems(int id)
        {
            var productItems = await _context.Products.FindAsync(id);
            if (productItems == null)
            {
                return NotFound();
            }

            _context.Products.Remove(productItems);
            await _context.SaveChangesAsync();

            return productItems;
        }

        [Route("item/search")]
        [HttpGet]
        public async Task <IActionResult> SearchItem(string q)
        {
            var items = await _context.Products.Where(x => x.Name.Contains(q)).Select(x => (new { ProductName = x.Name, ProductId = x.Id, CategoryId = x.CategoryId })).ToListAsync();
            return Ok(items);
        }

        [Route("new")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateItemDto data)
        {
            var user = (Users)HttpContext.Items["User"];
            if (user == null) return Unauthorized();

            var product = new Products { Name = data.Name, Description = data.Description, Price = data.Price, Quantity = data.Quantity, DateCreated = DateTime.Now, CategoryId = data.CategoryId  };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        [Route("update")]
        [HttpPatch]
        public async Task <IActionResult> UpdateProduct(UpdateItemDto data)
        {
            var product = await _context.Products.FindAsync(data.Id);
            product.Name = data.Name;
            product.Description = data.Description;
            product.Price = data.Price;
            product.Quantity = data.Quantity;

            await _context.SaveChangesAsync();

            return Ok(product);
        }

        [Authorize]
        [Route("images/upload/{id}/files")]
        [HttpPost]
        public async Task <IActionResult> UploadImage(int id, [FromForm(Name = "files")] List<IFormFile> files) //([FromForm(Name = "file")] IFormFileCollection formFile)
        {
            //var formFiles = new 
            try
            {
                string subDirectory = "UploadedImg";
                var productImgs = new List<ProductImages>();
                var imgUrls = _fileService.UploadFiles(files, subDirectory);
                foreach(var imgUrl in imgUrls)
                {
                    var productImg = new ProductImages { ProductId = id, ImageUrl = imgUrl };
                    productImgs.Add(productImg);
                }

                _context.ProductImages.AddRange(productImgs);
                await _context.SaveChangesAsync();

                var product = await _context.Products.Include(p => p.ProductImages).Where(x => x.Id == id).FirstOrDefaultAsync();
                return Ok(product);
                //return Ok(new { formFiles.Count, Size = _fileService.SizeConverter(formFiles.Sum(f => f.Length)) });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [Route("images/remove/{id}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveImage(int id)
        {
            if (id <= 0) return BadRequest();

            var prodImg = _context.ProductImages.Find(id);
            _context.ProductImages.Remove(prodImg);
            await _context.SaveChangesAsync();

            string subDirectory = "UploadedImg";
            _fileService.DeleteFile(prodImg.ImageUrl, subDirectory);
            return Ok();
        }

        private bool ProductItemsExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
