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
        public async Task<IEnumerable<Sale>> Get() => await sm.GetAllSales();

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
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string saleId)
        {
            var sale = await sm.GetSaleOrNull(saleId);
            if (sale == null)
                return NotFound();
            var result = await sm.DeleteSale(saleId);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Sale sale)
        {
            sale.ID = await sm.CreateSale(sale);
            if (sale.ID != string.Empty)
                return CreatedAtAction(nameof(Get), new { id = sale.ID }, sale);
            return BadRequest();
        }

        [HttpPut("{saleId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string saleId, [FromBody] Sale newSale)
        {
            if (saleId != newSale.ID)
                return BadRequest();
            var sale = await sm.GetSaleOrNull(saleId);
            if (sale == null)
                return NotFound();
            var result = await sm.UpdateSale(saleId, newSale);
            if(!result)
                return BadRequest();
            return Ok(newSale);
        }
    }
}
