using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleService.DAL.Repositories
{
    public interface ISaleRepository
    {
        public Task<IReadOnlyCollection<DTOs.Sale>> GetAllSales();
        public Task<DTOs.Sale> GetSaleOrNull(string id);
        public Task<IReadOnlyCollection<DTOs.Sale>> GetSalesByOwnerId(string id);
        public Task<DTOs.Sale> GetSaleByProductGroupId(string id);
        public Task<IReadOnlyCollection<DTOs.Sale>> FindSales(FilterDefinition<Sale> filter);
        public Task<string> CreateSale(DTOs.Sale newSale);
        public Task<bool> UpdateSale(string id, DTOs.Sale newSale);
        public Task<bool> DeleteSale(string id);
        public Task<long> DeleteSalesByOwnerId(string ownerId);
    }
}
