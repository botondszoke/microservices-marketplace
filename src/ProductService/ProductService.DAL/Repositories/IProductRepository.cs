using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.Repositories
{
    public interface IProductRepository
    {
        public Task<IReadOnlyCollection<DTOs.Product>> GetAllProduct();
        public Task<DTOs.Product> GetProductOrNull(string id);
        public Task<IReadOnlyCollection<DTOs.Product>> GetProductsByOwnerId(string ownerId);
        public Task<IReadOnlyCollection<DTOs.Product>> GetProductsByGroupId(string groupId);
        public Task<IReadOnlyCollection<DTOs.Product>> FindProducts(FilterDefinition<Product> filter);
        public Task<string> CreateProduct(DTOs.Product newProduct);
        public Task<bool> DeleteProduct(string id);
        public Task<long> DeleteProductsByOwnerId(string ownerId);
        public Task<bool> UpdateProduct(string id, DTOs.Product product);
    }
}
