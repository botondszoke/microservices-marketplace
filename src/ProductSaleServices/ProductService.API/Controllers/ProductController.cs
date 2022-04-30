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

        [Route("queries/all")]
        [HttpGet]
        public async Task<IEnumerable<Product>> Get() => await pm.GetAllProducts();

        [Route("queries/user")]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByOwnerId()
        {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

            if (ownerId.ToString() == null)
                return BadRequest();

            var products = await pm.GetProductsByOwnerId(ownerId.ToString());
            return Ok(products);
        }
        [HttpGet("queries/{productId}")]
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
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string productId)
        {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);
            
            if (ownerId.ToString() == null)
                return BadRequest();

            var product = await pm.GetProductOrNull(productId);
            
            if (product == null)
                return NotFound();

            if (ownerId.ToString() != product.OwnerID)
                return Unauthorized();

            var result = await pm.DeleteProduct(productId, product);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPost("actions/create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

            if (ownerId.ToString() == null)
                return BadRequest();

            if (product.OwnerID != ownerId.ToString())
                return Unauthorized();

            product.ID = await pm.CreateProduct(product);
            if (product.ID != string.Empty)
                return CreatedAtAction(nameof(Get), new { id = product.ID }, product);
            return BadRequest();
        }

        [HttpPut("actions/update/{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string productId, [FromBody] Product newProduct)
        {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

            if (productId != newProduct.ID || ownerId.ToString() == null)
                return BadRequest();

            var product = await pm.GetProductOrNull(productId);
            
            if (product == null)
                return NotFound();

            if (product.OwnerID != ownerId.ToString())
                return Unauthorized();

            var result = await pm.UpdateProduct(productId, product, newProduct);
            if (!result)
                return BadRequest();
            return Ok(newProduct);
        }

        [HttpPut("actions/sell/{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Sell(string productId)
        {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

            if (ownerId.ToString() == null)
                return BadRequest();

            var product = await pm.GetProductOrNull(productId);

            if (product.OwnerID == ownerId.ToString())
                return BadRequest();

            if (product == null)
                return NotFound();

            var result = await pm.SoldProduct(product, ownerId.ToString());
            if (!result)
                return BadRequest();

            product = await pm.GetProductOrNull(productId);

            return Ok(product);
        }

        [HttpPut("actions/sellByGroupId/{groupId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SellByGroupId(string groupId)
        {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

            if (ownerId.ToString() == null)
                return BadRequest();


            //TODO: Check if Group owner == ownerID move to here instead of function...
            var result = await pm.SoldProductFromGroup(groupId, ownerId.ToString());
            
            if (result == null)
                return BadRequest();
            
            if (result.ID == string.Empty)
                return NotFound();

            return Ok(result);
        }


    }
}
