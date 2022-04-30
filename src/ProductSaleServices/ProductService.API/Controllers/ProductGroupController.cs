using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.BL;
using ProductService.DAL.DTOs;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductGroupController : ControllerBase
    {
        private readonly ProductGroupManager pm;

        public ProductGroupController(ProductGroupManager pm) => this.pm = pm;

        [Route("queries/all")]
        [HttpGet]
        public async Task<IEnumerable<ProductGroup>> Get() => await pm.GetAllProductGroups();

        [Route("queries/unavailable")]
        [HttpGet]
        public async Task<IEnumerable<ProductGroup>> GetUnavailableProductGroups() => await pm.GetUnavailableProductGroups();

        [Route("queries/user")]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<ProductGroup>>> GetProductGroupsByOwnerId() {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

            if (ownerId.ToString() == null)
                return BadRequest();

            var products = await pm.GetProductGroupsByOwnerId(ownerId);
            return Ok(products);
        }


        [HttpGet("queries/{productGroupId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Product>> Get(string productGroupId)
        {
            var group = await pm.GetProductGroupOrNull(productGroupId);
            if (group == null)
                return NotFound();
            return Ok(group);
        }

        [HttpDelete("{productGroupId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string productGroupId)
        {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

            if (ownerId.ToString() == null)
                return BadRequest();

            var group = await pm.GetProductGroupOrNull(productGroupId);
            
            if (group == null)
                return NotFound();
            
            if (ownerId.ToString() != group.OwnerID)
                return Unauthorized();

            var result = await pm.DeleteProductGroup(productGroupId);
            
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Create([FromBody] ProductGroup group)
        {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

            if (ownerId.ToString() == null)
                return BadRequest();

            if (group.OwnerID != ownerId.ToString())
                return Unauthorized();

            group = await pm.CreateProductGroup(group);
            if (group.ID != string.Empty)
                return CreatedAtAction(nameof(Get), new { id = group.ID }, group);
            return BadRequest();
        }

        [HttpPut("{productGroupId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string productGroupId, [FromBody] ProductGroup newProductGroup)
        {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

            if (productGroupId != newProductGroup.ID || ownerId.ToString() == null)
                return BadRequest();

            var productGroup = await pm.GetProductGroupOrNull(productGroupId);

            if (productGroup == null)
                return NotFound();

            if (productGroup.OwnerID != ownerId.ToString())
                return Unauthorized();

            var result = await pm.UpdateProductGroup(productGroupId, productGroup, newProductGroup);
            if (!result)
                return BadRequest();
            return Ok(newProductGroup);
        }
    }
}
