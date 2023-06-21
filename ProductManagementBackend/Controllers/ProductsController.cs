using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementBackend.Models;

namespace ProductManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseController
    {
        private readonly ProductManagementContext _context;
        private readonly IConfiguration _config;

        public ProductsController(ProductManagementContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: api/Products
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromForm] ProductRequestPut model)
        {
            var product = await _context.Products.FindAsync(id);
            Console.WriteLine(model);

            if (product == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(model.Title))
            {
                product.Title = model.Title;
            }

            if (model.Price != null)
            {
                product.Price = (decimal)model.Price;
            }

            if (!string.IsNullOrEmpty(model.Description))
            {
                product.Description = model.Description;
            }


            if (model.Image != null)
            {
                // Check if the product has an existing image
                if (!string.IsNullOrEmpty(product.Image))
                {
                    // Delete the old image file from storage
                    DeleteImage(product.Image);
                }

                // Save the new image file to storage
                string imagePath = SaveImage(model.Image);

                // Update the product with the new image path
                product.Image = imagePath;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(product);
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromForm] ProductRequestPost model)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ProductManagementContext.Products'  is null.");
            }

            string imagePath = SaveImage(model.Image); // Save the image file and get the stored path

            // Create a new product instance
            Product product = new Product
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                Image = imagePath // Assuming you have a property named "ImagePath" in the Product model
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }


        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(product.Image))
            {
                // Delete the old image file from storage
                DeleteImage(product.Image);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string SaveImage(IFormFile imageFile)
        {
            // Get the path where you want to store the image
            var randomName = Path.GetRandomFileName() + "_" + imageFile.FileName;
            string storedFilesPath = _config["StoredFilesPath"];

            if (!Directory.Exists(storedFilesPath))
            {
                Directory.CreateDirectory(storedFilesPath);
            }

            var imagePath = Path.Combine(_config["StoredFilesPath"], randomName);

            // Save the image file to the specified path
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }

            return "/images/" + randomName; // Return the stored image path
        }

        private void DeleteImage(string imagePath)
        {
            // Construct the absolute path of the image file
            string absolutePath = "wwwroot" + imagePath;

            FileInfo file = new FileInfo(absolutePath);

            // Check if the image file exists
            if (file.Exists)
            {
                // Delete the image file
                file.Delete();
            }
        }
    }
}
