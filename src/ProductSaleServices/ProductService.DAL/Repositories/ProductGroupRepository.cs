using MongoDB.Bson;
using MongoDB.Driver;
using ProductService.DAL.ProductDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.Repositories
{
    internal static class ProductGroupConverter
    {
        public static DTOs.ProductGroup ConvertFromDb(ProductGroup productGroup)
        {
            if (productGroup == null)
                return null;
            return new DTOs.ProductGroup
            {
                ID = productGroup.ID.ToString(),
                OwnerID = productGroup.OwnerID.ToString(),
                SampleProduct = ProductConverter.ConvertFromDb(productGroup.SampleProduct),
                //ProductIDs = productGroup.ProductIDs.Select(p => p.ToString()).ToArray(),
                Quantity = productGroup.Quantity
            };

        }

        public static ProductGroup ConvertToDb(DTOs.ProductGroup productGroup)
        {
            if (productGroup == null)
                return null;
            return new ProductGroup
            {
                ID = productGroup.ID == string.Empty ? ObjectId.Empty : ObjectId.Parse(productGroup.ID),
                OwnerID = productGroup.OwnerID,
                SampleProduct = ProductConverter.ConvertToDb(productGroup.SampleProduct),
                //ProductIDs = productGroup.ProductIDs.Select(p => ObjectId.Parse(p.ToString())).ToArray(), 
                Quantity = productGroup.Quantity
            };
        }

        public async static Task<List<DTOs.ProductGroup>> ConvertProductGroupsFromDb(this IFindFluent<ProductGroup, ProductGroup> _productGroups)
        {
            var dbProductGroups = await _productGroups.ToListAsync();
            List<DTOs.ProductGroup> productGroups = new List<DTOs.ProductGroup>();
            foreach (var productGroup in dbProductGroups)
            {
                productGroups.Add(ConvertFromDb(productGroup));
            }
            return productGroups;
        }
    }
    public class ProductGroupRepository : IProductGroupRepository
    {
        private readonly IProductContext _context;

        public ProductGroupRepository(IProductContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<DTOs.ProductGroup>> GetAllProductGroups()
        {
            return await _context
                    .ProductGroups
                    .Find(_ => true)
                    .ConvertProductGroupsFromDb();
        }

        public async Task<DTOs.ProductGroup> GetProductGroupOrNull(string id)
        {
            var productGroup = await _context
                .ProductGroups
                .Find(p => p.ID.Equals(ObjectId.Parse(id)))
                .FirstOrDefaultAsync();
            return ProductGroupConverter.ConvertFromDb(productGroup);
        }

        public async Task<IReadOnlyCollection<DTOs.ProductGroup>> GetProductGroupsByOwnerId(string ownerId)
        {
            return await _context
                .ProductGroups
                .Find(p => p.OwnerID.Equals(ownerId))
                .ConvertProductGroupsFromDb();
        }

        public async Task<IReadOnlyCollection<DTOs.ProductGroup>> GetUnavailableProductGroups()
        {
            return await _context
                .ProductGroups
                .Find(p => p.SampleProduct.IsAvailable == false)
                .ConvertProductGroupsFromDb();
        }

        public async Task<DTOs.ProductGroup> CreateProductGroup(DTOs.ProductGroup productGroup)
        {
            productGroup.ID = string.Empty;
            productGroup.SampleProduct.ID = string.Empty;
            productGroup.SampleProduct.GroupID = null;
            var dbProductGroup = ProductGroupConverter.ConvertToDb(productGroup);
            await _context
                .ProductGroups
                .InsertOneAsync(dbProductGroup);

            dbProductGroup.SampleProduct.GroupID = dbProductGroup.ID;
            var update = await _context
                .ProductGroups
                .ReplaceOneAsync(p => p.ID.Equals(dbProductGroup.ID), dbProductGroup);
            if (!(update.IsAcknowledged && update.ModifiedCount > 0))
                return productGroup;

            productGroup = ProductGroupConverter.ConvertFromDb(dbProductGroup);
            return productGroup;
        }

        public async Task<bool> DeleteProductGroup(string id)
        {
            var delete = await _context
                .ProductGroups
                .DeleteOneAsync(p => p.ID.Equals(ObjectId.Parse(id)));
            return delete.IsAcknowledged && delete.DeletedCount > 0;
        }

        public async Task<bool> UpdateProductGroup(string id, DTOs.ProductGroup productGroup)
        {
            var update = await _context
                .ProductGroups
                .ReplaceOneAsync(p => p.ID.Equals(ObjectId.Parse(id)), ProductGroupConverter.ConvertToDb(productGroup));
            return update.IsAcknowledged && update.ModifiedCount > 0;
        }

        public async Task<bool> RemoveProductFromGroup(string id)
        {
            var group = await GetProductGroupOrNull(id);
            if (group == null)
                return false;
            DTOs.ProductGroup updated = new()
            {
                ID = group.ID,
                OwnerID = group.OwnerID,
                SampleProduct = group.SampleProduct,
                Quantity = group.Quantity - 1,
            };
            return await UpdateProductGroup(id, updated);
        }
        public async Task<bool> AddProductToGroup(string id)
        {
            var group = await GetProductGroupOrNull(id);
            if (group == null)
                return false;
            DTOs.ProductGroup updated = new()
            {
                ID = group.ID,
                OwnerID = group.OwnerID,
                SampleProduct = group.SampleProduct,
                Quantity = group.Quantity + 1,
            };
            return await UpdateProductGroup(id, updated);
        }
    }
}
