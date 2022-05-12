using SaleService.DAL.DTOs;
using SaleService.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleService.BL
{
    public class SaleManager
    {
        private readonly ISaleRepository _saleRepository;

        public SaleManager(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<IReadOnlyCollection<Sale>> GetAllSales()
            => await _saleRepository.GetAllSales();

        public async Task<Sale> GetSaleOrNull(string id)
            => await _saleRepository.GetSaleOrNull(id);

        public async Task<IReadOnlyCollection<Sale>> GetOwnerSales(string ownerId)
            => await _saleRepository.GetSalesByOwnerId(ownerId);

        public async Task<Sale> GetSaleByProductGroupId(string id)
            => await _saleRepository.GetSaleByProductGroupId(id);

        public async Task<string> CreateSale(Sale sale)
        {
            if (sale.OwnerID == null || sale.ProductGroupID == null || sale.UnitPrice <= 0)
                return string.Empty;

            return await _saleRepository.CreateSale(sale);
        }

        public async Task<bool> UpdateSale(string id, Sale newSale)
        {
            return await _saleRepository.UpdateSale(id, newSale);
        }

        public async Task<bool> DeleteSale(string id)
        {
            return await _saleRepository.DeleteSale(id);
        }
    }
}
