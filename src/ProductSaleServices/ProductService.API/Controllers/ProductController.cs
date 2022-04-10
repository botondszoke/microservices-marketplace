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

        [Route("queries")]
        [HttpGet]
        public async Task<IEnumerable<Product>> Get() => await pm.GetAllProducts();

        [Route("queries/ownerId/{ownerId}")]
        [HttpGet]
        public async Task<IEnumerable<Product>> GetProductsByOwnerId(string ownerId) => await pm.GetProductsByOwnerId(ownerId);

        [HttpGet("queries/id/{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Product>> Get(string productId)
        {
            var product = await pm.GetProductOrNull(productId);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpDelete("actions/delete/{productId}")]
        [ProducesResponseType(204)]
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

        [HttpPost("actions/create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            product.ID = await pm.CreateProduct(product);
            if (product.ID != string.Empty)
                return CreatedAtAction(nameof(Get), new { id = product.ID }, product);
            return BadRequest();
        }

        [HttpPut("actions/update/{productId}")]
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

        [HttpPut("actions/sell/{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Sell(string productId, [FromBody] Product newProduct)
        {
            if (productId != newProduct.ID)
                return BadRequest();
            var product = await pm.GetProductOrNull(productId);
            if (product == null)
                return NotFound();

            var result = await pm.SoldProduct(product, newProduct.OwnerID);
            if (!result)
                return BadRequest();
            return Ok(newProduct);
        }

        [HttpPut("actions/sellByGroupId/{groupId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SellByGroupId(string groupId, [FromBody] Product newProduct)
        {
            var result = await pm.SoldProductFromGroup(groupId, newProduct.OwnerID);
            if (result == null)
                return BadRequest();
            if (result.ID == string.Empty)
                return NotFound();
            return Ok(result);
        }


    }
}
