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

        [HttpGet]
        public async Task<IEnumerable<ProductGroup>> Get() => await pm.GetAllProductGroups();

        [HttpGet("{productGroupId}")]
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
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string productGroupId)
        {
            var group = await pm.GetProductGroupOrNull(productGroupId);
            if (group == null)
                return NotFound();
            var result = await pm.DeleteProductGroup(productGroupId);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] ProductGroup group)
        {
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
            if (productGroupId != newProductGroup.ID)
                return BadRequest();
            var productGroup = await pm.GetProductGroupOrNull(productGroupId);
            if (productGroup == null)
                return NotFound();

            var result = await pm.UpdateProductGroup(productGroupId, productGroup, newProductGroup);
            if (!result)
                return BadRequest();
            return Ok(newProductGroup);
        }
    }
}
