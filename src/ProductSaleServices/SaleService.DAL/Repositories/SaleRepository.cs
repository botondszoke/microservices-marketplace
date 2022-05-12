using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaleService.DAL.SaleDatabase;

namespace SaleService.DAL.Repositories
{
    internal static class SaleConverter
    {
        public static DTOs.Sale ConvertFromDb(Sale sale)
        {
            if (sale == null)
                return null;
            return new DTOs.Sale
            {
                ID = sale.ID.ToString(),
                OwnerID = sale.OwnerID.ToString(),
                ProductGroupID = sale.ProductGroupID.ToString(),
                UnitPrice = sale.UnitPrice,
                Currency = Enum.Parse<DTOs.Currency>(sale.Currency),
            };
        }

        public static Sale ConvertToDb(DTOs.Sale sale)
        {
            if (sale == null)
                return null;
            return new Sale
            {
                ID = sale.ID == string.Empty ? ObjectId.Empty : ObjectId.Parse(sale.ID),
                OwnerID = sale.OwnerID,
                ProductGroupID = ObjectId.Parse(sale.ProductGroupID),
                UnitPrice = sale.UnitPrice,
                Currency = sale.Currency.ToString()
            };
        }

        public async static Task<List<DTOs.Sale>> ConvertSalesFromDb(this IFindFluent<Sale, Sale> _sales)
        {
            var dbSales = await _sales.ToListAsync();
            List<DTOs.Sale> sales = new List<DTOs.Sale>();
            foreach (var sale in dbSales)
            {
                sales.Add(ConvertFromDb(sale));
            }
            return sales;
        }
    }
    public class SaleRepository : ISaleRepository
    {
        private readonly ISaleContext _context;

        public SaleRepository(ISaleContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<DTOs.Sale>> GetAllSales()
        {
            return await _context
                .Sales
                .Find(_ => true)
                .ConvertSalesFromDb();
        }

        public async Task<DTOs.Sale> GetSaleOrNull(string id)
        {
            var sale = await _context
                .Sales
                .Find(s => s.ID.Equals(ObjectId.Parse(id)))
                .FirstOrDefaultAsync();
            return SaleConverter.ConvertFromDb(sale);
        }

        public async Task<IReadOnlyCollection<DTOs.Sale>> GetSalesByOwnerId(string id)
        {
            return await _context
                .Sales
                .Find(s => s.OwnerID.Equals(id))
                .ConvertSalesFromDb();
        }

        public async Task<DTOs.Sale> GetSaleByProductGroupId(string id)
        {
            var sale = await _context
                .Sales
                .Find(s => s.ProductGroupID.Equals(ObjectId.Parse(id)))
                .FirstOrDefaultAsync();
            return SaleConverter.ConvertFromDb(sale);
        }

        public async Task<IReadOnlyCollection<DTOs.Sale>> FindSales(FilterDefinition<Sale> filter)
        {
            return await _context
                .Sales
                .Find(filter)
                .ConvertSalesFromDb();
        }

        public async Task<string> CreateSale (DTOs.Sale newSale)
        {
            newSale.ID = string.Empty;
            var dbSale = SaleConverter.ConvertToDb(newSale);
            await _context
                .Sales
                .InsertOneAsync(dbSale);
            newSale = SaleConverter.ConvertFromDb(dbSale);
            return newSale.ID;
        }

        public async Task<bool> UpdateSale (string id, DTOs.Sale newSale)
        {
            var update = await _context
                .Sales
                .ReplaceOneAsync(s => s.ID.Equals(ObjectId.Parse(id)), SaleConverter.ConvertToDb(newSale));

            return update.IsAcknowledged && update.ModifiedCount > 0;
        }

        public async Task<bool> DeleteSale (string id)
        {
            var delete = await _context
                .Sales
                .DeleteOneAsync(s => s.ID.Equals(ObjectId.Parse(id)));
            return delete.IsAcknowledged && delete.DeletedCount > 0;
        }

        public async Task<long> DeleteSalesByOwnerId(string ownerId)
        {
            var delete = await _context
                .Sales
                .DeleteManyAsync(s => s.ID.Equals(ObjectId.Parse(ownerId)));

            return delete.IsAcknowledged ? delete.DeletedCount : 0;
        }
    }
}
