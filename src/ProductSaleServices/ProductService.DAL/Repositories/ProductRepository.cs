using MongoDB.Bson;
using MongoDB.Driver;
using ProductService.DAL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.Repositories
{
    internal static class ProductConverter
    {
        public static DTOs.Product ConvertFromDb(Product product)
        {
            if (product == null)
                return null;
            return new DTOs.Product 
            { 
                ID = product.ID.ToString(), 
                OwnerID = product.OwnerID.ToString(),
                GroupID = product.GroupID?.ToString(),
                Name = product.Name, 
                Condition = product.Condition, 
                Description = product.Description, 
                IsAvailable = product.IsAvailable, 
                PictureLinks = product.PictureLinks 
            };

        }

        public static Product ConvertToDb(DTOs.Product product)
        {
            if (product == null)
                return null;
            return new Product
            {
                ID = product.ID == string.Empty ? ObjectId.Empty : ObjectId.Parse(product.ID),
                OwnerID = product.OwnerID,
                GroupID = product.GroupID == null ? null : ObjectId.Parse(product.GroupID),
                Name = product.Name,
                Condition = product.Condition,
                Description = product.Description,
                IsAvailable = product.IsAvailable,
                PictureLinks = product.PictureLinks
            };
        }

        public async static Task<List<DTOs.Product>> ConvertProductsFromDb(this IFindFluent<Product, Product> _products)
        {
            var dbProducts = await _products.ToListAsync();
            List<DTOs.Product> products = new List<DTOs.Product>();
            foreach (var product in dbProducts)
            {
                products.Add(ConvertFromDb(product));
            }
            return products;
        }
    }
    public class ProductRepository : IProductRepository
    {
        private readonly IProductContext _context;

        public ProductRepository(IProductContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<DTOs.Product>> GetAllProduct()
        {
            return await _context
                    .Products
                    .Find(_ => true)
                    .ConvertProductsFromDb();
        }

        public async Task<DTOs.Product> GetProductOrNull(string id)
        {
            var product = await _context
                .Products
                .Find(p => p.ID.Equals(ObjectId.Parse(id)))
                .FirstOrDefaultAsync();
            return ProductConverter.ConvertFromDb(product);
        }

        public async Task<IReadOnlyCollection<DTOs.Product>> GetProductsByOwnerId(string ownerId)
        {
            return await _context
                .Products
                .Find(p => p.OwnerID.Equals(ObjectId.Parse(ownerId)))
                .ConvertProductsFromDb();
        }

        public async Task<IReadOnlyCollection<DTOs.Product>> GetProductsByGroupId(string groupId)
        {
            return await _context
                .Products
                .Find(p => p.GroupID.Equals(ObjectId.Parse(groupId)))
                .ConvertProductsFromDb();
        }

        public async Task<IReadOnlyCollection<DTOs.Product>> FindProducts(FilterDefinition<Product> filter)
        {
            return await _context
                .Products
                .Find(filter)
                .ConvertProductsFromDb();
        }

        public async Task<string> CreateProduct(DTOs.Product newProduct)
        {
            newProduct.ID = string.Empty;
            var dbProduct = ProductConverter.ConvertToDb(newProduct);
            await _context
                .Products
                .InsertOneAsync(dbProduct);
            newProduct = ProductConverter.ConvertFromDb(dbProduct);
            return newProduct.ID;
        }

        public async Task<bool> DeleteProduct(string id)
        {
            var delete = await _context
                .Products
                .DeleteOneAsync(p => p.ID.Equals(ObjectId.Parse(id)));
            return delete.IsAcknowledged && delete.DeletedCount > 0;
        }

        public async Task<long> DeleteProductsByOwnerId(string ownerId)
        {
            var delete = await _context
                .Products
                .DeleteManyAsync(p => p.OwnerID.Equals(ObjectId.Parse(ownerId)));
            return delete.IsAcknowledged ? delete.DeletedCount : 0;
        }

        public async Task<long> DeleteProductsByGroupId(string groupId)
        {
            var delete = await _context
                .Products
                .DeleteManyAsync(p => p.GroupID.Equals(ObjectId.Parse(groupId)));
            return delete.IsAcknowledged ? delete.DeletedCount : 0;
        }

        public async Task<bool> UpdateProduct(string id, DTOs.Product product)
        {
            var update = await _context
                .Products
                .ReplaceOneAsync(p => p.ID.Equals(ObjectId.Parse(id)), ProductConverter.ConvertToDb(product));
            return update.IsAcknowledged && update.ModifiedCount > 0;
        }
    }
}
