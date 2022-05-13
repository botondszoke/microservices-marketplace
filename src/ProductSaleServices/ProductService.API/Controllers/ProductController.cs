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
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] string? filter = "all")
        {
            if (filter == "user")
            {
                // Authentication result in X-Forwarded-User
                Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

                if (ownerId.ToString() == null)
                    return BadRequest();

                var ownerProducts = await pm.GetProductsByOwnerId(ownerId.ToString());
                return Ok(ownerProducts);
            }
            var products = await pm.GetAllProducts();
            return Ok(products);

        }
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

        [HttpPost()]
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
                return CreatedAtAction(nameof(GetProducts), new { id = product.ID }, product);
            return BadRequest();
        }

        [HttpPut("{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string productId, [FromBody] Product? newProduct = null, [FromQuery] string? action = "update")
        {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

            if (ownerId.ToString() == null)
                return BadRequest();

            if (action == "sell")
            {
                var sellProduct = await pm.GetProductOrNull(productId);

                if (sellProduct == null)
                    return NotFound();

                if (sellProduct.OwnerID == ownerId.ToString())
                    return BadRequest();

                var sellResult = await pm.SoldProduct(sellProduct, ownerId.ToString());
                if (!sellResult)
                    return BadRequest();

                sellProduct = await pm.GetProductOrNull(productId);

                return Ok(sellProduct);
            }

            if (newProduct.ID == null || productId != newProduct.ID)
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

        [HttpPut()]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SellByGroupId([FromQuery] string groupId, [FromQuery] int quantity)
        {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

            if (ownerId.ToString() == null)
                return BadRequest();

            if (quantity <= 0)
                return BadRequest();

            //TODO: Check if Group owner == ownerID move to here instead of function...
            var result = await pm.SoldProductFromGroup(groupId, ownerId.ToString(), quantity);
            
            if (result.Count != quantity || result.Count == 0)
                return BadRequest();
            
            if (result[0].ID == string.Empty)
                return NotFound();

            var emailResult = await pm.SendPurchaseEmail(result[0], quantity, ownerId.ToString());

            return Ok(result);
        }


    }
}
