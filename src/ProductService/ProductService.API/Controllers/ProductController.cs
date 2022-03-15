using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.BL;
using ProductService.DAL.DTOs;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductManager pm;

        public ProductController(ProductManager pm) => this.pm = pm;

        [HttpGet]
        public async Task<IEnumerable<Product>> GetAll() => await pm.GetAllProducts();

        [HttpGet("{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Product>> Get(string productId)
        {
            var product = await pm.GetProductOrNull(productId);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpDelete("{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string productId)
        {
            var product = await pm.GetProductOrNull(productId);
            if (product == null)
                return NotFound();
            var result = await pm.DeleteProduct(productId, product);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            await pm.CreateProduct(product);
            if (product.ID != string.Empty)
                return CreatedAtAction(nameof(Get), new { id = product.ID }, product);
            return BadRequest();
        }

        [HttpPut("{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string productId, [FromBody] Product newProduct)
        {
            if (productId != newProduct.ID)
                return BadRequest();
            var product = await pm.GetProductOrNull(productId);
            if (product == null)
                return NotFound();

            var result = await pm.UpdateProduct(productId, product, newProduct);
            if (!result)
                return BadRequest();
            return Ok(newProduct);
        }
    }
}
