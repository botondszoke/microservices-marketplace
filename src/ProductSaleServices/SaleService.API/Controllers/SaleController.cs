using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaleService.BL;
using SaleService.DAL.DTOs;

namespace SaleService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly SaleManager sm;

        public SaleController(SaleManager sm) => this.sm = sm;

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSales([FromQuery] string? filter = "all", [FromQuery] string? productGroupId = "0")
        {
            if (filter == "user")
            {
                // Authentication result in X-Forwarded-User
                Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

                if (ownerId.ToString() == null)
                    return BadRequest();
                var sales = await sm.GetOwnerSales(ownerId.ToString());
                return Ok(sales);
            }

            if (filter == "productGroupId")
            {
                // Authentication result in X-Forwarded-User
                Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);
                if (ownerId.ToString() == null)
                    return BadRequest();
                var sale = await sm.GetSaleByProductGroupId(productGroupId);
                return Ok(sale);
            }

            var result = await sm.GetAllSales();
            return Ok(result);
        }

        [HttpGet("{saleId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Sale>> Get(string saleId)
        {
            var sale = await sm.GetSaleOrNull(saleId);
            if (sale == null)
                return NotFound();
            return Ok(sale);
        }

        [HttpDelete("{saleId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string saleId)
        {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

            if (ownerId.ToString() == null)
                return BadRequest();

            var sale = await sm.GetSaleOrNull(saleId);

            if (sale == null)
                return NotFound();

            if (ownerId.ToString() != sale.OwnerID)
                return Unauthorized();

            var result = await sm.DeleteSale(saleId);

            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPost()]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Create([FromBody] Sale sale)
        {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

            if (ownerId.ToString() == null)
                return BadRequest();

            if (sale.OwnerID != ownerId.ToString())
                return Unauthorized();

            sale.ID = await sm.CreateSale(sale);
            if (sale.ID != string.Empty)
                return CreatedAtAction(nameof(GetSales), new { id = sale.ID }, sale);
            return BadRequest();
        }

        [HttpPut("{saleId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string saleId, [FromBody] Sale newSale)
        {
            // Authentication result in X-Forwarded-User
            Request.Headers.TryGetValue("X-Forwarded-User", out Microsoft.Extensions.Primitives.StringValues ownerId);

            if (saleId != newSale.ID || ownerId.ToString() == null)
                return BadRequest();
            
            var sale = await sm.GetSaleOrNull(saleId);
            
            if (sale == null)
                return NotFound();
            
            if (sale.OwnerID != ownerId.ToString())
                return Unauthorized();
            
            var result = await sm.UpdateSale(saleId, newSale);
            if(!result)
                return BadRequest();
            return Ok(newSale);
        }
    }
}
